using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorSelectionArea : MonoBehaviour
{
    private LineRenderer currentlyManagedLineRenderer;
    private bool isDrawing = false;
    private Vector3 startWorldPos;
    private Camera mainCamera;

    private List<Vector3> selectionPath = new List<Vector3>();

    public TrailAreaSettings cursorSettings;
    public GameObject cursorObject;
    public GameObject lineRendererPrefab;

    CursorLoopLinePooler linePooler;

    private float currentDistance;

    void Start()
    {
        mainCamera = Camera.main;
        linePooler = FindAnyObjectByType<CursorLoopLinePooler>();
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
            if (selectionPath.Count == 0 || Vector3.Distance(mouseWorldPos, selectionPath[selectionPath.Count - 1]) > cursorSettings.selectionSensitivity)
            {
                selectionPath.Add(mouseWorldPos);
                currentDistance += Vector3.Distance(mouseWorldPos, startWorldPos);
                UpdateLineRendererPositions();

                bool isClosedLoop = cursorSettings.allowCutLoops ? TryCloseLoopAtAnyPoint() : TryCloseLoop();

                if (isClosedLoop)
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
            cursorObject.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y + cursorSettings.verticalOffset, mouseWorldPos.z);
        }
    }

    void ProcessPoints()
    {
        foreach (GameObject selectable in GameObject.FindGameObjectsWithTag("Selectable"))
        {
            if (isPointInSelectionArea(selectable.transform.position))
            {
                Debug.Log("Object selected: " + selectable.name);
                SelectableObjectBehaviour selectableBehaviour = selectable.GetComponent<SelectableObjectBehaviour>();
                if (selectableBehaviour != null)
                {
                    selectableBehaviour.OnSelected();
                }
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
        DetachLineRenderer();
    }

    bool TryCloseLoop()
    {
        if (selectionPath.Count < cursorSettings.minimumSteps) return false;
        if (currentDistance < cursorSettings.minimumDistanceToCloseLoop) return false;

        if (Vector3.Distance(selectionPath[0], selectionPath[^1]) <= cursorSettings.closingSensitivity)
        {
            selectionPath.Add(selectionPath[0]); // Close the loop by adding the first point again
            UpdateLineRendererPositions();
            return true;
        }
        return false;
    }

    bool TryCloseLoopAtAnyPoint()
    {
        if (selectionPath.Count < cursorSettings.minimumSteps) return false;
        if (currentDistance < cursorSettings.minimumDistanceToCloseLoop) return false;

        for (int i = 0; i < selectionPath.Count - cursorSettings.minimumSteps; i++)
        {
            if (Vector3.Distance(selectionPath[i], selectionPath[^1]) <= cursorSettings.closingSensitivity)
            {
                // Delete everything before this point
                selectionPath.RemoveRange(0, i);
                selectionPath.Add(selectionPath[0]); // Close the loop by adding the first point again
                UpdateLineRendererPositions();
                return true;
            }
        }
        return false;
    }

    void StartDrawing()
    {
        startWorldPos = GetMouseWorldPositionProjectedToSurface();
        selectionPath.Clear();
        selectionPath.Add(startWorldPos);
        currentDistance = 0f;
    }

    void ConfirmSelectionLoop()
    {
        // Extract the selection area points and store in a list
        ProcessPoints();
        DetachLineRenderer();
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

    #region Line Methods
    public void UpdateLineRendererPositions()
    {
        if (!isDrawing) return;
        if (currentlyManagedLineRenderer == null)
        {
            CreateNewLineRenderer();
        }
        currentlyManagedLineRenderer.enabled = true;
        currentlyManagedLineRenderer.positionCount = selectionPath.Count;
        currentlyManagedLineRenderer.SetPositions(selectionPath.ToArray());
    }

    public void CreateNewLineRenderer()
    {
        if (currentlyManagedLineRenderer != null)
        {
            DetachLineRenderer();
        }
        currentlyManagedLineRenderer = linePooler.CreateNewLineRendererInstance();
    }

    public void DetachLineRenderer()
    {
        linePooler.IsolateCurrentLine(currentlyManagedLineRenderer);
        currentlyManagedLineRenderer = null;
    }
    #endregion
}
