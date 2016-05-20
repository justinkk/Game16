using UnityEngine;
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
    const int GAME_HELP1_TIME = GAME_TIME - 5;
    const int MINIGAME_WARNING_TIME = 10;
    const int MINIGAME_TIME = 2 * 60;
    const int END_TIME = 30;

    const string GAME_HELP1 = "Try ramming into each other";
    const string GAME_HELP2 = "Now explore the amusement park";
    const string MINIGAME_WARNING = "Prepare for\n";

    public enum State {
        Menu,
        Game,
        Minigame,
        End
    }
    public State state = State.Menu;

    public GameObject tireBox;

    static readonly string[] MINIGAME_NAMES = {
        "Bumper Cars",
        "The Maze"
    };
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

    public bool CanJoin() {
        return state == State.Menu || state == State.Game;
    }

    public void StartPlayer(PlayerController player) {
        if (state == State.Menu) {
            startGame();
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

    void spawnRandomItemAtLocation(float x, float y) {
        int itemIndex = Random.Range(0, items.Length - 1);
        Instantiate(items[itemIndex], new Vector2(x, y), Quaternion.identity);
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
                int index = Random.Range(0, regions.Length-1);
                BoxCollider2D region = regions[index];
                if (region != null) {
                    float x = Random.Range(0, region.size.x) + region.offset.x - region.size.x / 2f;
                    float y = Random.Range(0, region.size.y) + region.offset.y - region.size.y / 2f;
                    spawnRandomItemAtLocation(x, y);
                }
            }

            if (gameTime == GAME_HELP1_TIME && tireBox != null) {
                foreach (PlayerController player in players) {
                    if (player.isPlaying) {
                        player.ShowMessage(GAME_HELP1, 5f);
                    }
                }
            }
            else if (gameTime == MINIGAME_WARNING_TIME) {
                minigameIndex = (new System.Random()).Next(MINIGAME_NAMES.Length);
                foreach (PlayerController player in players) {
                    if (player.isPlaying) {
                        player.ShowMessage(MINIGAME_WARNING + MINIGAME_NAMES[minigameIndex], MINIGAME_WARNING_TIME);
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
        
        foreach (PlayerController player in players) {
            if (player != null && player.isPlaying) {
                player.ShowMessage(minigameManager.GetInstruction(), 5f);
            }
        }
    }

    public void endMinigame() {
        if (state == State.Minigame) {
            gameTime = 0;
            tick();
        }
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

    float randomNegativeOneOrPositiveOne() {
        float r = Random.Range(0, 1);
        if (r > 0.5f) {
            return 1.0f;
        }
        return -1.0f;
    }

    public void OnPlayerCollision(PlayerController playerA, PlayerController playerB) {
        if (state == State.Game) {
            if (tireBox != null && playerA.IsBoosting()) {
                Destroy(tireBox);
                tireBox = null;
                
                foreach (PlayerController player in players) {
                    if (player.isPlaying) {
                        player.ShowMessage(GAME_HELP2, 5f);
                    }
                }
            } else if (playerA.IsBoosting()) {
                if (playerA.DidChangeAttachedPlayer()) {
                    float x = playerB.body.transform.position.x + Random.Range(3, 5) * randomNegativeOneOrPositiveOne();
                    float y = playerB.body.transform.position.y + Random.Range(3, 5) * randomNegativeOneOrPositiveOne();
                    spawnRandomItemAtLocation(x, y);
                }
            }
        } else if (state == State.Minigame && minigameManager != null) {
            minigameManager.OnPlayerCollision(playerA, playerB);
        }
    }
}
