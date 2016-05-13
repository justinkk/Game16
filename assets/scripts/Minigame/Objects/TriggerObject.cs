using UnityEngine;
using System.Collections;


/* Coder: avan994
 An abstract class for objects that use trigger events. 

 Parameter Effects on Trigger:
 -Value: modifies players score
 -Autodestroy: autodestroys this object
 -Event Name: passed to minigame

 -Triggerable: Disallows triggering by player until reset
 */
public abstract class TriggerObject : MonoBehaviour
{
    protected static int NUM_PLAYERS = 4;

    protected int value = 0;
    protected bool autodestroy = false;
    protected string eventName = "DEFAULT_OVERLAP_EVENT";
    protected bool[] triggerable = new bool[NUM_PLAYERS];

    protected MinigameManager minigame;

    /*  Initializes the trigger object. */
    public TriggerObject(int _value, bool _autodestroy, string _eventName)
    {
        value = _value;
        autodestroy = _autodestroy;
        eventName = _eventName;
    }

    /* Resets trigger to allow player to trigger them, and finds an
     * instance of the minigame, which is called to update events on overlap. */
    public void Start()
    {
        ResetTrigger();
        minigame = GameObject.Find("MinigameManager").GetComponent<MinigameManager>();
        if (minigame == null)
        {
            Debug.Log("Trigger object created without valid minigame component in scene!");
        }
        minigame.UpdateScore(1, 100);
    }

    public void ResetTrigger()
    {
        for (int i = 0; i < NUM_PLAYERS; i++)
        {
            triggerable[i] = true;
        }
    }

    public void ResetTrigger(int index)
    {
        triggerable[index] = true;
    }
}
