using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by objects where the direction of gravity
 * remains constant despite player movement.
 */

public class ConstantGravity : MonoBehaviour
{

    public float speed;
    public float speed_cap;
    public GameObject anim;

    private Vector3 localGravity;
    private Rigidbody rb;
    private bool isGrabbed;
    private Vector3 origin_pos;
    private Quaternion origin_rot;
    private bool canBreak;

    // Timer prevents the object from breaking on the player's hands
    private float timer = .02f;
    private float nextFire;

    void Start() {
        //Establishes gravity and point to return to based on initial position
        localGravity = this.transform.up * -speed;
        origin_pos = this.transform.position;
        origin_rot = this.transform.rotation;
        isGrabbed = false;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        nextFire = Time.time;
    }

    void startGrab() {
        isGrabbed = true;
        this.gameObject.layer = 2;
    }

    void endGrab() {
        isGrabbed = false;
        nextFire = Time.time + timer;
        this.gameObject.layer = 18;
    }

    void Update() {
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    // Shatters when it collides with an object that isn't the player and it isn't being grabbed
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject != this.gameObject && other.gameObject.tag != "Player"
                && rb.velocity.sqrMagnitude > speed_cap && !isGrabbed && Time.time > nextFire)
        {
            shatter();
        }
    }

    // Upon breaking plays the appropriate animation and returns to original position
    void shatter() {
        Instantiate(anim, this.transform.position, this.transform.rotation);
        this.transform.position = origin_pos;
        this.transform.rotation = origin_rot;
        rb.velocity = Vector3.zero;
    }
}
