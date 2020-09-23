using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A target that has objects dropped on it to activate a 
 * connected object. 
 */

public class TargetSwitch : MonoBehaviour
{
    public GameObject connection;
    private bool used = false;
    private Animator anim;


    void Start() {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other) {
        if (!used && other.gameObject.layer == 18) {
            anim.Play("Take");
            used = true;
            other.gameObject.SendMessage("shatter", SendMessageOptions.DontRequireReceiver);
            Destroy(other.gameObject);
            connection.SendMessage("triggered", SendMessageOptions.DontRequireReceiver);
        }
    }
}
