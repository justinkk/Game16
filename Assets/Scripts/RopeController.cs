using UnityEngine;
using System.Collections;

[RequireComponent (typeof (DistanceJoint2D), typeof (DistanceJoint2D))]
public class RopeController : RopeSegmentController {
   //public RopeSegmentController segment;
   private DistanceJoint2D[] joints;
   private GameObject segment;

   /*
    * Instantiates length number of rope segments, returns the last segment in the chain
    */
   private RopeSegmentController MakeChain(RopeSegmentController lastSegment, RopeSegmentController currSegment, int length, float segmentLength,
         GameObject segment) {
      lastSegment.SetFrom(transform);

      for (int i = 0; i < length; i++) {
         currSegment = Instantiate(segment).GetComponent<RopeSegmentController>();
         currSegment.transform.parent = transform;

         currSegment.SetLength(segmentLength);
         currSegment.SetFrom(lastSegment.transform);
         lastSegment.SetTo(currSegment.transform);

         if (i == 0) {
            print(currSegment.GetComponent<Rigidbody2D>());
            print(lastSegment.GetComponent<DistanceJoint2D>().connectedBody);
            lastSegment.GetComponent<DistanceJoint2D>().connectedBody = currSegment.GetComponent<Rigidbody2D>();
            print(lastSegment.GetComponent<DistanceJoint2D>().connectedBody);

         }
         lastSegment = currSegment;
      }
      /*
      for (int i = 0; i < length; i++) {
         currSegment = (RopeSegmentController) Instantiate(segment);
         //currSegment.transform.parent = this.transform;
         currSegment.SetLength(segmentLength);
         currSegment.SetFrom(lastSegment.transform);
         lastSegment.SetTo(currSegment.transform);

         if (i == 0) {
            print(currSegment.GetComponent<Rigidbody2D>());
            print(lastSegment.GetComponent<DistanceJoint2D>().connectedBody);
            lastSegment.GetComponent<DistanceJoint2D>().connectedBody = currSegment.GetComponent<Rigidbody2D>();
            print(lastSegment.GetComponent<DistanceJoint2D>().connectedBody);

         }
         lastSegment = currSegment;
      }
      */

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

   /*
    * Instantiates length number of rope segments to the right side, returns the last one
    */
    /*
   private RopeSegmentController MakeChainRight(RopeSegmentController lastSegment, 
         RopeSegmentController currSegment, int length, float segmentLength) {
      return MakeChain(lastSegment, currSegment, length, segmentLength, toTransform.gameObject.GetComponent<RopeSegmentController>());
   }
   */

   /*
    * Instantiates length number of rope segments to the left side, returns the last one
    */
    /*
   private RopeSegmentController MakeChainLeft(RopeSegmentController lastSegment, 
         RopeSegmentController currSegment, int length, float segmentLength) {
      return MakeChain(lastSegment, currSegment, length, segmentLength, fromTransform.gameObject.GetComponent<RopeSegmentController>());
   }
   */

   /*
    * Make this rope, by making the appropriate number of attached rope segments
    * numSegments must be at least 1
    */
   public void MakeRope(Transform from, Transform to, float segmentLength, int numSegments) { 
      segment = Resources.Load("RopeSegment") as GameObject;
      GameObject leftSegment = Instantiate(segment);
      fromTransform = leftSegment.transform;
      fromTransform.parent = transform;
      GameObject rightSegment = Instantiate(segment);
      toTransform = rightSegment.transform;
      toTransform.parent = transform;

      joints = GetComponents<DistanceJoint2D>();
      joint = joints[0];
      joint.distance = segmentLength;

      //Right side
      RopeSegmentController lastSegment = leftSegment.GetComponent<RopeSegmentController>();
      RopeSegmentController currSegment = lastSegment;
      currSegment = MakeChain(lastSegment, currSegment, numSegments - 1, segmentLength, segment);
      currSegment.SetTo(to);

      joints[0].connectedBody = leftSegment.GetComponent<Rigidbody2D>();
      AttachChain(leftSegment.GetComponent<RopeSegmentController>(), numSegments);

      //Left side
      lastSegment = rightSegment.GetComponent<RopeSegmentController>();
      currSegment = lastSegment;
      currSegment = MakeChain(lastSegment, currSegment, numSegments - 1, segmentLength, segment);
      currSegment.SetTo(from);


      joints[1].connectedBody = rightSegment.GetComponent<Rigidbody2D>();
      AttachChain(rightSegment.GetComponent<RopeSegmentController>(), numSegments);

      /*

      joints = GetComponents<DistanceJoint2D>();
      joint = joints[0];
      joint.distance = segmentLength;

      //Right side
      RopeSegmentController lastSegment = toTransform.gameObject.GetComponent<RopeSegmentController>();
      RopeSegmentController currSegment = lastSegment;
      currSegment = MakeChainRight(lastSegment, currSegment, numSegments - 1, segmentLength);
      currSegment.SetTo(to);

      //Left side
      lastSegment = fromTransform.gameObject.GetComponent<RopeSegmentController>();
      currSegment = lastSegment;
      currSegment = MakeChainLeft( lastSegment, currSegment, numSegments - 1, segmentLength);
      currSegment.SetTo(from);

      */

      /*
      //Right side
      RopeSegmentController currSegment = Instantiate(segment);
      toTransform = currSegment.transform;
      print(toTransform);
      //currSegment.transform.parent = this.transform;
      currSegment.SetLength(segmentLength);
      joints[0].breakForce = Mathf.Infinity;
      joints[0].connectedBody = toTransform.GetComponent<Rigidbody2D>();
      
      RopeSegmentController lastSegment = currSegment;
      currSegment = null;
      MakeChain(ref lastSegment, ref currSegment, numSegments - 1, segmentLength);
      currSegment.SetTo(to);

      //Left side
      //lastSegment = this;
      currSegment = Instantiate(segment);
      //currSegment.transform.parent = this.transform;
      currSegment.SetLength(segmentLength);
      joints[1].breakForce = Mathf.Infinity;
      joints[1].connectedBody = currSegment.gameObject.GetComponent<Rigidbody2D>();
      fromTransform = currSegment.transform;
      lastSegment = currSegment;
      currSegment = null;
      MakeChain(ref lastSegment, ref currSegment, numSegments - 1, segmentLength);
      currSegment.SetTo(from);
      */
      /*
      //Right side
      RopeSegmentController lastSegment = this;
      RopeSegmentController currSegment = null;
      this.MakeChain(ref lastSegment, ref currSegment, numSegments, segmentLength);
      currSegment.SetTo(to);

      //Left side
      //lastSegment = this;
      currSegment = Instantiate(segment);
      //currSegment.transform.parent = this.transform;
      currSegment.SetLength(segmentLength);
      joints[1].connectedBody = currSegment.GetComponent<Rigidbody2D>();
      this.SetTo(currSegment.transform);
      lastSegment = currSegment;
      currSegment = null;
      MakeChain(ref lastSegment, ref currSegment, numSegments - 1, segmentLength);
      currSegment.SetTo(from);
      */
   }

}
