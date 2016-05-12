using UnityEngine;
using System.Collections;

public class RollerCoasterManager : MonoBehaviour
{
	GameObject trackLine;
	GameObject trackCurve;

	// Use this for initialization
	void Start ()
	{
		trackLine = Resources.Load ("Track") as GameObject;
		trackCurve = Resources.Load ("TrackCurve") as GameObject;

		createTrack ();
	}


	private void createTrack() {


		GameObject trackObject = Instantiate (trackLine);
		Track piece = new Track (trackObject, false, gameObject.transform, null, 90f, -1);
		piece = layTrackLine (32, piece, Track.EAST);
		piece = layTurn (piece, Track.SOUTH);
		piece = layTrackLine (5, piece, Track.SOUTH);
		piece = layTurn (piece, Track.WEST);
		piece = layTrackLine (8, piece, Track.WEST);
		piece = layTurn (piece, Track.NORTH);
		piece = layTrackLine (3, piece, Track.NORTH);
		piece = layTurn (piece, Track.EAST);
		piece = layTrackLine (6, piece, Track.EAST);
		piece = layTurn (piece, Track.SOUTH);
		piece = layTrackLine (1, piece, Track.SOUTH);

		piece = layTurn (piece, Track.WEST);
		piece = layTrackLine (12, piece, Track.WEST);

		piece = layTurn (piece, Track.NORTH);
		piece = layTrackLine (1, piece, Track.NORTH);

		piece = layTurn (piece, Track.WEST);
		piece = layTrackLine (4, piece, Track.WEST);

		piece = layTurn (piece, Track.SOUTH);
		piece = layTrackLine (3, piece, Track.SOUTH);

		piece = layTurn (piece, Track.WEST);
		piece = layTrackLine (9, piece, Track.WEST);

		piece = layTurn (piece, Track.NORTH);
		piece = layTrackLine (5, piece, Track.NORTH);

		piece = layTurn (piece, Track.EAST);
	}

	private Track layTrackLine(int num, Track root, int dir) {
		float rotation = 0f;
		if (dir == Track.WEST || dir == Track.EAST) {
			rotation = 90f;
		}

		for (int i = 0; i < num; i++) {
			GameObject trackObject = Instantiate (trackLine);
			Track nextPiece = new Track (trackObject, false, gameObject.transform, root, rotation, dir);
			root = nextPiece;
		}
		return root;
	}

	private Track layTurn(Track root, int dir) {
		GameObject trackCurveObject = Instantiate (trackCurve);
		float rotation = 90f * (4 - dir);
		Track nextPiece = new Track (trackCurveObject, true, gameObject.transform, root, rotation, dir);
		return nextPiece;
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}

