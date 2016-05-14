using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerCanvas {
    float top = 1f;
    float right = 1f;
    float middle = 0.5f;
    float bottom = 0f;
    float left = 0f;

    const float ARROW_SIZE = 20;

    List<GameObject> menuComponents = new List<GameObject>();
    List<GameObject> gameComponents = new List<GameObject>();
    List<GameObject> blankComponents = new List<GameObject>();
    List<GameObject> endComponents = new List<GameObject>();
    GameObject[] playerArrows = new GameObject[4];

    PlayerController player;
    float timer = 0;

    Canvas canvas;
    Text title;
    Text message;

    public PlayerCanvas(Transform parentTransform, PlayerController player) {
        this.player = player;

        GameObject canvasObj = new GameObject("Player Canvas " + player.index);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.transform.SetParent(parentTransform);

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();

        switch (player.index) {
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
        bgImage.color = player.color;
        AddObject(bgImage, rectTransform);
        menuComponents.Add(bg);
        blankComponents.Add(bg);
        endComponents.Add(bg);

        GameObject gameTitle = new GameObject();
        title = gameTitle.AddComponent<Text>();
        title.text = "Tied Together\n\nPress A to Start";
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = 20;
        title.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        title.fontStyle = FontStyle.Bold;
        title.color = Color.black;
        AddObject(title, rectTransform);
        menuComponents.Add(gameTitle);
        endComponents.Add(gameTitle);

        GameObject messageObj = new GameObject();
        message = messageObj.AddComponent<Text>();
        message.alignment = TextAnchor.UpperCenter;
        message.fontSize = 20;
        message.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        message.fontStyle = FontStyle.Bold;
        message.color = Color.black;
        Outline messageOutline = messageObj.AddComponent<Outline>();
        messageOutline.effectColor = Color.white;
        AddObject(message, rectTransform);
    }

    void AddObject(Component obj, RectTransform parentTransform) {
        obj.transform.SetParent(parentTransform);

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = parentTransform.rect.size;
        rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);

        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void StartGame() {
        foreach (GameObject o in menuComponents) {
            o.SetActive(false);
        }
    }

    public void StartMinigame() {
    }

    public void MakeBlank() {
        foreach (GameObject o in menuComponents) {
            o.SetActive(false);
        }

        foreach (GameObject o in blankComponents) {
            o.SetActive(true);
        }
    }

    public void StartEnd(bool isWinner, float score) {
        foreach (GameObject o in gameComponents) {
            o.SetActive(false);
        }

        message.text = "";
        title.text = (isWinner ? "You Won!\n\n" : "") + "Score: " + score.ToString("0");
        foreach (GameObject o in endComponents) {
            o.SetActive(true);
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

        if (player.isPlaying) {
            DrawPlayerArrows();
        }
    }

    void DrawPlayerArrows() {
        Vector3 position = player.transform.position;
        
        for (int i = 0; i < 4; ++i) {
            if (i == player.index - 1)
                continue;

            PlayerController otherPlayer = GameManager.instance.players[i];
            if (otherPlayer != null && otherPlayer.isPlaying) {
                GameObject arrowObj = playerArrows[i];
                if (arrowObj == null) {
                    // Create new player arrow
                    arrowObj = new GameObject();
                    Image arrow = arrowObj.AddComponent<Image>();
                    arrow.sprite = Resources.Load<Sprite>("player_arrow");
                    arrow.color = otherPlayer.color;
                    AddObject(arrow, canvas.GetComponent<RectTransform>());
                    
                    arrow.transform.SetParent(canvas.GetComponent<RectTransform>());
                    RectTransform rectTransform = arrow.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(ARROW_SIZE, ARROW_SIZE);
                    rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
                    rectTransform.localScale = new Vector3(1f, 1f, 1f);
                    playerArrows[i] = arrowObj;
                    gameComponents.Add(arrowObj);
                }

                Vector3 otherPosition = otherPlayer.transform.position;
                if (IsPointOnScreen(otherPosition)) {
                    // Hide arrow
                    arrowObj.SetActive(false);
                } else {
                    // Show and update arrow
                    arrowObj.SetActive(true);
                    float dy = otherPosition.y - position.y;
                    float dx = otherPosition.x - position.x;
                    float angle = Mathf.Atan2(dy, dx) * 180f / Mathf.PI;
                    arrowObj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    Vector2 size = canvas.GetComponent<RectTransform>().rect.size;
                    float x_max = size.x / 2 - ARROW_SIZE;
                    float y_max = size.y / 2 - ARROW_SIZE;
                    float x_proj = dx * y_max / Mathf.Abs(dy);
                    float y_proj = dy * x_max / Mathf.Abs(dx);
                    if (Mathf.Abs(x_proj) < x_max) {
                        arrowObj.GetComponent<Image>().GetComponent<RectTransform>().localPosition = new Vector3(x_proj, y_max * Mathf.Sign(dy), 10);
                    } else {
                        arrowObj.GetComponent<Image>().GetComponent<RectTransform>().localPosition = new Vector3(x_max * Mathf.Sign(dx), y_proj, 10);
                    }
                }
            }
        }
    }

    bool IsPointOnScreen(Vector3 worldPoint) {
        Vector3 point = player.camera.WorldToViewportPoint(worldPoint);
        if (point.x > 0 && point.x < 1 && point.y > 0 && point.y < 1) {
            return true;
        }
        return false;
    }
}
