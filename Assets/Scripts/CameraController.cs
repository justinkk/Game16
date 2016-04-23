using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
   private Transform parent;

	// Use this for initialization
	void Start () {
      parent = transform.parent;     
	}
	
	// Update is called once per frame
	void Update () {
      //Keep camera aligned to world
      transform.eulerAngles = Vector3.zero;
	}
}
