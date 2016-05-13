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
    protected int[] timers = new int[NUM_PLAYERS];
    protected bool[] staying = new bool[NUM_PLAYERS];
    protected float mainTimer;
    protected int mainTime;

    /*  */
    public StayObject(int _value, bool _autodestroy, string _eventName, int _stayDuration) 
        : base(_value, _autodestroy, _eventName)
    {
        stayDuration = _stayDuration;
    }

    new void Start()
    {
        base.Start();
        ResetTimers();
    }

    void Update()
    {
        mainTimer += Time.deltaTime;

        if (mainTime <= mainTimer - 1)
        {
            mainTime++;
            for (int i = 0; i < NUM_PLAYERS; i++)
            {
                if (staying[i])
                {
                    timers[i]++;
                } else
                {
                    //GameManager.instance.players[i].ShowMessage("WWE SEE YOU THERE. ");
                }
            }
            tick();
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
    }

    /* Count up to timer while player stays */
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();

            if (!triggerable[player.index]) return;

            staying[player.index] = true;
            if (timers[player.index] < stayDuration)
            {
                return;
            }

            minigame.UpdateScore(player.index, value);
            minigame.UpdateEvent(player.index, eventName);

            if (autodestroy)
            {
                Destroy(gameObject);
                return;
            }

            triggerable[player.index] = false;
        }
    }

    /* Reset timer if the player leaves */
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();
            timers[player.index] = 0;
            //staying[player.index] = false;
            EndStay(player);
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

    /* Overrideable method, called every second. */
    public virtual void tick() {}

    public virtual void EndStay(PlayerController player) {}
}
