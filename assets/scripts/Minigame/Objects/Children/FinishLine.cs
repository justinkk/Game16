using UnityEngine;
using System.Collections;

/* This class sends the "FinishLine" event to Minigame.
 * Represents a player reaching the finish line, which by default
 * ends the game. */
public class FinishLine : OverlapObject {

    public FinishLine(int _value)
        : base(_value, false, "FinishLine")
    {
        if (_value <= 0)
        {
            Debug.Log("In FinishLine.cs constructor: Recommend _value > 0");
        }
    }
}
