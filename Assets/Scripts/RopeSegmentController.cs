using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DistanceJoint2D))]
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
        return toTransform; ;
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

    void FixedUpdate() {
        //Turn to match rotation of left and right joints
        if (fromTransform != null && toTransform != null) {
            Vector2 delta = new Vector2(-fromTransform.position.y + toTransform.position.y,
              fromTransform.position.x - toTransform.position.x);
            if (delta != Vector2.zero) {
                float angle = Vector2.Angle(Vector2.up, delta);
                if (delta.x > 0)
                    angle = 360 - angle;

                float angleDifference = angle - transform.eulerAngles.z;
                if (angleDifference < -180)
                    angleDifference += 360;
                else if (angleDifference > 180)
                    angleDifference -= 360;

                //Quickly go, faster for a bigger angle difference. Biggest when difference is 180
                GetComponent<Rigidbody2D>().angularVelocity
                  = PlayerController.OFFSET_TO_ANGULAR_VELOCITY * Mathf.Sin(Mathf.PI * angleDifference / (360 * 2));
            }
        }
    }
}
