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

    private List<Vector3> selectionPath = new List<Vector3>();

    [Header("Cursor Selection Area Settings")]
    public float selectionSensitivity;
    public float closingSensitivity;
    public int minumumSteps;
    public float verticalOffset;
    public GameObject cursorObject;

    List<Vector3> selectionAreaPoints = new List<Vector3>();
    void Start()
    {
        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleLineDrawing();
        MoveCursorObject();
    }

    void HandleLineDrawing()
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

    void MoveCursorObject()
    {
        if (cursorObject != null)
        {
            Vector3 mouseWorldPos = GetMouseWorldPositionProjectedToSurface();
            cursorObject.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y + verticalOffset, mouseWorldPos.z);
        }
    }

    void ProcessPoints()
    {
        foreach (GameObject selectable in GameObject.FindGameObjectsWithTag("Selectable"))
        {
            if (isPointInSelectionArea(selectable.transform.position))
            {
                Debug.Log("Object selected: " + selectable.name);
            }
        }
    }

    bool isPointInSelectionArea(Vector3 point)
    {
        // Use Ray casting algorithm to determine if the point is inside the polygon defined by selectionPath
        int intersections = 0;
        Vector3 rayStart = new Vector2(point.x, point.z - 1000f);
        Vector3 rayEnd = new Vector2(point.x, point.z + 1000f);
        for (int i = 0; i < selectionPath.Count - 1; i++)
        {
            Vector3 segmentStart = new Vector2(selectionPath[i].x, selectionPath[i].z);
            Vector3 segmentEnd = new Vector2(selectionPath[i + 1].x, selectionPath[i + 1].z);
            if (TwoLinesIntersect(segmentStart, segmentEnd, rayStart, rayEnd))
            {
                intersections++;
            }
        }
        Debug.Log("Intersections: " + intersections + " for point: " + point);
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
