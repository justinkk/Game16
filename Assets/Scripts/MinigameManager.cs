using UnityEngine;
using System.Collections.Generic;

public class MinigameManager : MonoBehaviour {

    public float[] playerScores = new float[4];

    void Awake() {
        DontDestroyOnLoad(gameObject);
        GameManager.instance.startMinigame(this);
    }

    // Use this for initialization
    virtual public  void Start () { }
	
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
