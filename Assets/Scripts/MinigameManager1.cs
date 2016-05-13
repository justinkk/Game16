using UnityEngine;
using System.Collections;

public class MinigameManager1 : MinigameManager {
    private int scoreToWin = 4;

    override public void UpdateEvent(int index, string e) {
        if (string.Compare(e, "Bonus") == 0)
        {
            if (playerScores[index - 1] >= scoreToWin)
            {
                GameManager.instance.players[index - 1].ShowMessage("WINNER: " + playerScores[index - 1]);
                //GameManager.instance.transitionState();
            }
        }
    }

}
