using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used in one of our demo levels to quickly add back all of the targets
 * players were dropping items at.
 */

public class NewTargets : MonoBehaviour
{
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;

    void Update() {
        if (OVRInput.GetDown(OVRInput.Button.Three)) {
            target1.SetActive(true);
            target2.SetActive(true);
            target3.SetActive(true);
            target4.SetActive(true);
        }
    }
}
