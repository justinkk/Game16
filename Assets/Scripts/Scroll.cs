using UnityEngine;
using System.Collections;

public class Scroll : MonoBehaviour {

    public float x_speed = 1;
    public float y_speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 offset = new Vector2(Time.time * x_speed, Time.time * y_speed);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
	}
}
