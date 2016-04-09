using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//"Constants" that can be changed in the editor
	public int MAX_SPEED = 1;
	public float ACCELERATION = 4.0f;
	public float TRACTION = 2.0f;
	public float INPUT_THRESHOLD = 0.3f; //Button pressed if axis > this threshold
	private static float ONE_OVER_ROOT_TWO = 1.0f / Mathf.Sqrt(2.0f);

	//Instance variables and variables that behave like them
	private Rigidbody2D body;
	private Animator animator;

	//Returns whether the axis parameter is big enough to represent a button press
	bool Pressed(float axis) {
		return Mathf.Abs(axis) > INPUT_THRESHOLD;
	}

	//Called on the object's creation
	void Start() {
		body = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponent<Animator>();
	}

	//Called once per physics step
	void FixedUpdate() {
		//Figure out which direction to travel in
		float verticalInput = Input.GetAxis("Vertical"); //TODO: Replace Input.GetAxis with something better,
		float horizontalInput = Input.GetAxis("Horizontal");  // and something that could work with multiple
															  // controllers
		Vector2 force = Vector2.zero;
		if (verticalInput > INPUT_THRESHOLD) {
			force += Vector2.up;
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
			body.drag = TRACTION;
		} else {
			body.drag = 0;
			force *= ACCELERATION;
			body.AddForce(force);
		}

		//Handle top speed
		Vector2 currVelocity = body.velocity;
		if (currVelocity.magnitude > MAX_SPEED) { 
			currVelocity.Normalize();
			body.velocity = currVelocity * MAX_SPEED;
		}
	}
	
	// Update is called once per frame
	void Update() {
	
	}
}
