using UnityEngine;
using System.Collections.Generic;

public class MinigameManager0 : MinigameManager {

    override public void OnPlayerCollision(PlayerController playerA, PlayerController playerB) {
        if (playerA.IsBoosting()) {
            UpdateScore(playerA.index, 1);
        }
        if (playerB.IsBoosting()) {
            UpdateScore(playerB.index, 1);
        }
    }
}
