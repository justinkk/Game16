using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		gameObject.transform.localPosition = new Vector3 (0, 0, 0);
		gameObject.transform.localScale = new Vector3 (2f, 2f, 1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

