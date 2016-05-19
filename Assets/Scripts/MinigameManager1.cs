using UnityEngine;
using System.Linq;

public class MinigameManager1 : MinigameManager {

    private bool[] reached = new bool[4];

    override public string GetInstruction() {
        return "Find your way to the Moon!";
    }

    override public void Start() {
        base.Start();

        // Randomly attach players
        PlayerController firstPlayer = null;
        System.Random r = new System.Random();
        foreach (int i in Enumerable.Range(0, 4).OrderBy(x => r.Next())) {
            PlayerController player = GameManager.instance.players[i];
            if (player != null && player.isPlaying) {
                if (firstPlayer == null) {
                    firstPlayer = player;
                } else {
                    // Attach firstEnd and player
                    GameObject ropePrefab = Resources.Load("Rope") as GameObject;
                    Vector3 location = (firstPlayer.transform.position + player.transform.position) / 2;
                    RopeController rope = Instantiate(ropePrefab).GetComponent<RopeController>();

                    firstPlayer.SetPlayerRope(rope, player.index);
                    player.SetPlayerRope(rope, firstPlayer.index);
                    rope.MakeRope(firstPlayer.transform, player.transform, 0.2f, 8, location);

                    firstPlayer = null;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Player") {
            PlayerController player = coll.gameObject.GetComponent<PlayerController>();
            reached[player.index - 1] = true;

            // Check if both (if connected) players have reached
            if (player.IsAttached()) {
                int other = player.GetAttachedPlayer();
                if (reached[other - 1]) {
                    UpdateScore(player.index, 100);
                    UpdateScore(other, 100);
                    GameManager.instance.endMinigame();
                }
            } else {
                UpdateScore(player.index, 100);
                GameManager.instance.endMinigame();
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll) {
        if (coll.gameObject.tag == "Player") {
            PlayerController player = coll.gameObject.GetComponent<PlayerController>();
            reached[player.index - 1] = false;
        }
    }
}
