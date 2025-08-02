using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TrailAreaBehaviour : MonoBehaviour
{
    LineRenderer lineRenderer;

    public float TrailLingeringTime = 1;

    public bool isSafe = false;
    public bool shouldProcess = false;

    public List<GameObject> objectsInside;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetConfirmed(bool isClosed)
    {
        // Start the lingering timer
        // Find if any objects are contained inside the trail area
        if (isClosed)
        {
            objectsInside = FindObjectsInArea();
            SetClosed();
        }
        else
        {
            SetBroken();
        }
        StartCoroutine(LingeringTimer());
    }

    List<GameObject> FindObjectsInArea()
    {
        List<GameObject> foundObjects = new List<GameObject>();
        foreach (GameObject selectable in GameObject.FindGameObjectsWithTag("Selectable"))
        {
            if (isPointInSelectionArea(selectable.transform.position))
            {
                foundObjects.Add(selectable);
            }
        }

        return foundObjects;
    }

    bool isPointInSelectionArea(Vector3 point)
    {
        // Use Ray casting algorithm to determine if the point is inside the polygon defined by selectionPath
        int intersections = 0;
        Vector3 rayStart = new Vector2(point.x, point.z - 1000f);
        Vector3 rayEnd = new Vector2(point.x, point.z + 1000f);
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 segmentStart = new Vector2(lineRenderer.GetPosition(i).x, lineRenderer.GetPosition(i).z);
            Vector3 segmentEnd = new Vector2(lineRenderer.GetPosition(i + 1).x, lineRenderer.GetPosition(i + 1).z);
            if (TwoLinesIntersect(segmentStart, segmentEnd, rayStart, rayEnd))
            {
                intersections++;
            }
        }
        // Debug.Log("Intersections: " + intersections + " for point: " + point);
        if (intersections == 0) return false;
        // If the number of intersections is even, the point is inside the polygon
        return intersections % 2 == 0;
    }

    bool TwoLinesIntersect(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
    {
        float denominator = (line1End.x - line1Start.x) * (line2End.y - line2Start.y) - (line1End.y - line1Start.y) * (line2End.x - line2Start.x);
        if (Mathf.Approximately(denominator, 0f))
            return false; // Lines are parallel

        float ua = ((line2End.x - line2Start.x) * (line1Start.y - line2Start.y) - (line2End.y - line2Start.y) * (line1Start.x - line2Start.x)) / denominator;
        float ub = ((line1End.x - line1Start.x) * (line1Start.y - line2Start.y) - (line1End.y - line1Start.y) * (line1Start.x - line2Start.x)) / denominator;

        return ua >= 0f && ua <= 1f && ub >= 0f && ub <= 1f;
    }

    IEnumerator LingeringTimer()
    {
        yield return new WaitForSeconds(TrailLingeringTime);
        shouldProcess = true;
    }

    void Update()
    {
        if (shouldProcess) HandleObjectMovementAcrossTrail();
    }

    void HandleObjectMovementAcrossTrail()
    {
        List<GameObject> newFrameObjects = FindObjectsInArea();
        // Rare occurance
        if (newFrameObjects.Count == objectsInside.Count) return;

        List<GameObject> objectsThatHavePassedTrail = new List<GameObject>();

        foreach (GameObject obj in objectsInside)
        {
            // Check if the object is still inside the trail area
            if (!newFrameObjects.Contains(obj))
            {
                // Object has left the trail area
                objectsThatHavePassedTrail.Add(obj);
            }
        }

        foreach (GameObject obj in newFrameObjects)
        {
            if (!objectsInside.Contains(obj))
            {
                // Object has entered the trail area
                objectsThatHavePassedTrail.Add(obj);
            }
        }

        if (objectsThatHavePassedTrail.Count > 0) SetBroken();

    }

    void SetClosed()
    {
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        isSafe = true;
    }

    void SetBroken()
    {
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        isSafe = false;
    }
}
