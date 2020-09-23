using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used on animation components so they don't clutter
 * the scene after they've finished
 */
public class Fadeout : MonoBehaviour
{
    private float fadePerSecond = 3.5f;

     private void Update() {
         var material = GetComponent<Renderer>().material;
         var color = material.color;

         material.color = new Color(color.r, color.g, color.b, color.a - (fadePerSecond * Time.deltaTime));
         Destroy(this.gameObject, .5f);
     }
}
