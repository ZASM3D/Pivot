using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Breifly blacks out the player's vision when teleporting
 * so the transition is smoother.
 */
public class BlinkPlane : MonoBehaviour
{
    private float fadePerSecond = 7f;

    void Start() {
        var col = GetComponent<Renderer>().material.color;
        col.a = .5f;
        GetComponent<Renderer>().material.color = col;
        Destroy(this.gameObject, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        var material = GetComponent<Renderer>().material;
        var color = material.color;

        material.color = new Color(color.r, color.g, color.b, color.a + (fadePerSecond * Time.deltaTime));
        if (material.color.a > .9) fadePerSecond *= -1;
        if (material.color.a < 0) Destroy(this.gameObject);
    }
}
