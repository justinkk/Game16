using UnityEngine;
using System.Collections;

public class UICreator : MonoBehaviour {

	// Use this for initialization
	void Start () {

		for (int i = 0; i < 4; i++) {
			addPlayerCanvas (i);
		}
	}

	void addPlayerCanvas(int playerId) {
		GameObject playerCanvasModel = Resources.Load ("PlayerCanvas") as GameObject;
		GameObject playerCanvas = Instantiate (playerCanvasModel);
		playerCanvas.transform.parent = gameObject.transform;

		RectTransform rectTransform = playerCanvas.GetComponent<RectTransform> ();

		float top = 1f;
		float right = 1f;
		float middle = 0.5f;
		float bottom = 0f;
		float left = 0f;
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

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

//
//
//using System.Collections;
//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;
//
//
//public class UICreator : MonoBehaviour {
//
//	private GameObject overAllCanvas;
//	private GameObject playerCanvas;
//
//	void Start () {
//
//		overAllCanvas = gameObject;
//
//		playerCanvas = new GameObject ();
//		playerCanvas.AddComponent<Canvas> ();
//		playerCanvas.name = "Player Canvas";
//
//		playerCanvas.transform.parent = overAllCanvas.transform;
//		//		playerCanvas.AddComponent<RectTransform> ();
//
//
//		Canvas canvas = playerCanvas.GetComponent<Canvas> ();
//		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
//
//		RectTransform rectTransform = playerCanvas.GetComponent<RectTransform> ();
//		rectTransform.anchoredPosition = new Vector2 (0.5f, 0.5f);
//		rectTransform.anchorMax = new Vector2 (0, 1);
//		rectTransform.anchorMin = new Vector2 (0, 1);
//		rectTransform.localPosition = new Vector2 (200, -150);
//		rectTransform.sizeDelta = new Vector2 (400, 300);
//
//
//		GameObject background = new GameObject ();
//		background.transform.parent = playerCanvas.transform;
//		background.AddComponent<Text> ();
//		Text text = background.GetComponent<Text> ();
//		text.text = "HELLO WORLD";
//		background.AddComponent<RectTransform> ();
//		RectTransform rt = background.GetComponent<RectTransform> ();
//		rt.
//
//		// create game object and child object
//		//		overAllCanvas = gameObject;
//		//		childGO = new GameObject ();
//		//
//		//		// give them names for fun
//		//		childGO.name = "ChildObject";
//		//
//		//		// set the child object as a child of the parent
//		//		childGO.transform.parent = overAllCanvas.transform;
//		//
//		//		// add a canvas to the parent
//		//		childGO.AddComponent<Canvas> ();
//		//		// add a recttransform to the child
//		//		childGO.AddComponent<RectTransform> ();
//		//
//		//		// make a reference to the parent canvas and use the ref to set its properties
//		//		Canvas myCanvas = childGO.GetComponent<Canvas> ();
//		//
//		//		// add a text component to the child
//		//		childGO.AddComponent<Text> ();
//		//		childGO.AddComponent<Image>();
//		//		// make a reference to the child rect transform and set its values
//		//		RectTransform childRectTransform = childGO.GetComponent<RectTransform> ();
//		//		RectTransform parentRectTransform = overAllCanvas.GetComponent<RectTransform> ();
//		//
//		//		// set child anchors for resizing behaviour
//		//		childRectTransform.anchoredPosition3D = new Vector3(0f,0f,0f);
//		//		childRectTransform.sizeDelta = new Vector2 (0f, 0f);
//		//		childRectTransform.anchorMin = new Vector2 (0f,0f);
//		//		childRectTransform.anchorMax = new Vector2 (1f, 1f);
//		//
//		//		// set text font type and material at runtime from font stored in Resources folder
//		//		Text textComponent = childGO.GetComponent<Text> ();
//		//		Image image = childGO.GetComponent<Image> ();
//		//		image.sprite = Resources.Load<Sprite> ("border.png");
//		//
//		//		// set the font text
//		//		textComponent.text = "Hello World";
//
//
//	}
//}