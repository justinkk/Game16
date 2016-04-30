﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (DistanceJoint2D))]
public class RopeSegmentController : MonoBehaviour {
   public Transform fromTransform;
   public Transform toTransform;

   protected DistanceJoint2D joint;
   
   void Start() {
      FindJoint();
   }

   /*
    * If you have multiple DistanceJoint2D, set the one of interest
    */
   public void SetJoint(DistanceJoint2D joint) {
      this.joint = joint;
   }

   /*
    * Makes sure joint has been initialized
    */
   private void FindJoint() {
      if (joint == null) {
         joint = GetComponent<DistanceJoint2D>();
         //joint.breakForce = 100000f;
      }
   }

   /*
    * Set the length of this joint
    */
   public void SetLength(float segmentLength) {
      FindJoint();
      joint.distance = segmentLength;
   }
   /*
   public void SetJoints(Transform from, Transform to) {
      fromTransform = from;
      toTransform = to;
      joint.connectedBody = toTransform.GetComponent<Rigidbody2D>();
   }
   */

   /*
    * Get which transform you're attaching to
    */
   public Transform GetTo() {
      return toTransform;;
   }

   /*
    * Get which transform you're attaching from
    */
   public Transform GetFrom() {
      return fromTransform;
   }

   /*
    * Set which transform you're attaching to
    */
   public void SetTo(Transform to) {
      toTransform = to;
      /*
      FindJoint();
      joint.connectedBody = toTransform.GetComponent<Rigidbody2D>();
      */
   }

   /*
    * Set which transform you're attaching from
    */
   public void SetFrom(Transform from) {
      fromTransform = from;
   }
	
	 void FixedUpdate () {
    /*
      //Turn to match rotation of left and right joints
      if (fromTransform != null && toTransform != null) {
         Vector2 delta = new Vector2(fromTransform.position.x - toTransform.position.x,
            fromTransform.position.y - toTransform.position.y);
         float angle = Vector2.Angle(delta, Vector2.up);
         transform.eulerAngles = new Vector3(0, 0, angle + 90);
         //TODO: Fix angle. check out playercontroller code
      }
      */
	 }
}
