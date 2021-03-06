﻿using UnityEngine;
using System.Collections;

public class TrackTrigger : MonoBehaviour {

    Track track;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void setTrack(Track track) {
        this.track = track;
    }

    void OnTriggerStay2D(Collider2D other) {

        if (track == null || track.next == null) {
            return;
        }

        Vector3 forceDirection = track.next.track.transform.localPosition - track.track.transform.localPosition;

        if (track.isCurve) {
            if (track.direction == Track.EAST || track.direction == Track.WEST) {
                forceDirection.y = forceDirection.y / 2;
            } else {
                forceDirection.x = forceDirection.x / 2;
            }

        }

        other.attachedRigidbody.AddForce(new Vector2(forceDirection.x / 2, forceDirection.y / 2), ForceMode2D.Force);



    }
}

