using UnityEngine;
using System.Collections;

public class SafePlatform : StayObject {
    private Vector3 minimumSize = new Vector3(1F, 1F, 0);
    public SafePlatform() : base(1, false, "SafePlatform", 1)
    {
    }

    public override void tick()
    {
        ResetTrigger();
        transform.localScale -= new Vector3(1F, 1F, 0);
        if (transform.localScale.x < 1)
        {
            DestroyObject(gameObject);
        }
    }


    public override void EndStay(PlayerController player)
    {
        GameManager.instance.players[player.index - 1].ShowMessage("You fell off the safe platform and DIED!");
        DestroyObject(player.gameObject);
    }
}
