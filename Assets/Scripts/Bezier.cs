using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]

/* Used for drawing the curve for targeting teleportation.
 * Makes endpoint accessible to outside classes and changes
 * color of the line to display valid destinations.
 */

public class Bezier : MonoBehaviour {
    public bool endPointDetected;

    public GameObject origin;
    public Material trueMaterial;
    public Material falseMaterial;

    public RaycastHit EndPoint {
        get { return endpoint; }
    }

    public float ExtensionFactor {
        set { extensionFactor = value; }
    }

    private RaycastHit endpoint;
    private float extensionFactor;
    private Vector3[] controlPoints;
    private LineRenderer lineRenderer;
    public float extendStep;
    private int SEGMENT_COUNT = 50;

    void Start() {
        controlPoints = new Vector3[3];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        extensionFactor = 0f;
    }

    void Update() {
        UpdateControlPoints();
        HandleExtension();
        if (endpoint.transform == null || endpoint.transform.tag == "teleportProof") {
            lineRenderer.material = falseMaterial;
        } else {
            lineRenderer.material = trueMaterial;
        }
        DrawCurve();
    }

    public void ToggleDraw(bool draw) {
        lineRenderer.enabled = draw;
    }

    void HandleExtension() {
        if (extensionFactor == 0f)
            return;

        float finalExtension = extendStep + Time.deltaTime * extensionFactor * 2f;
        extendStep = Mathf.Clamp(finalExtension, 2.5f, 7.5f);
    }

    // The first control is the remote. The second is a forward projection. The third is a forward and downward projection.
    void UpdateControlPoints() {
        controlPoints[0] = origin.transform.position; // Get Controller Position
        controlPoints[1] = controlPoints[0] + (origin.transform.forward * extendStep * 2f / 5f);
        controlPoints[2] = controlPoints[1] + (origin.transform.forward * extendStep * 3f / 5f) + this.transform.up * -1f * extendStep / 2;
    }


    // Draw the bezier curve.
    void DrawCurve() {
        if (!lineRenderer.enabled)
            return;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, controlPoints[0]);

        Vector3 prevPosition = controlPoints[0];
        Vector3 nextPosition = prevPosition;
        for (int i = 1; i <= SEGMENT_COUNT; i++) {
            float t = i / (float) SEGMENT_COUNT;
            lineRenderer.positionCount = i + 1;

            if (i == SEGMENT_COUNT) { // For the last point, project out the curve two more meters.
                Vector3 endDirection = Vector3.Normalize(prevPosition - lineRenderer.GetPosition(i-2));
                nextPosition = prevPosition + endDirection * 2f;
            } else {
                nextPosition = CalculateBezierPoint(t, controlPoints[0], controlPoints[1], controlPoints[2]);
            }

            if (CheckColliderIntersection(prevPosition, nextPosition)) { // If the segment intersects a surface, draw the point and return.
                lineRenderer.SetPosition(i, endpoint.point);
                endPointDetected = true;
                return;
            } else { // If the point does not intersect, continue to draw the curve.
                lineRenderer.SetPosition(i, nextPosition);
                endPointDetected = false;
                prevPosition = nextPosition;
            }
        }
    }

    // Check if the line between start and end intersect a collider.
    bool CheckColliderIntersection(Vector3 start, Vector3 end) {
        Ray r = new Ray(start, end - start);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, Vector3.Distance(start, end))) {
            endpoint = hit;
            return true;
        }

        endpoint = hit;

        return false;
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        return
            Mathf.Pow((1f - t), 2) * p0 +
            2f * (1f - t) * t * p1 +
            Mathf.Pow(t, 2) * p2;
    }
}
