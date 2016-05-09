using UnityEngine;
using System.Collections;

/* An abstract class Minigame. 
 Automatically invokes the "EndGame" function after maxDuration seconds,
 as a kind of time limit. Default maxDuration is 2 minutes. */
public abstract class Minigame : MonoBehaviour {
    protected static int NUM_PLAYERS = 4;

    private int maxDuration = 120;
    protected bool gameEnded = false;
    protected int[] playerScores = new int[NUM_PLAYERS];
    public int winner
    {
        get; set;
    }

    /* Default to 2-minute long max duration. */
    public Minigame()
    {
    }

    /* Manually set the Minigame max duration. */
    public Minigame(int _maxDuration)
    {
        maxDuration = _maxDuration;
    }

	/* The start method */
	void Start () {
        winner = -1;
        ResetScores();
        Invoke("EndGame", maxDuration);
	}

    /* Reset all player scores to 0 */
    void ResetScores()
    {
        for (int i = 0; i < NUM_PLAYERS; i++)
        {
            playerScores[i] = 0;
        }
    }

    /* PUBLIC METHODS */
    /* ------------------------------ */
    /* Updates the player score by the given value */
    public void updateScore(int playerIndex, int value)
    {
        playerScores[playerIndex] += value;
    }

    /* TODO: Stub function. Called after game ends and the winner is 
     * congratulated to transition back to the title/play screen.  */
    public void Ending()
    {
        /* FILL ME IN */
        /* FILL ME IN */
        /* FILL ME IN */
        /* FILL ME IN */
        /* FILL ME IN */
    }

    /* PUBLIC OVERRIDABLE METHODS */
    /* Can be overrided or extended for minigame-specific behavior/scoring. */
    /* ------------------------------ */

    /* Called when triggering a minigame-affecting event, identified by e.  */
    public virtual void updateEvent(int index, string e) {
        if (e.Length == 0) return; /* Empty string not worth checking. */

        /* */
        if (string.Compare(e, "FinishLine", true) == 0)
        {
            EndGame();
        }
        else if (string.Compare(e, "Bonus", true) == 0)
        {
            /* Stub */
        }
        else if (string.Compare(e, "Hazard", true) == 0)
        {
            /* Stub */
        }
        else
        {
            Debug.Log("Unknown event in Minigame.updateEvent: " + e);
        }
    }

    /* TODO: Stub function. Called in the EndGame method to congratulate the winner.
     * Calls "Ending" in CongratulationDuration seconds. */
    public virtual void CongratulateWinner(int winner)
    {
        int CongratulationDuration = 7;
        /* FILL ME IN */
        /* FILL ME IN */
        /* FILL ME IN */
        /* FILL ME IN */
        /* FILL ME IN */

        Invoke("Ending", CongratulationDuration);
    }

    /* Tallies up player scores and determines the winning player's index. */
    public virtual void EndGame() {
        int max = playerScores[0];
        int maxIndex = 0;
        for (int i = 1; i < NUM_PLAYERS; i++)
        {
            if (playerScores[i] > max)
            {
                maxIndex = i;
                max = playerScores[i];
            }
        }

        CongratulateWinner(maxIndex);
    }
}
