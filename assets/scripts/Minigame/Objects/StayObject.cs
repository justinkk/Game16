using UnityEngine;
using System.Collections;


/* Coder: avan994
 A object that the player must stay on for a while to trigger the event. 

 Child of TriggerObject class.

 New Parameter Effects:
 -StayDuration: amount of time required to trigger stay event.
 */
public class StayObject : TriggerObject
{
    private float stayDuration = 1;
    protected float[] timers = new float[NUM_PLAYERS];

    /*  */
    public StayObject(int _value, bool _autodestroy, string _eventName, float _stayDuration) 
        : base(_value, _autodestroy, _eventName)
    {
        stayDuration = _stayDuration;
    }

    void Start()
    {
        ResetTimers();
    }

    /* Count up to timer while player stays */
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();

            if (!triggerable[player.index]) return;
            if (timers[player.index] < stayDuration)
            {
                timers[player.index] += Time.deltaTime;
                return;
            }

            minigame.updateScore(player.index, value);
            minigame.updateEvent(player.index, eventName);

            if (autodestroy)
            {
                Destroy(gameObject);
                return;
            }

            triggerable[player.index] = false;
        }
    }

    /* Reset timer if the player leaves */
    void OnTriggerEnd2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();
            timers[player.index] = 0;
        }
    }

    /* PUBLIC METHODS */
    /* ------------------------- */
    /* Resets the specific player index's timer to 0 */
    public void ResetTimer(int index)
    {
        timers[index] = 0;
    }

    /* Reset all the timers to 0 */
    public void ResetTimers()
    {
        for (int i = 0; i < NUM_PLAYERS; i++)
        {
            timers[i] = 0;
        }
    }
}
