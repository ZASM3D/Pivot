using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Object dissapears when triggered by a switch. Used in early versions before doors
 * or for quick testing.
 */

public class TriggeredObejct : MonoBehaviour
{
    void triggered() {
        this.gameObject.SetActive(false);
    }
}
