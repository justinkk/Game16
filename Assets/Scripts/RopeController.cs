using UnityEngine;
using System.Collections;

[RequireComponent (typeof (DistanceJoint2D), typeof (DistanceJoint2D))]
public class RopeController : RopeSegmentController {
   public RopeSegmentController segment;
   private DistanceJoint2D[] joints;
   /*
    * Instantiates length number of rope segments, leaves lastSegment and currSegment referring to last one
    */
   private void MakeChain(ref RopeSegmentController lastSegment, ref RopeSegmentController currSegment, int length, float segmentLength) {
      for (int i = 0; i < length; i++) {
         currSegment = Instantiate(segment);
         //currSegment.transform.parent = this.transform;
         currSegment.SetLength(segmentLength);
         lastSegment.SetTo(currSegment.transform);
         currSegment.SetFrom(lastSegment.transform);

         lastSegment = currSegment;
      }
   }

   /*
    * Make this rope, by making the appropriate number of attached rope segments
    * numSegments must be at least 1
    */
   public void MakeRope(Transform from, Transform to, float segmentLength, int numSegments) {   
      joints = GetComponents<DistanceJoint2D>();
      joint = joints[0];
      joint.distance = segmentLength;

      //Right side
      RopeSegmentController lastSegment = this;
      RopeSegmentController currSegment = null;
      this.MakeChain(ref lastSegment, ref currSegment, numSegments, segmentLength);
      currSegment.SetTo(to);

      //Left side
      lastSegment = this;
      currSegment = Instantiate(segment);
      //currSegment.transform.parent = this.transform;
      currSegment.SetLength(segmentLength);
      joints[1].connectedBody = currSegment.GetComponent<Rigidbody2D>();
      lastSegment = currSegment;
      currSegment = null;
      MakeChain(ref lastSegment, ref currSegment, numSegments - 1, segmentLength);
      currSegment.SetTo(from);
   }
}
