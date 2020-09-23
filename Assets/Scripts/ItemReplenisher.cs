using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* An infinite source of a particular item. Every time there are 
 * no colliders inside of it, it spawns a new version of the item.
 * Used before I made self-replenishing items.
 */

public class ItemReplenisher : MonoBehaviour
{

    public GameObject item;
    public bool debugMode = false;

    private Collider box;
    private Vector3 poss;

    void Start() {
        poss = item.transform.position;
        box = GetComponent<Collider>();
    }

    void Update() {
        Collider[] occupants = Physics.OverlapBox(this.transform.position, box.bounds.extents);
        if (debugMode) Debug.Log("There are " + (occupants.Length - 1) + " colliders in the replenisher.");
        
        // It detects it's own collider so looks for when there's just 1
        if (occupants.Length == 1) {
            Instantiate(item, poss, Quaternion.identity);
        }
    }
}
