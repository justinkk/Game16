using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Constants

	// Set in editor. Determines which controller to use.
	public int index = 0;

   private static readonly float[] DEFAULT_STATS = {5, 10, 2, 30}; //Default base stats
	public const float INPUT_THRESHOLD_LOW = 0.1f; //In deadzone if magnitude < this threshold
	public const float VELOCITY_THRESHOLD_LOW = 0.3f; //In deadzone if magnitude < this threshold
	public const float INPUT_THRESHOLD_HIGH = 0.8f; //Key pressed if axis > this threshold
	private static readonly float[] BOOST_PER_STAT = {0.05f, 0.08f, 0.1f, 0.1f}; //How much each stat changes per stat level
	private static readonly float ONE_OVER_ROOT_TWO = 1.0f / Mathf.Sqrt(2.0f);

	//Brakes stat computes drag amount
	//Multiply by this to get %charged per second
	public const float BRAKES_TO_CHARGE_SPEED = 0.8f;
	//How much torque should be applied, based on degrees you're off
	public const float DEGREES_TO_TORQUE = 0.03f;
	public const float MAX_ANGULAR_VELOCITY = 360;

	//Instance variables
	private Rigidbody2D body;
	private Animator animator;
	private float[] baseStats; //Instance variable in case we want to allow multiple vehicles
	private int[] statLevels;

	private bool charging = false;
	private float chargeStart;

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

	//Returns whether the axis parameter is big enough to represent a button press
	private bool KeyPressed(float axis) {
		return Mathf.Abs(axis) > INPUT_THRESHOLD_HIGH;
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
		Vector2 input = new Vector2(Input.GetAxisRaw("X" + index), Input.GetAxisRaw("Y" + index));
		if (index == 1) {
      	input.x += Input.GetAxisRaw("X1 Alt");
      	input.y += Input.GetAxisRaw("Y1 Alt");
   	}

		if (input.magnitude < INPUT_THRESHOLD_LOW)
			input = Vector2.zero;

		if (KeyPressed(input.x) && KeyPressed(input.y)) {
			//Diagonal: normalize the movement speed
			input *= ONE_OVER_ROOT_TWO;	//TODO: Only applies to keyboard movement, make sure you're on keyboard
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
		

		if (desiredDirection != Vector2.zero) {
			float angle = Vector2.Angle(Vector2.up, desiredDirection);
			if (desiredDirection.x > 0)
				angle = 360 - angle;
			float angleDifference = angle - transform.eulerAngles.z;
			if (angleDifference < -180)
				angleDifference += 360;
			else if (angleDifference > 180)
				angleDifference -= 360;

			print(desiredDirection + ", " + angle + ", " + angleDifference);

			body.AddTorque(angleDifference * DEGREES_TO_TORQUE);
			body.angularVelocity = Mathf.Clamp(body.angularVelocity, -MAX_ANGULAR_VELOCITY, MAX_ANGULAR_VELOCITY);
			//transform.eulerAngles = new Vector3(0, 0, angle);
		}
	}

	//Called on the object's creation
	void Start() {
		body = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponent<Animator>();

		//Inintialize stat levels, defaulting to 0
		statLevels = new int[StatConstants.NUM_STATS];
		//Initiate base stats
		baseStats = (float[])DEFAULT_STATS.Clone();
   }

	//Called once per physics step
	void FixedUpdate() {
		Vector2 force = InputVector();

		//Apply force
		if (force != Vector2.zero) {
			body.drag = 0;
			force *= ComputeStat(StatConstants.ACCELERATION) * (1.0f - ChargePercent());
			body.AddForce(force);
		}
		if (force == Vector2.zero || charging) {
			//Apply brakes with linear drag
			body.drag = ComputeStat(StatConstants.BRAKES);
		}

		//Handle top speed with quadratic drag
		Vector2 currVelocity = body.velocity;
		float magnitude = currVelocity.magnitude;
		currVelocity.Normalize();
		body.AddForce(-currVelocity * magnitude * magnitude * DragCoefficient());

		TurnCharacter();
	}
	
	// Update is called once per frame
	void Update() {
		//Record when you started charging boost
		if (Input.GetKeyDown("space")) {
			charging = true;
			chargeStart = Time.time;
		}
		//Apply a boost
		if (Input.GetKeyUp("space")) {
			body.AddForce(InputVector() * ChargePercent() * ComputeStat(StatConstants.BOOST), ForceMode2D.Impulse);
			charging = false;
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
