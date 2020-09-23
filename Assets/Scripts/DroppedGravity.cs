using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* First prototype of object that changes gravity based on its
 * orientation while being held. Has a shadow that indicates where
 * it will fall towards.
 */

public class DroppedGravity : MonoBehaviour
{
    private Vector3 localGravity = new Vector3(0, -1, 0);

    public float speed;
    public int shadow_length;

    private GameObject shadow;
    private Rigidbody rb;
    private bool isGrabbed;
    private Vector3 origin_pos;
    private Quaternion origin_rot;

    void Start() {
        origin_pos = this.transform.position;
        origin_rot = this.transform.rotation;
        isGrabbed = false;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        shadow = this.transform.GetChild(0).gameObject;
        newGravity();
    }

    void Update() {
        if (isGrabbed) {
            // Shadow appears on surface directly 'below' the object
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.up * -1, out hit, shadow_length)) {
                shadow.SetActive(true);
                shadow.transform.position = hit.point + (hit.normal * .001f);
                shadow.transform.up = hit.normal;
            } else {
                shadow.SetActive(false);
            }
        } else {
            shadow.SetActive(false);
        }
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    void newGravity() {
        localGravity = this.transform.up * -1 * speed;
    }

    void startGrab() {
        isGrabbed = true;
    }

    void endGrab() {
        isGrabbed = false;
        newGravity();
    }

    void shatter() {
        // Play animation here

        this.transform.position = origin_pos;
        this.transform.rotation = origin_rot;
        rb.velocity = Vector3.zero;
        newGravity();
    }
}
