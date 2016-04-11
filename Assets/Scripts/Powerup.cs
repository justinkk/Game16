using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	public string type;
	public bool augmenting;

	//If a player enters my hitbox, delete me
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.GetComponent<PlayerController>().ChangeStat(type, augmenting);
			Destroy(gameObject);
		}
	}
}
