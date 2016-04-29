using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICreator : MonoBehaviour {

    public static UICreator instance = null;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        addBorders(0.05f);
	}

	public PlayerCanvas addPlayerCanvas(int playerId) {
        return new PlayerCanvas(gameObject.transform, playerId);
    }

    void addBorders(float thickness) {
        GameObject hBorder = new GameObject();
        GameObject vBorder = new GameObject();
        Image hImage = hBorder.AddComponent<Image>();
        Image vImage = vBorder.AddComponent<Image>();
        hImage.color = Color.black;
        vImage.color = Color.black;
        hImage.transform.SetParent(gameObject.transform);
        vImage.transform.SetParent(gameObject.transform);
        RectTransform hTransform = hImage.GetComponent<RectTransform>();
        RectTransform vTransform = vImage.GetComponent<RectTransform>();
        hTransform.localPosition = new Vector2(0f, 0f);
        vTransform.localPosition = new Vector2(0f, 0f);

        hTransform.anchorMin = new Vector2(0f, 0.5f);
        hTransform.anchorMax = new Vector2(1f, 0.5f);
        hTransform.localScale = new Vector3(1f, thickness, 1f);

        vTransform.anchorMin = new Vector2(0.5f, 0f);
        vTransform.anchorMax = new Vector2(0.5f, 1f);
        vTransform.localScale = new Vector3(thickness, 1f, 1f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}