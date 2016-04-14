using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//"Constants" that can be changed in the editor
	public int MAX_SPEED;
	public float ACCELERATION;
	public float TRACTION;
	public float INPUT_THRESHOLD = 0.3f; //Button pressed if axis > this threshold
	public float PERCENT_PER_LEVEL = 0.1f; //How much a stat changes per stat level
	private static float ONE_OVER_ROOT_TWO = 1.0f / Mathf.Sqrt(2.0f);

	//Instance variables and variables that behave like them
	private Rigidbody2D body;
	private Animator animator;
	private int[] statLevels;
	/*private int speedLevel;
	private int accelerationLevel;
	private int tractionLevel;*/

	//Returns whether the axis parameter is big enough to represent a button press
	bool Pressed(float axis) {
		return Mathf.Abs(axis) > INPUT_THRESHOLD;
	}

	//Gives current value of a stat, given base value and current level
	private float ComputeStat(float baseStat, int statLevel) {
		if (statLevel > 0) 
			return baseStat * (1.0f + PERCENT_PER_LEVEL * statLevel);
		else if (statLevel < 0)
			return baseStat * (Mathf.Pow(1.0f - PERCENT_PER_LEVEL, -statLevel));
		else
			return baseStat;
	}

	//Called on the object's creation
	void Start() {
		body = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponent<Animator>();

		//Inintialize stat levels, defaulting to 0
		statLevels = new int[StatConstants.NUM_STATS];
		/*speedLevel = 0;
		accelerationLevel = 0;
		tractionLevel = 0;*/
	}

	//Called once per physics step
	void FixedUpdate() {
		//Figure out which direction to travel in
		float verticalInput = Input.GetAxis("Vertical"); //TODO: Replace Input.GetAxis with something better,
		float horizontalInput = Input.GetAxis("Horizontal");  // and something that could work with multiple
															  // controllers
		Vector2 force = Vector2.zero;
		if (verticalInput > INPUT_THRESHOLD) {
			force += Vector2.up;			   //TODO: consider analog input?
			animator.SetBool("GoingUp", true); //TODO: this probably belongs in Update() instead of FixedUpdate()
		} else if (verticalInput < -INPUT_THRESHOLD) { // but it's a terrible animation anyway
			force += Vector2.down;
			animator.SetBool("GoingUp", false);
		}
		if (horizontalInput > INPUT_THRESHOLD) {
			force += Vector2.right;
			animator.SetBool("GoingRight", true);
		} else if (horizontalInput < -INPUT_THRESHOLD) {
			force += Vector2.left;
			animator.SetBool("GoingRight", false);
		}

		if (Pressed(verticalInput) && Pressed(horizontalInput)) {
			//Diagonal: normalize the movement speed
			force *= ONE_OVER_ROOT_TWO;	
		} 

		animator.SetBool("VerticalFasterThanHorizontal", Mathf.Abs(force.y) > Mathf.Abs(force.x));
		
		//Apply force
		if (force == Vector2.zero) {
			//Apply traction
			body.drag = ComputeStat(TRACTION, statLevels[StatConstants.BRAKES]);
		} else {
			body.drag = 0;
			force *= ComputeStat(ACCELERATION, statLevels[StatConstants.ACCELERATION]);
			body.AddForce(force);
		}

		//Handle top speed
		Vector2 currVelocity = body.velocity;
		float currMaxSpeed = ComputeStat(MAX_SPEED, statLevels[StatConstants.SPEED]);
		if (currVelocity.magnitude > currMaxSpeed) { 
			currVelocity.Normalize();
			body.velocity = currVelocity * currMaxSpeed;
		}
	}
	
	// Update is called once per frame
	void Update() {
		//int val = (int)Mathf.Floor(Random.Range(-10.0f, 10.0f));
		//Debug.Log("level: " + val + ", stat computed: " + ComputeStat(MAX_SPEED, val));
		print("Speed level" + statLevels[StatConstants.SPEED]);
		print("Accel level" + statLevels[StatConstants.ACCELERATION]);
		print("Brake level" + statLevels[StatConstants.BRAKES]);
	}

	/*
	* Changes a stat of this player
	* stat: the stat's name
	* augmenting: true if increasing, false if decreasing
	*/
	public void ChangeStat(int stat, bool augmenting) {
		int change = augmenting ? 1 : -1;
		statLevels[stat] += change;
		//TODO: Make this good style instead of terrible
		/*
		int change; 
		if (augmenting)
			change = 1;
		else
			change = -1;

		if (stat == "Speed") {
			speedLevel += change;
		} else if (stat == "Acceleration") {
			accelerationLevel += change;
		} else if (stat == "Traction") {
			tractionLevel += change;
		}
		*/
	}
}
