using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    
    public BoxCollider block;
    public AudioSource audio;
    
    public int numTriggers = 1;
    public AudioClip open;
    public AudioClip close;

    private Animator anim;
    private bool closed = false;

    void Start() {
        anim = GetComponent<Animator>();
        block.enabled = true;
        anim.Play("Close");
    }

    void triggered() {
        // Play the animation and sound
        if (numTriggers == 1) {
            // Only opens if all required triggers are activated
            anim.Play("Open");
            block.enabled = false;
            audio.clip = open;
            audio.Play();
        } else {
            numTriggers--;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 11 && !closed) {
            // Make the collider solid and trigger closing animation and sound 
            block.enabled = true;
            anim.Play("Close");
            closed = true;
            audio.clip = close ;
            audio.Play();
        }
    }
}
