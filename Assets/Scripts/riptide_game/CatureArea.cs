using UnityEngine;
using UnityEngine.Splines;

public class CatureArea : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    bool isPointInSplineArea(Vector3 point)
    {

        // Implement ray casting algorithm to check if the point is inside the spline area
        Spline spline = GetComponent<Spline>();
        if (spline == null)
        {
            Debug.LogError("Spline component not found on the GameObject.");
            return false;
        }

        // The spline will be closed
        // Check both directions of the ray and if it intersects the spline, it is containd inside
        Ray rayForward = new Ray(point, Vector3.forward);
        Ray rayBackward = new Ray(point, Vector3.back);
        bool intersectsForward = doesRayIntersectSpline(rayForward, spline);
        bool intersectsBackward = doesRayIntersectSpline(rayBackward, spline);

        return intersectsForward || intersectsBackward;
    }

    bool doesRayIntersectSpline(Ray ray, Spline spline)
    {
        // Iterate through the spline segments and check for intersection
        for (int i = 0; i < spline.Count - 1; i++)
        {
            Vector3 start = spline[i].Position;
            Vector3 end = spline[i + 1].Position;
            Vector3 direction = end - start;
            float segmentLength = direction.magnitude;
            direction.Normalize();
            float t = Vector3.Dot(ray.direction, direction);
            if (t > 0 && t < segmentLength)
            {
                Vector3 closestPoint = start + direction * t;
                float distance = Vector3.Distance(ray.origin, closestPoint);
                if (distance < 0.1f) // Assuming a threshold for intersection
                {
                    return true;
                }
            }
        }
        return false;
    }
}

