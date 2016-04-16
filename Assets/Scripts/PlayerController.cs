using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Constants
	private static readonly float[] DEFAULT_STATS = {5, 10, 1.2f, 40}; //Default base stats
	public const float INPUT_THRESHOLD = 0.1f; //Button pressed if axis > this threshold
	private static readonly float[] BOOST_PER_STAT = {0.05f, 0.1f, 0.1f, 0.1f}; //How much each stat changes per stat level
	private static readonly float ONE_OVER_ROOT_TWO = 1.0f / Mathf.Sqrt(2.0f);

	//Brakes stat computes drag amount
	//Multiply by this to get %charged per second
	public const float BRAKES_TO_CHARGE_SPEED = 0.8f;

	//Instance variables
	private Rigidbody2D body;
	private Animator animator;
	private float[] baseStats; //Instance variable in case we want to allow multiple vehicles
	private int[] statLevels; 
	//The time at which player started charging/boosting
	private float chargeStart; 
	//private float boostStart;
	//For keeping track of boost effect; default to false
	private bool charging;
	//private bool boosting;
	
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
	private bool Pressed(float axis) {
		return Mathf.Abs(axis) > INPUT_THRESHOLD;
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
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (input.magnitude < INPUT_THRESHOLD)
			input = Vector2.zero;

		if (Pressed(input.x) && Pressed(input.y)) {
			//Diagonal: normalize the movement speed
			input *= ONE_OVER_ROOT_TWO;	//TODO: Only applies to keyboard movement, make sure you're on keyboard
		} 

		return input;
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
		//Figure out which direction to travel in
		float verticalInput = Input.GetAxisRaw("Vertical"); //TODO: Replace Input.GetAxisRaw with something better,
		float horizontalInput = Input.GetAxisRaw("Horizontal");  // and something that could work with multiple
															  // controllers
		Vector2 force = InputVector();
		//Apply force
		if (force == Vector2.zero) {
			//Apply brakes with linear drag
			body.drag = ComputeStat(StatConstants.BRAKES);
		} else {
			body.drag = 0;
			force *= ComputeStat(StatConstants.ACCELERATION) * (1.0f - ChargePercent());
			body.AddForce(force);
		}

		//Handle top speed with quadratic drag
		Vector2 currVelocity = body.velocity;
		float magnitude = currVelocity.magnitude;
		currVelocity.Normalize();
		body.AddForce(-currVelocity * magnitude * magnitude * DragCoefficient());
	}
	
	// Update is called once per frame
	void Update() {
		//Record when you started charging boost
		if (Input.GetKeyDown("space")) {
			charging = true;
			//boosting = false;
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
