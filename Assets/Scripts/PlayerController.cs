using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Constants
	private static readonly float[] DEFAULT_STATS = {5, 10, 1.2f}; //Default base stats
	//public int MAX_SPEED;
	//public float ACCELERATION;
	//public float TRACTION;
	public static float INPUT_THRESHOLD = 0.1f; //Button pressed if axis > this threshold
	//public float PERCENT_PER_LEVEL = 0.1f; //How much a stat changes per stat level
	private static readonly float[] BOOST_PER_STAT = {0.05f, 0.1f, 0.1f}; //How much each stat changes per stat level
	private static readonly float ONE_OVER_ROOT_TWO = 1.0f / Mathf.Sqrt(2.0f);

	//Instance variables
	private Rigidbody2D body;
	private Animator animator;
	private float[] baseStats; //Instance variable in case we want to allow multiple vehicles
	private int[] statLevels; 
	//The time at which player started charging/boosting
	private float chargeStart; 
	private float boostStart;
	//For keeping track of boost effect; default to false
	private bool charging;
	private bool boosting;
	//Boost stat for percent per second how quickly your boost runs out

	//Returns whether the axis parameter is big enough to represent a button press
	bool Pressed(float axis) {
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
			//Apply brakes
			body.drag = ComputeStat(StatConstants.BRAKES);
		} else {
			body.drag = 0;
			force *= ComputeStat(StatConstants.ACCELERATION);
			body.AddForce(force);
		}

		//Handle top speed
		Vector2 currVelocity = body.velocity;
		float currMaxSpeed = ComputeStat(StatConstants.SPEED);
		//if (currVelocity.magnitude > currMaxSpeed) {
		float magnitude = currVelocity.magnitude;
		currVelocity.Normalize();
		body.AddForce(-currVelocity * magnitude * magnitude * DragCoefficient());
			//body.AddForce(-currVelocity * magnitude * 1.15f);
			/*
			currVelocity.Normalize();
			body.velocity = currVelocity * currMaxSpeed;
			*/
		//}
	}
	
	// Update is called once per frame
	void Update() {
		Debug.Log(body.velocity);
		//Record when you started charging boost
		if (Input.GetKeyDown("space")) {
			charging = true;
			boosting = false;
			chargeStart = Time.time;
		}
		//Record when you started boosting
		if (Input.GetKeyUp("space")) {
			charging = false;
			boosting = true;
			boostStart = Time.time;
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
