using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public PlayerController[] players = new PlayerController[4];
    GameObject[] items = new GameObject[8];
    BoxCollider2D[] regions = new BoxCollider2D[7];

    bool timerEnabled = false;
    float gameTimeFloat = 0;
    int gameTime = 0;

    const int GAME_TIME = 3 * 60;
    const int GAME_HELP1_TIME = GAME_TIME - 5;
    const int MINIGAME_WARNING_TIME = 10;
    const int MINIGAME_TIME = 100;
    const int END_TIME = 30;
    const int ITEM_LIFETIME = 60;
    const int NUM_INITIAL_ITEMS = 20;

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

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start() {

    }

    public void CreatePlayer(PlayerController player) {
        players[player.index - 1] = player;
        player.mostRecentTimeStatStolen = GAME_TIME;
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
        //Spawn items with 2:1 positive:negative ratio
        int rand = Random.Range(0, items.Length * 3 / 2);
        int index = 0;
        if (rand < items.Length) {
            index = rand / 2;
        } else {
            index = rand - (items.Length / 2);
        }
        //Random.Range is inclusive, so need to account for chance that index = items.Length
        if (index > items.Length - 1) {
            index = items.Length - 1;
        }
        Destroy(Instantiate(items[index], new Vector2(x, y), Quaternion.identity), ITEM_LIFETIME);
    }

    void spawnRandomItemAtRandomLocation() {
        int index = Random.Range(0, regions.Length - 1);
        BoxCollider2D region = regions[index];
        if (region != null) {
            float x = Random.Range(0, region.size.x) + region.offset.x - region.size.x / 2f;
            float y = Random.Range(0, region.size.y) + region.offset.y - region.size.y / 2f;
            spawnRandomItemAtLocation(x, y);
        }
    }

    void loadInitialItems() {
        for (int i = 0; i < NUM_INITIAL_ITEMS; i++) {
            spawnRandomItemAtRandomLocation();
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
            for (int i = 0; i < 3; i++) {
                if (Random.value < 0.4) {
                    spawnRandomItemAtRandomLocation();
                }
            }

            if (gameTime == GAME_HELP1_TIME && tireBox != null) {
                foreach (PlayerController player in players) {
                    if (player.isPlaying) {
                        player.ShowMessage(GAME_HELP1, 5f);
                    }
                }
            } else if (gameTime == MINIGAME_WARNING_TIME) {
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
        items[0] = Resources.Load<GameObject>("SpeedUp");
        items[1] = Resources.Load<GameObject>("AccelUp");
        items[2] = Resources.Load<GameObject>("TractUp");
        items[3] = Resources.Load<GameObject>("BoostUp");
        items[4] = Resources.Load<GameObject>("SpeedDown");
        items[5] = Resources.Load<GameObject>("AccelDown");
        items[6] = Resources.Load<GameObject>("TractDown");
        items[7] = Resources.Load<GameObject>("BoostDown");
        regions = gameObject.GetComponents<BoxCollider2D>();
        loadInitialItems();
    }

    void loadMinigame() {
        timerEnabled = false;

        // Transition to random minigame
        state = State.Minigame;
        SceneManager.LoadScene("Minigame" + minigameIndex);

        foreach (PlayerController player in players) {
            if (player.isPlaying) {
                player.gameObject.transform.SetParent(null);
            } else {
                player.Remove();
            }
        }
    }

    public void startMinigame(MinigameManager manager) {
        minigameManager = manager;
        startTimer(MINIGAME_TIME);

        foreach (PlayerController player in players) {
            if (player != null && player.isPlaying) {
                //Reset score
                manager.UpdateScore(player.index, 0);
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

    int chooseItemToSpawn(PlayerController player) {
        int index = 0;
        int max = player.GetStatLevels()[0];
        for (int i = 1; i < StatConstants.NUM_STATS - 1; i++) {
            if (player.GetStatLevels()[i] > max) {
                index = i;
            }
        }
        return index;
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
            } else if (playerA.IsBoosting() && gameTime < (playerB.mostRecentTimeStatStolen - 5)) {
                if (playerA.DidChangeAttachedPlayer()) {
                    float x = playerB.body.transform.position.x + Random.Range(3, 5) * randomNegativeOneOrPositiveOne();
                    float y = playerB.body.transform.position.y + Random.Range(3, 5) * randomNegativeOneOrPositiveOne();
                    int itemIndex = chooseItemToSpawn(playerB);
                    playerB.ChangeStat(itemIndex, false);
                    Destroy(Instantiate(items[itemIndex], new Vector2(x, y), Quaternion.identity), ITEM_LIFETIME);
                    playerB.mostRecentTimeStatStolen = gameTime;
                }
            }
        } else if (state == State.Minigame && minigameManager != null) {
            minigameManager.OnPlayerCollision(playerA, playerB);
        }
    }
}
