using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICreator : MonoBehaviour {

    public static UICreator instance = null;

    Image hImage;
    Image vImage;

    Text timer;

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
        addTimer();
	}

	public PlayerCanvas addPlayerCanvas(PlayerController player) {
        return new PlayerCanvas(gameObject.transform, player);
    }

    void addBorders(float thickness) {
        GameObject hBorder = new GameObject();
        GameObject vBorder = new GameObject();
        hImage = hBorder.AddComponent<Image>();
        vImage = vBorder.AddComponent<Image>();
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

    void addTimer() {
        GameObject timerObj = new GameObject();
        timer = timerObj.AddComponent<Text>();
        timer.alignment = TextAnchor.MiddleCenter;
        timer.fontSize = 30;
        timer.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        timer.fontStyle = FontStyle.Bold;
        timer.color = Color.white;
        Outline timerOutline = timerObj.AddComponent<Outline>();
        timerOutline.effectColor = Color.black;

        timer.transform.SetParent(gameObject.transform);
        RectTransform rectTransform = timer.GetComponent<RectTransform>();
        rectTransform.sizeDelta = gameObject.GetComponent<RectTransform>().rect.size;
        rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void setTimer(int time) {
        if (time < 0) {
            timer.text = "";
        } else {
            timer.text = (time / 60) + ":" + (time % 60).ToString("00");
        }
    }

    public void refreshBorders() {
        hImage.transform.SetAsLastSibling();
        vImage.transform.SetAsLastSibling();
        timer.transform.SetAsLastSibling();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Remove() {
        Destroy(gameObject);
    }
}