using UnityEngine;
using System.Collections.Generic;

public class MinigameManager : MonoBehaviour {

    public GameObject[] playerSpawnpoints = new GameObject[4];

    public float[] playerScores = new float[4];

    virtual public string GetInstruction() { return ""; }

    void Awake() {
        DontDestroyOnLoad(gameObject);
        GameManager.instance.startMinigame(this);
    }

    // Use this for initialization
    public  void Start () {
        for (int i = 0; i < playerSpawnpoints.Length; ++i) {
            PlayerController player = GameManager.instance.players[i];
            if (player != null && player.isPlaying) {
                player.transform.position = playerSpawnpoints[i].transform.position;
                player.transform.rotation = playerSpawnpoints[i].transform.rotation;
                player.body.velocity = Vector2.zero;
                player.body.angularVelocity = 0;
                player.ResetAttachedPlayer();
            }
            Destroy(playerSpawnpoints[i]);
        }
    }
	
	// Update is called once per frame
	virtual public void Update () { }

    virtual public void Tick() { }

    virtual public void UpdateEvent(int index, string e) { }

    virtual public void OnPlayerCollision(PlayerController playerA, PlayerController playerB) { }

    public void UpdateScore(int playerIndex, float value) {
        playerScores[playerIndex - 1] += value;
        GameManager.instance.players[playerIndex - 1].ShowMessage("" + playerScores[playerIndex - 1]);
    }

    public bool[] GetWinners() {
        float maxScore = -1;
        foreach (float score in playerScores) {
            if (score > maxScore) {
                maxScore = score;
            }
        }

        bool[] winners = new bool[4];
        for (int i = 0; i < 4; ++i) {
            if (playerScores[i] == maxScore) {
                winners[i] = true;
            }
        }

        return winners;
    }
}
