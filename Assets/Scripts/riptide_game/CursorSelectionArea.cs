using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class CursorSelectionArea : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool isDrawing = false;
    private Vector3 startWorldPos;
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<Vector3> selectionPath = new List<Vector3>();

    [Header("Cursor Selection Area Settings")]
    public float selectionSensitivity; // Distance threshold to consider a point in the selection area
    public float closingSensitivity; // Distance threshold to close the selection area
    public int minumumSteps;
    public float verticalOffset; // Offset to project the selection area vertically

    List<Vector3> selectionAreaPoints = new List<Vector3>();
    void Start()
    {
        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (isDrawing)
        {
            Vector3 mouseWorldPos = GetMouseWorldPositionProjectedToSurface();
            // Only register the point if it is significantly different from the last point
            if (selectionPath.Count == 0 || Vector3.Distance(mouseWorldPos, selectionPath[selectionPath.Count - 1]) > selectionSensitivity)
            {
                selectionPath.Add(mouseWorldPos);
                lineRenderer.positionCount = selectionPath.Count;
                lineRenderer.SetPositions(selectionPath.ToArray());
                lineRenderer.enabled = true;


                if (TryCloseLoop())
                {
                    ConfirmSelectionLoop();
                }
            }
        }
    }

    void ProcessPoints()
    {
        // Iterate over all objects with tag "Selectable"
        foreach (GameObject selectable in GameObject.FindGameObjectsWithTag("Selectable"))
        {
            // Check if the object is within the selection area
            if (isPointInSelectionArea(selectable.transform.position))
            {
                // Perform the desired action on the selectable object
                Debug.Log("Object selected: " + selectable.name);
            }
        }
    }

    bool isPointInSelectionArea(Vector3 point)
    {
        int intersections = 0;
        Vector3 rayStart = new Vector2(point.x, point.z);
        Vector3 rayEnd = new Vector2(point.x, point.z + 1000f);
        Ray ray = new Ray(rayStart, rayEnd - rayStart);

        for (int i = 0; i < selectionPath.Count - 1; i++)
        {
            Vector3 segmentStart = new Vector2(selectionPath[i].x, selectionPath[i].z);
            Vector3 segmentEnd = new Vector2(selectionPath[i + 1].x, selectionPath[i + 1].z);
            if (LineIntersectsRay(segmentStart, segmentEnd, ray))
            {
                intersections++;
            }
        }

        Debug.Log("Intersections: " + intersections + " for point: " + point);
        // If the number of intersections is odd, the point is inside the polygon
        return intersections % 2 == 0;
    }

    bool LineIntersectsRay(Vector3 lineStart, Vector3 lineEnd, Ray ray)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        Vector3 rayDirection = ray.direction;
        Vector3 rayOrigin = ray.origin;
        Vector3 lineToRay = rayOrigin - lineStart;
        float crossProduct = Vector3.Cross(lineDirection, rayDirection).magnitude;
        if (crossProduct < 0.0001f) return false; // Lines are parallel
        float t = Vector3.Cross(lineToRay, rayDirection).magnitude / crossProduct;
        if (t < 0 || t > 1) return false; // Intersection is outside the line segment
        float u = Vector3.Cross(lineToRay, lineDirection).magnitude / crossProduct;
        return u >= 0 && u <= 1; // Intersection is within the ray
    }

    void OnMouseDown()
    {
        isDrawing = true;
        StartDrawing();
    }

    void OnMouseUp()
    {
        isDrawing = false;
    }

    bool TryCloseLoop()
    {
        // Check if the previous point is close enough to the first point to close the loop
        if (selectionPath.Count > minumumSteps && Vector3.Distance(selectionPath[0], selectionPath[selectionPath.Count - 1]) <= closingSensitivity)
        {
            selectionPath.Add(selectionPath[0]); // Close the loop by adding the first point again
            lineRenderer.positionCount = selectionPath.Count;
            lineRenderer.SetPositions(selectionPath.ToArray());
            return true;
        }
        return false;
    }

    void StartDrawing()
    {
        startWorldPos = GetMouseWorldPositionProjectedToSurface();
        selectionPath.Clear();
        selectionPath.Add(startWorldPos);
    }

    void ConfirmSelectionLoop()
    {
        // Extract the selection area points and store in a list
        selectionAreaPoints = new List<Vector3>(selectionPath);
        ProcessPoints();
        StartDrawing();
    }

    private Vector3 GetMouseWorldPositionProjectedToSurface()
    {
        Vector3 mouseWorldPos = InputSystem.GetDevice<Mouse>().position.ReadValue();
        // Find the nearest hit that lands on the layer "surface"
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(mouseWorldPos), out hit, Mathf.Infinity, LayerMask.GetMask("Surface")))
        {
            return hit.point;
        }

        return Vector3.zero; // Return zero if no hit detected
    }
}
