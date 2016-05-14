﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public PlayerController[] players = new PlayerController[4];
    GameObject[] items = new GameObject[6];
    BoxCollider2D[] regions = new BoxCollider2D[7];

    bool timerEnabled = false;
    float gameTimeFloat = 0;
    int gameTime = 0;

    const int GAME_TIME = 5 * 60;
    const int GAME_WARNING = 10;
    const int MINIGAME_TIME = 2 * 60;
    const int END_TIME = 30;

    public enum State {
        Menu,
        Game,
        Minigame,
        End
    }
    public State state = State.Menu;

    static readonly string[] MINIGAME_NAMES = { "Bumper Cars" };
    int minigameIndex = 0;
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

    public void CreatePlayer(PlayerController player) {
        players[player.index - 1] = player;
    }

    public void StartPlayer(PlayerController player) {
        if (state == State.Menu || state == State.Game) {
            if (state == State.Menu) {
                startGame();
            }
        }
    }

    void startTimer(int seconds) {
        gameTimeFloat = seconds;
        gameTime = seconds;
        timerEnabled = true;
        UICreator.instance.setTimer(gameTime);
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
        UICreator.instance.setTimer(gameTime);

        if (state == State.Minigame && minigameManager != null) {
            // Provide minigame specific updates
            minigameManager.Tick();
        } else if (state == State.Game) {
            if (gameTime % 3 == 0) {
                int index = Random.Range(0, items.Length-1);
                BoxCollider2D region = regions[index];
                float x = Random.Range(0, region.size.x) + region.offset.x - region.size.x/2f;
                float y = Random.Range(0, region.size.y) + region.offset.y - region.size.y/2f;
                int itemIndex = Random.Range(0, items.Length-1);
                Instantiate(items[itemIndex], new Vector2(x, y), Quaternion.identity);
            }

            if (gameTime == GAME_WARNING) {
                minigameIndex = (new System.Random()).Next(MINIGAME_NAMES.Length);
                foreach (PlayerController player in players) {
                    if (player.isPlaying) {
                        player.ShowMessage("Prepare for\n" + MINIGAME_NAMES[minigameIndex], 5f);
                    }
                }
            }
        }
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
            startTimer(GAME_TIME);
        }
        items[0] = Resources.Load<GameObject>("SpeedDown");
        items[1] = Resources.Load<GameObject>("AccelUp");
        items[2] = Resources.Load<GameObject>("AccelDown");
        items[3] = Resources.Load<GameObject>("SpeedUp");
        items[4] = Resources.Load<GameObject>("TractUp");
        items[5] = Resources.Load<GameObject>("TractDown");
        regions = gameObject.GetComponents<BoxCollider2D>();
    }

    void loadMinigame() {
        timerEnabled = false;

        // Transition to random minigame
        state = State.Minigame;
        SceneManager.LoadScene("Minigame" + minigameIndex);

        foreach (PlayerController player in players) {
            if (!player.isPlaying) {
                player.Remove();
            }
        }
    }

    public void startMinigame(MinigameManager manager) {
        minigameManager = manager;
        startTimer(MINIGAME_TIME);
    }

    void loadEnd() {
        state = State.End;

        //SceneManager.LoadScene("End");

        // We don't need a new scene. Just change the canvas.
        startEnd();
    }

    public void startEnd() {
        startTimer(END_TIME);

        bool[] winners = minigameManager.GetWinners();
        float[] scores = minigameManager.playerScores;
        for (int i = 0; i < players.Length; ++i) {
            if (players[i] != null) {
                players[i].StartEnd(winners[i], scores[i]);
            }
        }
        UICreator.instance.refreshBorders();
    }

    void reset() {
        UICreator.instance.Remove();
        Destroy(gameObject);

        SceneManager.LoadScene("MainScene");
    }


    public void OnPlayerCollision(PlayerController playerA, PlayerController playerB) {
        if (state == State.Minigame && minigameManager != null) {
            minigameManager.OnPlayerCollision(playerA, playerB);
        }
    }
}
