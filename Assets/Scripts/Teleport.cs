using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* This handles the teleporting functionality which allows users to
 * teleport to a surface at any orientation.
 * The preview mode implements a system like the one in Budget Cuts
 * where players can see the orientation/position they will arive at
 * before commiting.
 */

public class Teleport : MonoBehaviour {

	private float nextFire;
    private float viewDeadzone = 0.9f;
	private bool previewMode = false;
    private bool turned = false;
	private GameObject playerPoss;
	
    public GameObject tracker;
	public GameObject hand;
	public GameObject view;
	public GameObject indicator;
	public RaycastHit savedPoint;
	public Camera previewCamera;
	public float heightLimit;
    public float fireRate = 0.25f;
    public GameObject blinkPlane;


	private bool pressed = false;

	void Start () {
		// Center eye camera
		playerPoss = this.transform.GetChild(1).GetChild(1).gameObject;
		savedPoint = new RaycastHit();
		view.SetActive(false);
		previewCamera.gameObject.SetActive(false);

		this.indicator.SetActive(false);
	}

    // Update is called once per frame
    void Update() {
		// Toggles preview mode vs standard teleport
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            previewMode = !previewMode;
            if (!previewMode) {
                view.SetActive(false);
                previewCamera.gameObject.SetActive(false);
                savedPoint = new RaycastHit();
            } else {
                view.SetActive(true);
                previewCamera.gameObject.SetActive(true);
            }
        }

        // Allows player to rotate view left and right 
		if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > .7f ||
		 			OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > .7f) {
			if (!turned) turnPlayer('r');
			turned = true;
		} else if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -.7f ||
					OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -.7f) {
			if (!turned) turnPlayer('l');
			turned = true;
		} else {
			turned = false;
		}

        // Quick reset scene for testing purposes
		if (OVRInput.Get(OVRInput.Button.Two) && OVRInput.Get(OVRInput.Button.Four)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

        if (previewMode) {
            previewTeleport();
        } else {
            normalTeleport();
        }
    }

	void turnPlayer(char dir) {
		Vector3 ori = Vector3.ProjectOnPlane(playerPoss.transform.position, this.transform.up);
		if (dir == 'r') {
			this.transform.rotation = Quaternion.FromToRotation(this.transform.forward,
											this.transform.right + this.transform.forward) * this.transform.rotation;
		} else {
			this.transform.rotation = Quaternion.FromToRotation(this.transform.forward,
											(this.transform.right * -1) + this.transform.forward) * this.transform.rotation;
		}
		this.transform.position += ori -  Vector3.ProjectOnPlane(playerPoss.transform.position, this.transform.up);
	}

    /* With preview teleport users can essentially send a camera to the teleport location first and view
     * a feed from it on the hand. The feed updates with their movement translated to the propper orientation.
     * This can help with understanding the new orientation before they arrive.
     */
    void previewTeleport() {
    	// Used to adjust view upon teleport to be same orientation as before teleport
    	Vector3 difference = Vector3.ProjectOnPlane(this.transform.position, this.transform.up) - Vector3.ProjectOnPlane(playerPoss.transform.position, this.transform.up);
        Vector3 oldLookAdj = Vector3.zero;
        Vector3 newLook = Vector3.zero;

        if (savedPoint.transform != null) {
    		// Updates the position of the preview camera if a point has been selected
    		previewCamera.gameObject.SetActive(true);
    		previewCamera.transform.up = savedPoint.normal;
    		previewCamera.transform.rotation = Quaternion.FromToRotation(this.transform.up, savedPoint.normal) * this.transform.rotation;
    		previewCamera.transform.position = savedPoint.point + (previewCamera.transform.rotation * hand.transform.localPosition);
    		previewCamera.transform.rotation *= hand.transform.localRotation;

            oldLookAdj = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerPoss.transform.forward, this.transform.up), savedPoint.normal);
            newLook = Vector3.ProjectOnPlane(previewCamera.transform.forward, savedPoint.normal);

            if (oldLookAdj.magnitude > viewDeadzone) {
                previewCamera.transform.rotation = Quaternion.FromToRotation(newLook, oldLookAdj) * previewCamera.transform.rotation;
            }
            view.SetActive(true);
    	} else {
    		view.SetActive(false);
    		previewCamera.gameObject.SetActive(false);
    	}

    	if (OVRInput.GetDown(OVRInput.Button.One) && savedPoint.transform != null){
            // Teleports the player to the saved location if they wish
            this.transform.rotation = Quaternion.FromToRotation(this.transform.up, savedPoint.normal) * this.transform.rotation;
            if (oldLookAdj.magnitude > viewDeadzone)
            {
                this.transform.rotation = Quaternion.FromToRotation(newLook, oldLookAdj) * this.transform.rotation;
            }
            this.transform.position = savedPoint.point;

    		this.transform.position += difference;

    		Physics.gravity = this.transform.up * -6;
    		savedPoint = new RaycastHit();
    	}

    	RaycastHit hit = this.GetComponent<Bezier>().EndPoint;
    	if (hit.transform != null) {

    		if (!indicator.activeSelf)indicator.SetActive(true);

    		indicator.transform.position = hit.point;
    		indicator.transform.forward = tracker.transform.forward;
    		indicator.transform.up = hit.normal;
    		if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > .5f && Time.time > nextFire && hit.transform.tag != "teleportProof"){
    			nextFire = Time.time + fireRate;
    			savedPoint = hit;
                Debug.DrawLine(savedPoint.transform.position, savedPoint.transform.position + savedPoint.normal, Color.red, 100);
    		}
    	} else {
    		indicator.SetActive(false);
    	}
    }

    void normalTeleport() {
        // Gets endpoint of targeting curve
		RaycastHit hit = this.GetComponent<Bezier>().EndPoint;

		if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > .5f) {
			pressed = true;
			this.GetComponent<Bezier>().ToggleDraw(true);
			this.indicator.SetActive(true);
		} else {
			this.GetComponent<Bezier>().ToggleDraw(false);
			this.indicator.SetActive(false);
		}

		if (hit.transform != null) {
			if (indicator.activeSelf) {
                // Updates the targeting ring at the end of the curve
				indicator.transform.position = hit.point;
	            indicator.transform.rotation = Quaternion.FromToRotation(this.transform.up, hit.normal) * this.transform.rotation;
			}

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < .5f && pressed && Time.time > nextFire && hit.transform.tag != "teleportProof"){
                // Actual teleporting happens here
				nextFire = Time.time + fireRate;
				pressed = false;

                // Puts a black plane in front of the player's eyes briefly 
				var blink = Instantiate(blinkPlane, playerPoss.transform.position + playerPoss.transform.forward * .15f,
				 				Quaternion.FromToRotation(new Vector3(0f, 1f, 0f), playerPoss.transform.forward * -1));
				blink.transform.parent = playerPoss.transform;

                // Finds the rotation from current rotation to the new rotation and adjusts player orientation
                Vector3 oldLookAdj = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerPoss.transform.forward, this.transform.up), hit.normal);
                this.transform.rotation = Quaternion.FromToRotation(this.transform.up, hit.normal) * this.transform.rotation;
                Vector3 newLook = Vector3.ProjectOnPlane(playerPoss.transform.forward, this.transform.up);

                // Rotates player further so that they're facing the direction that makes the most sense 
                if (oldLookAdj.magnitude > viewDeadzone ) {
                    this.transform.rotation = Quaternion.FromToRotation(newLook, oldLookAdj) * this.transform.rotation;
                }

                // Adjusts position so the player doesn't arrive in the floor 
				Vector3 difference = Vector3.ProjectOnPlane(this.transform.position, this.transform.up) - Vector3.ProjectOnPlane(playerPoss.transform.position, this.transform.up);
                this.transform.position = hit.point;
    			this.transform.position += difference;
				Physics.gravity = this.transform.up * -6;
			}
		} else {
			indicator.SetActive(false);
		}
	}
}
