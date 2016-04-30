using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Constants
   private static readonly float[] DEFAULT_STATS = {5, 10, 1.5f, 30}; //Default base stats
	public const float INPUT_THRESHOLD_LOW = 0.1f; //In deadzone if magnitude < this threshold
	public const float VELOCITY_THRESHOLD_LOW = 0.3f; //In deadzone if magnitude < this threshold
	private static readonly float[] BOOST_PER_STAT = {0.05f, 0.08f, 0.1f, 0.1f}; //How much each stat changes per stat level
   private static readonly Color[] COLORS = {
      new Color(1f, 0.6f, 0.6f, 1f),
      new Color(1f, 1.0f, 0.6f, 1f),
      new Color(0.6f, 0.8f, 1.0f, 1f),
      new Color(0.8f, 1.0f, 0.6f, 1f)
   };

	private const int DEGREES_IN_CIRCLE = 360;

	//Brakes stat computes drag amount
	//Multiply by this to get %charged per second
	public const float BRAKES_TO_CHARGE_SPEED = 0.8f;
	//Multiplier for how long your boost lasts depending on boost stat
	public const float BOOST_TO_BOOST_TIME = 0.015f;
	//How much angular velocity should be applied, based on degrees you're off
	public static readonly float OFFSET_TO_ANGULAR_VELOCITY = 4 * DEGREES_IN_CIRCLE;

   //Instance variables
   private bool isPlaying = false;
   public Color color;
   private PlayerCanvas canvas;
   public new Camera camera;
	private Rigidbody2D body;
   private RopeController playerRope = null;
	//private Animator animator;
	private float[] baseStats; //Instance variable in case we want to allow multiple vehicles
	private int[] statLevels;

	private bool charging = false;
	private float chargeStart;
	private float boostEnd = -100; //Boost end time. Initialized to negative so you're not starting in a boost

	private ParticleSystem exhaustParticles;
   private const float MAX_EXHAUST = 50;
	// Set in editor. Determines which controller to use.
	public int index = 0;
	// Set in editor. The rope's prefab.
	public RopeController rope;

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

   /**
    *
    */
   public void SetPlayerRope(RopeController rope) {
      playerRope = rope;
   }

	//Gives current value of a stat, given base value and current level
	private float ComputeStat(int statNum) {
		int statLevel = statLevels[statNum];
		float baseStat = baseStats[statNum];
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
	 * Tell the character to charge
	 */
	private void Charge() {
      //canvas.ShowMessage("CHARGING"); // probably remove this later
		charging = true;
		chargeStart = Time.time;
	}

	/**
	 * Tell the character to boost
	 */
	private void Boost() {
      float chargePercent = ChargePercent();
		float boostAmount = chargePercent * chargePercent * ComputeStat(StatConstants.BOOST);
		body.AddForce(InputVector() * boostAmount * body.mass, ForceMode2D.Impulse);
		boostEnd = Time.time + boostAmount * BOOST_TO_BOOST_TIME;
		charging = false;
		exhaustParticles.emissionRate = 0;

	}

   void Awake() {
      DontDestroyOnLoad(gameObject);
   }

   //Called on the object's creation
   void Start() {
      color = COLORS[index - 1];
      canvas = UICreator.instance.addPlayerCanvas(this);
      camera = GetComponentInChildren<Camera>();

      body = gameObject.GetComponent<Rigidbody2D>();

		GameObject smoke = Resources.Load ("Smoke") as GameObject;
		GameObject playerExhaust = Instantiate (smoke);
		playerExhaust.transform.SetParent (gameObject.transform);
		playerExhaust.transform.localPosition = new Vector3 (0f, 0f, 0f);
		playerExhaust.transform.localRotation = Quaternion.Euler (90f, 0f, 0f);

		exhaustParticles = playerExhaust.GetComponent<ParticleSystem> ();
		exhaustParticles.simulationSpace = ParticleSystemSimulationSpace.World;
		//animator = gameObject.GetComponent<Animator>();

		//Inintialize stat levels, defaulting to 0
		statLevels = new int[StatConstants.NUM_STATS];
		//Initiate base stats
		baseStats = (float[])DEFAULT_STATS.Clone();

    // FOR TESTING ONLY: (otherwise startPlayer is called in StartPlaying())
    //GameManager.instance.startPlayer(this);
    }

    void StartPlaying() {
      isPlaying = true;
      canvas.StartGame();
      GameManager.instance.startPlayer(this);
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
            StartPlaying();
         } else {
            Charge();
         }
		}

      if (charging) {
         exhaustParticles.emissionRate = MAX_EXHAUST * ChargePercent();
      }

		if ((index == 1 && Input.GetKeyUp("space")) || Input.GetKeyUp(KeyCodes.GetA(index)) || Input.GetKeyUp(KeyCodes.GetZ(index))
        && charging) {//Make sure you aren't being tricky with buttons or burned out if we implement that
			Boost();
		}

      canvas.Update();
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
         PlayerController otherPlayer = coll.gameObject.GetComponent<PlayerController>();
         if ((IsBoosting() || otherPlayer.IsBoosting())
               && otherPlayer.index > index) {
   			//TODO: Rope length
   			//TODO: Make rope only when someone boosting whom you're not attached to
   			//TODO: Drop powerup(s) if you're hit by someone you're not attached to
   			//TODO: Replace rope if there's an old rope
            GameObject ropePrefab = Resources.Load("Rope") as GameObject;
            Vector3 location = (transform.position + otherPlayer.transform.position) / 2;
            print(location);
            RopeController rope = Instantiate(ropePrefab).GetComponent<RopeController>();
            print(rope.transform.position);
            SetPlayerRope(rope);
            otherPlayer.SetPlayerRope(rope);
   			playerRope.GetComponent<RopeController>().MakeRope(transform, coll.transform, 0.2f, 8, location);
         }
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
	}
}
