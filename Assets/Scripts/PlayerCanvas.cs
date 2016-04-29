using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerCanvas {
    float top = 1f;
    float right = 1f;
    float middle = 0.5f;
    float bottom = 0f;
    float left = 0f;

    List<GameObject> menuComponents = new List<GameObject>();

    int playerId;
    float timer = 0;
    Text message;

    public PlayerCanvas(Transform parentTransform, int playerId) {
        this.playerId = playerId;

        GameObject canvasObj = new GameObject("Player Canvas " + playerId);
        Canvas playerCanvas = canvasObj.AddComponent<Canvas>();
        playerCanvas.transform.SetParent(parentTransform);

        RectTransform rectTransform = playerCanvas.GetComponent<RectTransform>();

        switch (playerId) {
            case 1:
                rectTransform.anchorMin = new Vector2(left, middle);
                rectTransform.anchorMax = new Vector2(middle, top);
                break;
            case 2:
                rectTransform.anchorMin = new Vector2(middle, middle);
                rectTransform.anchorMax = new Vector2(right, top);
                break;
            case 3:
                rectTransform.anchorMin = new Vector2(left, bottom);
                rectTransform.anchorMax = new Vector2(middle, middle);
                break;
            case 4:
                rectTransform.anchorMin = new Vector2(middle, bottom);
                rectTransform.anchorMax = new Vector2(right, middle);
                break;
        }

        rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = new Vector2(0f, 0f);
        rectTransform.offsetMax = new Vector2(0f, 0f);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);

        GameObject bg = new GameObject();
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = PlayerController.COLORS[playerId - 1];
        AddObject(bgImage, rectTransform);
        menuComponents.Add(bg);

        GameObject gameTitle = new GameObject("title");
        gameTitle.AddComponent<Text>();
        Text titleText = gameTitle.GetComponent<Text>();
        titleText.text = "Tied Together\n\nPress A to Start";
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontSize = 20;
        titleText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = Color.black;
        AddObject(titleText, rectTransform);
        menuComponents.Add(gameTitle);

        GameObject messageObj = new GameObject("title");
        messageObj.AddComponent<Text>();
        message = messageObj.GetComponent<Text>();
        message.alignment = TextAnchor.UpperCenter;
        message.fontSize = 20;
        message.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        message.fontStyle = FontStyle.Bold;
        message.color = Color.black;
        AddObject(message, rectTransform);
    }

    void AddObject(Component obj, RectTransform parentTransform) {
        obj.transform.SetParent(parentTransform);

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = parentTransform.rect.size;
        rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);

        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void HideMenu() {
        foreach (GameObject o in menuComponents) {
            o.SetActive(false);
        }
    }

    public void ShowMessage(string msg, float duration = 3f) {
        message.text = msg;
        timer = duration;
    }

    public void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                message.text = "";
            }
        }
    }
}
