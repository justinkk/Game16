﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public PlayerController[] activePlayers = new PlayerController[4];
    GameObject[] items = new GameObject[6];
    BoxCollider2D[] regions = new BoxCollider2D[6];

    bool timerEnabled = false;
    float gameTimeFloat = 0;
    int gameTime = 0;

    enum State {
        Menu,
        Game,
        Minigame,
        End
    }
    State state = State.Menu;

    const int numMinigames = 1;
    MinigameManager minigameManager = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }
    
    void Start () {

    }

    public void startPlayer(PlayerController player) {
        if (state == State.Menu || state == State.Game) {
            if (state == State.Menu) {
                startGame();
            }
            activePlayers[player.index - 1] = player;
        }
    }

    void startTimer(int seconds) {
        gameTimeFloat = seconds;
        gameTime = seconds;
        timerEnabled = true;
    }

    void Update() {
        if (timerEnabled) {
            gameTimeFloat -= Time.deltaTime;
            if (gameTimeFloat <= gameTime - 1) {
                --gameTime;
                tick();
            }
        }
    }

    // Called once per second when the game is running.
    // Check state for logic specific to game/minigame
    void tick() {
        if (gameTime <= 0) {
            transitionState();
        }

        if (state == State.Minigame && minigameManager != null) {
            // Provide minigame specific updates
            minigameManager.tick();
        }

        if (gameTime % 3 == 0) {
            int index = Random.Range(0, items.Length-1);
            BoxCollider2D region = regions[index];
            int x = (int)Random.Range(0, region.size.x) + (int)region.transform.position.x;
            int y = (int)Random.Range(0, region.size.y) + (int)region.transform.position.y;
            Debug.Log(x);
            Debug.Log(y);
            int itemIndex = Random.Range(0, items.Length-1);
            Instantiate(items[itemIndex], new Vector2(x, y), Quaternion.identity);
        }

        // TODO update timer UI
        print(gameTime);
    }

    void transitionState() {
        switch (state) {
            case State.Game:
                loadMinigame();
                break;
            case State.Minigame:
                loadEnd();
                break;
            case State.End:
                reset();
                break;
        }
    }

    void startGame() {
        if (state == State.Menu) {
            state = State.Game;
            startTimer(5 * 60);
        }
        items[0] = Resources.Load<GameObject>("SpeedDown");
        items[1] = Resources.Load<GameObject>("AccelUp");
        items[2] = Resources.Load<GameObject>("AccelDown");
        items[3] = Resources.Load<GameObject>("SpeedUp");
        items[4] = Resources.Load<GameObject>("TractUp");
        items[5] = Resources.Load<GameObject>("TractDown");
        regions = gameObject.GetComponents<BoxCollider2D>();
        Debug.Log(regions.Length);
    }

    void loadMinigame() {
        timerEnabled = false;

        // Transition to random minigame
        state = State.Minigame;
        int index = (new System.Random()).Next(numMinigames);
        SceneManager.LoadScene("Minigame" + index);
    }

    public void startMinigame(MinigameManager manager) {
        minigameManager = manager;
        startTimer(2 * 60);
    }

    void loadEnd() {
        state = State.End;

        SceneManager.LoadScene("End");
    }

    public void startEnd() {
        startTimer(30);

        // Display winner, scores, etc.
    }

    void reset() {
        instance = new GameManager();
        Destroy(gameObject);

        // TODO reset UI
    }
}
