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
            GameManager.instance.players[player.index - 1].ShowMessage("COLLISION DETECTED");

            if (!triggerable[player.index]) return;

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
}
