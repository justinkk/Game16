using UnityEngine;
using System.Collections;


/* Coder: avan994
 A object that the player must overlap to trigger events.

 Child of TriggerObject class.
 */
public class OverlapObject : TriggerObject {
    /*  */
    public OverlapObject(int _value, bool _autodestroy, string _eventName)
        : base(_value, _autodestroy, _eventName)
    {
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();

            if (!triggerable[player.index]) return;

            minigame.updateScore(player.index, value);
            minigame.updateEvent(eventName);

            if (autodestroy)
            {
                Destroy(gameObject);
                return;
            }

            triggerable[player.index] = false;
        }
    }
}
