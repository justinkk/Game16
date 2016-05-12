using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Track {

	GameObject track;
	GameObject previousPiece;
	bool isCurve;
	int direction;
	bool counterClockWise;
	Track prev;
//
//	float width;
//	float height;

	public const int NORTH = 3;
	public const int EAST = 0;
	public const int SOUTH = 1;
	public const int WEST = 2;


	public Track(GameObject track, bool isCurve, Transform parentTransform, Track prev, float rotation, int direction) {
		this.track = track;
		this.isCurve = isCurve;
		this.prev = prev;
		this.direction = direction;

		this.track.transform.SetParent (parentTransform);
		counterClockWise = false;
		if (prev != null && isCurve && (prev.direction + 1) % 4 != this.direction) {
			counterClockWise = true;
			rotation = rotation + 90f;
		}
		int counterClockWisePad = 0;
		if (counterClockWise || (prev != null && prev.counterClockWise)) {
			counterClockWisePad = 2;
		}

		track.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, rotation));
		track.transform.localScale = new Vector3 (2f, 2f, 1f);

		// initial track
		if (prev == null) {
			track.transform.localPosition = new Vector3 (-25f, -15f);
		} else {
			Vector3 prevPosition = prev.track.transform.localPosition;
			float width = track.GetComponent<SpriteRenderer> ().bounds.size.x / 4
				+ prev.track.GetComponent<SpriteRenderer>().bounds.size.x / 4;
			float height = track.GetComponent<SpriteRenderer> ().bounds.size.y / 4
				+ prev.track.GetComponent<SpriteRenderer> ().bounds.size.y / 4;
			
			Vector3 finalPosition; 
			
			if (isCurve) {
				finalPosition = calculatePosition (prevPosition, (direction + 3 + counterClockWisePad) % 4, width, height);
				if (!prev.isCurve) {
					finalPosition = adjustPosition (finalPosition, direction, track, prev.track);
				} 
			}
			else {
				finalPosition = calculatePosition (prevPosition, direction, width, height);

				if (prev.isCurve) {
					finalPosition = adjustPosition (finalPosition, (direction + 3 + counterClockWisePad) % 4, prev.track, track);
				}

			}

			track.transform.localPosition = finalPosition;

		}
	}

	private Vector3 adjustPosition(Vector3 prevPosition, int direction, GameObject curve, GameObject line) {
		float widthAdjustment = (curve.GetComponent<SpriteRenderer> ().bounds.size.x 
			- line.GetComponent<SpriteRenderer>().bounds.size.x ) / 4;
		float heightAdjustment = (curve.GetComponent<SpriteRenderer> ().bounds.size.y 
			- line.GetComponent<SpriteRenderer> ().bounds.size.y) / 4;
		Debug.LogWarning (widthAdjustment);
		switch (direction) {
		case EAST:
			Debug.LogWarning ("HEL:");
			prevPosition.x = prevPosition.x + widthAdjustment;
			break;
		case WEST:
			prevPosition.x = prevPosition.x - widthAdjustment;
			break;
		case NORTH:
			prevPosition.y = prevPosition.y + heightAdjustment;
			break;
		case SOUTH:
			prevPosition.y = prevPosition.y - heightAdjustment;
			break; 
		}
		return prevPosition;
	}


	private Vector3 calculatePosition(Vector3 prevPosition, int direction, float width, float height) {

		switch (direction) {
		case EAST:
			prevPosition.x = prevPosition.x + width;
			break;
		case WEST:
			prevPosition.x = prevPosition.x - width;
			break;
		case NORTH:
			prevPosition.y = prevPosition.y + height;
			break;
		case SOUTH:
			prevPosition.y = prevPosition.y - height;
			break; 
		}


		return prevPosition;
	}



}