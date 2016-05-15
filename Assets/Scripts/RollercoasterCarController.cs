using UnityEngine;
using System.Collections;

public class RollercoasterCarController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.localPosition = gameObject.transform.localPosition + Vector3.zero;	
	}

	void FixedUpdate() {
		gameObject.transform.localPosition = gameObject.transform.localPosition + Vector3.zero;	
	}
}
