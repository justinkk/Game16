using UnityEngine;
using System.Collections;

[RequireComponent (typeof (DistanceJoint2D), typeof (DistanceJoint2D))]
public class RopeController : RopeSegmentController {
   //public RopeSegmentController segment;
   private DistanceJoint2D[] joints;
   private GameObject segment;
   private PlayerController fromPlayerController;
   private PlayerController toPlayerController;

   /*
    * Instantiates length number of rope segments, returns the last segment in the chain
    */
   private RopeSegmentController MakeChain(RopeSegmentController lastSegment, RopeSegmentController currSegment, int length, float segmentLength,
         GameObject segment, Vector3 location) {
      lastSegment.SetFrom(transform);

      for (int i = 0; i < length; i++) {
         currSegment = Instantiate(segment).GetComponent<RopeSegmentController>();
         currSegment.transform.parent = transform;
         currSegment.transform.position = location;

         currSegment.SetLength(segmentLength);
         currSegment.SetFrom(lastSegment.transform);
         lastSegment.SetTo(currSegment.transform);

         lastSegment = currSegment;
      }

      return currSegment;
   }

   /*
    * Attaches the DistanceJoint2Ds along the chain
    */
   private void AttachChain(RopeSegmentController currSegment, int length) {
      for (int i = 0; i < length; i++) {
         currSegment.GetComponent<DistanceJoint2D>().connectedBody = currSegment.GetTo().GetComponent<Rigidbody2D>();
         currSegment = currSegment.GetTo().GetComponent<RopeSegmentController>();
      }
   }

   /**
    * Deletes this rope, and tells the attached players they aren't attached anymore
    */
   public void DeleteRope() {
      fromPlayerController.SetPlayerRope(null, -1);
      toPlayerController.SetPlayerRope(null, -1);

      Destroy(gameObject);
   }

   /*
    * Make this rope, by making the appropriate number of attached rope segments
    * numSegments must be at least 1
    */
   public void MakeRope(Transform fromPlayer, Transform toPlayer, float segmentLength, int numSegments, Vector3 location) { 
      transform.position = location;
      segment = Resources.Load("RopeSegment") as GameObject;

      GameObject leftSegment = Instantiate(segment);
      fromTransform = leftSegment.transform;
      fromTransform.position = location;
      fromTransform.parent = transform;

      GameObject rightSegment = Instantiate(segment);
      toTransform = rightSegment.transform;
      toTransform.position = location;
      toTransform.parent = transform;

      joints = GetComponents<DistanceJoint2D>();
      joint = joints[0];
      joint.distance = segmentLength;

      //Right side
      RopeSegmentController lastSegment = leftSegment.GetComponent<RopeSegmentController>();
      RopeSegmentController currSegment = lastSegment;
      currSegment = MakeChain(lastSegment, currSegment, numSegments - 1, segmentLength, segment, location);
      currSegment.SetTo(toPlayer);
      toPlayerController = toPlayer.GetComponent<PlayerController>();

      joints[0].connectedBody = leftSegment.GetComponent<Rigidbody2D>();
      AttachChain(leftSegment.GetComponent<RopeSegmentController>(), numSegments);

      //Left side
      lastSegment = rightSegment.GetComponent<RopeSegmentController>();
      currSegment = lastSegment;
      currSegment = MakeChain(lastSegment, currSegment, numSegments - 1, segmentLength, segment, location);
      currSegment.SetTo(fromPlayer);
      fromPlayerController = fromPlayer.GetComponent<PlayerController>();


      joints[1].connectedBody = rightSegment.GetComponent<Rigidbody2D>();
      AttachChain(rightSegment.GetComponent<RopeSegmentController>(), numSegments);
   }

}
