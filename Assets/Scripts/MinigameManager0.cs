using UnityEngine;
using System.Collections.Generic;

public class MinigameManager0 : MinigameManager {

	override public List<int> getWinners() {
        List<int> winners = new List<int>();
        winners.Add(2);
        return winners;
    }
}
