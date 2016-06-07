using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    //StatConstants                     //Speed, Acceleration, Brakes, Boost
    private static readonly float[] DEFAULT_STATS = { 8, 16, 1.5f, 0.5f }; //Default base stats
    public const float INPUT_THRESHOLD_LOW = 0.1f; //In deadzone if magnitude < this threshold
    public const float VELOCITY_THRESHOLD_LOW = 0.3f; //In deadzone if magnitude < this threshold
    private static readonly float[] BOOST_PER_STAT = { 0.4f, 0.12f, 0.15f, 0.09f }; //How much each stat changes per stat level
    private static readonly float[] ROPELESS_PENALTY = { 0.6f, 0.5f, 0.8f, 1.0f }; //How much each stat is punished for being ropeless
    private static readonly Color[] COLORS = {
      new Color(1f, 0.6f, 0.6f, 1f),
      new Color(1f, 1.0f, 0.6f, 1f),
      new Color(0.6f, 0.8f, 1.0f, 1f),
      new Color(0.8f, 1.0f, 0.6f, 1f)
    };

    private const int DEGREES_IN_CIRCLE = 360;

    //Brakes stat computes drag amount
    //Multiply by this to get %charged per second
    public const float BRAKES_TO_CHARGE_SPEED = 0.75f;
    //Multiplier for how long your boost lasts depending on boost stat
    public const float BOOST_TO_BOOST_TIME = 0.375f;
    //How much angular velocity should be applied, based on degrees you're off
    public static readonly float OFFSET_TO_ANGULAR_VELOCITY = 4 * DEGREES_IN_CIRCLE;

    //Instance variables
    public bool isPlaying = false;
    public Color color;
    private PlayerCanvas canvas;
    public new Camera camera;
    public Rigidbody2D body;

    // Audio
    public CollisionSound collisionSound;
    public CollisionSound powerupSound;
    public AudioClip powerupClip;
    public AudioClip powerdownClip;

    private RopeController playerRope = null;
    private bool didChangeAttachedPlayer = false;
    private int attachedPlayerIndex = -1;
    //private Animator animator;
    private float[] baseStats; //Instance variable in case we want to allow multiple vehicles
    private int[] statLevels;
    public int mostRecentTimeStatStolen = 0;

    private bool charging = false;
    private float chargeStart;
    private float boostEnd = -100; //Boost end time. Initialized to negative so you're not starting in a boost

    private ParticleSystem exhaustParticles;
    private const float MAX_EXHAUST = 50;
    // Set in editor. Determines which controller to use.
    public int index = 0;
    // Set in editor. The rope's prefab.
    //public RopeController rope;

    /**
	 * Returns what percent charged you are
	 * Maximum of 100%
	 * Zero when not currently charging
	 */
    private float ChargePercent() {
        if (charging)
            return Mathf.Min((Time.time - chargeStart) * ComputeStat(StatConstants.BRAKES), 1.0f);
        else
            return 0;
    }

    /**
	 * Returns whether the character is currently boosting
	 */
    public bool IsBoosting() {
        return boostEnd > Time.time;
    }

    public int[] GetStatLevels() {
        return statLevels;
    }

    /**
     * Returns the index of the attached player
     * -1 if not attached
     */
    public int GetAttachedPlayer() {
        return attachedPlayerIndex;
    }

    /**
     * Resets the attached player index
     * Does not affect the rope
     */
    public void ResetAttachedPlayer() {
        attachedPlayerIndex = -1;
    }

    /**
     * Returns whether you're attached by a rope to another player
     */
    public bool DidChangeAttachedPlayer() {
        return didChangeAttachedPlayer;
    }

    /**
     * Returns whether you're attached by a rope to another player
     */
    public bool IsAttached() {
        return attachedPlayerIndex != -1;
    }

    /**
     * Set data about which rope is being attached and who you're attaching to
     */
    public void SetPlayerRope(RopeController rope, int otherPlayerIndex) {
        playerRope = rope;
        attachedPlayerIndex = otherPlayerIndex;
    }

    /**
     * Return the rope attached to this player
     * null if unattached
     */
    public RopeController GetPlayerRope() {
        return playerRope;
    }

    //Gives current value of a stat, given base value and current level
    private float ComputeStat(int statNum) {
        int statLevel = statLevels[statNum];
        float baseStat = baseStats[statNum];
        //Punish for not having a rope
        if (attachedPlayerIndex == -1) {
            baseStat *= ROPELESS_PENALTY[statNum];
        }
        float percentChange = BOOST_PER_STAT[statNum];
        if (statLevel > 0)
            return baseStat * (1.0f + percentChange * statLevel);
        else if (statLevel < 0)
            return baseStat * (Mathf.Pow(1.0f - percentChange, -statLevel));
        else
            return baseStat;
    }

    //Computes the current drag coefficient, assuming quadratic drag
    private float DragCoefficient() {
        return (ComputeStat(StatConstants.ACCELERATION) / (ComputeStat(StatConstants.SPEED))
            / ComputeStat(StatConstants.SPEED));
    }

    /*
	 * Gets input, normalizes if necessary
	 * Returns 0 if within deadzone
	 */
    private Vector2 InputVector() {
        //Figure out which direction to travel in  
        //Vector2 input = new Vector2(Input.GetAxisRaw("X" + index), Input.GetAxisRaw("Y" + index));
        Vector2 input = new Vector2(Input.GetAxisRaw("X" + index), Input.GetAxisRaw("Y" + index));
        if (index == 1) {
            input.x += Input.GetAxisRaw("X1 Alt");
            input.y += Input.GetAxisRaw("Y1 Alt");
        }


        if (input.magnitude < INPUT_THRESHOLD_LOW)
            input = Vector2.zero;

        //Turn input on the unit square into input on the unit circle
        //So that input of (1,1) becomes (1/sqrt(2), 1/sqrt(2)) and so on
        if (input != Vector2.zero) {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
                //Divide by secant from right, by definition:
                //https://en.wikipedia.org/wiki/File:Unitcircledefs.svg
                input *= Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(Vector2.up, input));
            } else {
                //Divide by cosecant from right, by definition:
                //https://en.wikipedia.org/wiki/File:Unitcirclecodefs.svg
                input *= Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(Vector2.right, input));
            }
        }

        return input;
    }

    /*
	 * If it isn't zero, turns the character in the direction of movement
	 */
    private void TurnCharacter() {
        Vector2 desiredDirection = Vector2.zero;
        if (InputVector().magnitude > INPUT_THRESHOLD_LOW) {
            if (charging)
                desiredDirection = InputVector();
            else if (body.velocity.magnitude > VELOCITY_THRESHOLD_LOW)
                desiredDirection = body.velocity;
        }

        //Quickly turn to desired direction
        if (desiredDirection != Vector2.zero) {
            float angle = Vector2.Angle(Vector2.up, desiredDirection);
            if (desiredDirection.x > 0)
                angle = DEGREES_IN_CIRCLE - angle;

            float angleDifference = angle - transform.eulerAngles.z;
            if (angleDifference < -DEGREES_IN_CIRCLE / 2)
                angleDifference += DEGREES_IN_CIRCLE;
            else if (angleDifference > DEGREES_IN_CIRCLE / 2)
                angleDifference -= DEGREES_IN_CIRCLE;

            //Quickly go, faster for a bigger angle difference. Biggest when difference is 180
            body.angularVelocity = OFFSET_TO_ANGULAR_VELOCITY * Mathf.Sin(Mathf.PI * angleDifference / (DEGREES_IN_CIRCLE * 2));
        }
    }

    /**
     * Sets whether the charge indicators are visible
     */
    private void SetChargeIndicatorVisibility(bool visible) {
        transform.Find("ChargeMax").gameObject.GetComponent<SpriteRenderer>().enabled = visible;
        transform.Find("ChargeLevel").gameObject.GetComponent<SpriteRenderer>().enabled = visible;
    }

    /**
     * Call while charging.
     * Sets the charge indicator to the appropriate size
     */
    private void ResizeChargeIndicator() {
        GameObject chargeLevel = transform.Find("ChargeLevel").gameObject;
        float planarScale = transform.Find("ChargeMax").localScale.x * ChargePercent();
        Vector3 newScale = chargeLevel.transform.localScale;
        newScale.x = planarScale;
        newScale.y = planarScale;
        chargeLevel.transform.localScale = newScale;
    }

    /**
	 * Tell the character to charge
	 */
    private void Charge() {
        //canvas.ShowMessage("CHARGING"); // probably remove this later
        charging = true;
        chargeStart = Time.time;

        SetChargeIndicatorVisibility(true);
    }

    /**
	 * Tell the character to boost
	 */
    private void Boost() {
        float chargePercent = ChargePercent();
        float boostPercent = chargePercent * chargePercent * (1.0f + ComputeStat(StatConstants.BOOST));
        float boostAmount = boostPercent * ComputeStat(StatConstants.SPEED);

        Vector2 direction = InputVector();
        if (direction == Vector2.zero) {
            //TODO: get angle from the z euler angle
            float zAngle = Mathf.Deg2Rad * transform.eulerAngles.z;
            direction = new Vector2(-Mathf.Sin(zAngle), Mathf.Cos(zAngle));
        } else {
            direction.Normalize();
        }

        body.AddForce(direction * boostAmount * body.mass, ForceMode2D.Impulse);
        boostEnd = Time.time + boostPercent * BOOST_TO_BOOST_TIME;
        charging = false;
        exhaustParticles.emissionRate = 0;
        SetChargeIndicatorVisibility(false);
    }

    //Called after everything is initialized
    void Awake() {
        SetChargeIndicatorVisibility(false);
        DontDestroyOnLoad(gameObject);
    }

    //Called on the object's creation
    void Start() {
        color = COLORS[index - 1];
        canvas = UICreator.instance.addPlayerCanvas(this);
        camera = GetComponentInChildren<Camera>();

        gameObject.GetComponent<SpriteRenderer>().color = color;
        body = gameObject.GetComponent<Rigidbody2D>();

        GameObject smoke = Resources.Load("Smoke") as GameObject;
        GameObject playerExhaust = Instantiate(smoke);
        playerExhaust.transform.SetParent(gameObject.transform);
        playerExhaust.transform.localPosition = new Vector3(0f, 0f, 0f);
        playerExhaust.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        exhaustParticles = playerExhaust.GetComponent<ParticleSystem>();
        exhaustParticles.simulationSpace = ParticleSystemSimulationSpace.World;
        //animator = gameObject.GetComponent<Animator>();

        //Inintialize stat levels, defaulting to 0
        statLevels = new int[StatConstants.NUM_STATS];
        //Initiate base stats
        baseStats = (float[])DEFAULT_STATS.Clone();

        GameManager.instance.CreatePlayer(this);
    }

    void StartPlaying() {
        isPlaying = true;
        canvas.StartGame();
        GameManager.instance.StartPlayer(this);
    }

    //Called once per physics step
    void FixedUpdate() {
        if (isPlaying) {
            Vector2 force = InputVector();

            //Apply force
            if (force != Vector2.zero) {
                body.drag = 0;
                force *= ComputeStat(StatConstants.ACCELERATION) * (1.0f - ChargePercent());
                body.AddForce(force * body.mass);
            }
            if (force == Vector2.zero || charging) {
                //Apply brakes with linear drag
                body.drag = ComputeStat(StatConstants.BRAKES);
            }

            //Handle top speed with quadratic drag
            Vector2 currVelocity = body.velocity;
            float magnitude = currVelocity.magnitude;
            currVelocity.Normalize();
            body.AddForce(-currVelocity * magnitude * magnitude * DragCoefficient() * body.mass);

            TurnCharacter();
        } else {
            //Brake before you start
            body.drag = ComputeStat(StatConstants.BRAKES);
        }
    }

    // Update is called once per frame
    void Update() {
        //Charge or boost
        if ((index == 1 && Input.GetKeyDown("space")) || Input.GetKeyDown(KeyCodes.GetA(index)) || Input.GetKeyDown(KeyCodes.GetZ(index))) {
            if (!isPlaying) {
                if (GameManager.instance.CanJoin()) {
                    StartPlaying();
                }
            } else { //if (Time.time > boostEnd) {
                Charge();
            }
        }

        if (charging) {
            exhaustParticles.emissionRate = MAX_EXHAUST * ChargePercent();
            ResizeChargeIndicator();
        }

        if ((index == 1 && Input.GetKeyUp("space")) || Input.GetKeyUp(KeyCodes.GetA(index)) || Input.GetKeyUp(KeyCodes.GetZ(index))
        && charging) {//Make sure you aren't being tricky with buttons or burned out if we implement that
            Boost();
        }

        canvas.Update();
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player") {
            PlayerController otherPlayer = coll.gameObject.GetComponent<PlayerController>();                //Attach rope if:
            if ((IsBoosting() || otherPlayer.IsBoosting())//1. Someone's boosting
                      && attachedPlayerIndex != otherPlayer.index && index != otherPlayer.GetAttachedPlayer()//2. Not already attached
                      && otherPlayer.index > index) {                                                           //Only make 1 rope
                                                                                                                //Delete any old ropes
                if (IsAttached())
                    playerRope.DeleteRope();
                if (otherPlayer.IsAttached())
                    otherPlayer.GetPlayerRope().DeleteRope();

                //Mark as new player attached through rope
                didChangeAttachedPlayer = true;

                GameObject ropePrefab = Resources.Load("Rope") as GameObject;
                Vector3 location = (transform.position + otherPlayer.transform.position) / 2;
                RopeController rope = Instantiate(ropePrefab).GetComponent<RopeController>();

                SetPlayerRope(rope, otherPlayer.index);
                otherPlayer.SetPlayerRope(rope, index);
                rope.MakeRope(transform, coll.transform, 0.1f, 16, location);
            } else if (attachedPlayerIndex == otherPlayer.index) {
                didChangeAttachedPlayer = false;
            }
            GameManager.instance.OnPlayerCollision(this, otherPlayer);
        } else if (coll.gameObject.tag == "RollercoasterCar") {
            if (IsBoosting()) {
                if (gameObject.transform.parent == coll.gameObject.transform) {
                    gameObject.transform.SetParent(null);
                    Vector2 input = InputVector() * 2;
                    gameObject.transform.position = coll.gameObject.transform.position + new Vector3(input.x, input.y, 0);
                } else {
                    gameObject.transform.SetParent(coll.gameObject.transform);
                    gameObject.transform.localPosition = Vector3.zero;
                    Rigidbody2D rollercoasterRigidbody = coll.gameObject.GetComponent<Rigidbody2D>();
                    rollercoasterRigidbody.velocity = Vector3.zero;
                    rollercoasterRigidbody.angularVelocity = 0f;
                    boostEnd = -100;
                }

            }

        }

        if (coll.gameObject.tag != "Powerup") {
            collisionSound.PlaySound();
        }
    }

    /*
	* Changes a stat of this player
	* stat: the stat's name
	* augmenting: true if increasing, false if decreasing
	*/
    public void ChangeStat(int stat, bool augmenting) {
        int change = augmenting ? 1 : -1;
        statLevels[stat] += change;

        canvas.SetStat(stat, statLevels[stat]);

        string sign = augmenting ? "+" : "-";
        ShowMessage(sign + StatConstants.NAMES[stat]);

        powerupSound.PlaySound(augmenting ? powerupClip : powerdownClip);
    }

    public void ShowMessage(string msg, float duration = 3f) {
        canvas.ShowMessage(msg, duration);
    }

    public void StartEnd(bool isWinner, float score) {
        canvas.StartEnd(isWinner, score);
        Destroy(gameObject);
    }

    public void Remove() {
        canvas.MakeBlank();
        Destroy(gameObject);
    }
}
