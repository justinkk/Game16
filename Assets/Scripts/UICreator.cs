using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICreator : MonoBehaviour {
	float top = 1f;
	float right = 1f;
	float middle = 0.5f;
	float bottom = 0f;
	float left = 0f;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < 4; i++) {
			addPlayerCanvas (i);
		}
	}

	void addPlayerCanvas(int playerId) {
		GameObject playerCanvasModel = Resources.Load ("PlayerCanvas") as GameObject;
		GameObject playerCanvas = Instantiate (playerCanvasModel);
		playerCanvas.name = "Player Canvas " + playerId;
		playerCanvas.transform.SetParent(gameObject.transform);

		RectTransform rectTransform = playerCanvas.GetComponent<RectTransform> ();


		if (playerId % 4 == 0) {
			rectTransform.anchorMin = new Vector2 (left, middle);
			rectTransform.anchorMax = new Vector2 (middle, top);
		} else if (playerId % 4 == 1) {
			rectTransform.anchorMin = new Vector2 (middle, middle);
			rectTransform.anchorMax = new Vector2 (right, top);
		} else if (playerId % 4 == 2) {
			rectTransform.anchorMin = new Vector2 (left, bottom);
			rectTransform.anchorMax = new Vector2 (middle, middle);
		} else if (playerId % 4 == 3) {
			rectTransform.anchorMin = new Vector2 (middle, bottom);
			rectTransform.anchorMax = new Vector2 (right, middle);
		}

		rectTransform.anchoredPosition = new Vector2 (0.5f, 0.5f);
		rectTransform.offsetMin = new Vector2 (0f, 0f);
		rectTransform.offsetMax = new Vector2 (0f, 0f);
		rectTransform.localScale = new Vector3 (1f, 1f, 1f);

		GameObject gameTitle = new GameObject ();
		gameTitle.name = "title";
		gameTitle.AddComponent<Text> ();
		Text titleText = gameTitle.GetComponent<Text> ();
		titleText.text = "Tied Down";
		titleText.fontSize = 20;
		titleText.font = Resources.GetBuiltinResource (typeof(Font), "Arial.ttf") as Font;
		titleText.fontStyle = FontStyle.Bold;
		titleText.color = Color.red;
		addObjectToPlayerCanvas (gameTitle, playerCanvas);

		GameObject buttonObj = playerCanvas.transform.Find ("button").gameObject;
		Button button = buttonObj.GetComponent<Button> ();
		button.interactable = true;
		print (playerId);
		button.onClick.AddListener(delegate {
			hidePlayerCanvasComponents(playerCanvas);
		});

	}


	void hidePlayerCanvasComponents(GameObject playerCanvas) {
		print ("HIDE" + playerCanvas.name);
		GameObject button = playerCanvas.transform.Find ("button").gameObject;
		button.SetActive (false);

		GameObject title = playerCanvas.transform.Find ("title").gameObject;
		title.SetActive (false);

		GameObject background = playerCanvas.transform.Find ("background").gameObject;
		background.SetActive (false);

	}

	void addObjectToPlayerCanvas(GameObject obj, GameObject playerCanvas) {
		obj.transform.SetParent(playerCanvas.transform);

		RectTransform rectTransform = obj.GetComponent<RectTransform> ();
		rectTransform.localPosition = new Vector2 (0f, 50f);
		rectTransform.anchorMax = new Vector2 (middle, middle);
		rectTransform.anchorMin = new Vector2 (middle, middle);
		rectTransform.anchoredPosition = new Vector2 (middle, middle);

		rectTransform.localScale = new Vector3 (1f, 1f, 1f);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}