using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Rigidbody rb;
    [SerializeField]
    private bool isCursorEnabled = false;

    void Start()
    {
        mainCamera = Camera.main;
        linePooler = FindAnyObjectByType<CursorLoopLinePooler>();
        rb = GetComponent<Rigidbody>();
    }

    public void EnableCursor()
    {
        isCursorEnabled = true;
    }

    public void DisableCursor()
    {
        isCursorEnabled = false;
    }

    void Update()
    {
        if (!isCursorEnabled) return;
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
            rb.MovePosition(new Vector3(mouseWorldPos.x, mouseWorldPos.y + cursorSettings.verticalOffset + 1f, mouseWorldPos.z));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        Debug.Log("Collision with: " + collidedObject.name);
        // Check if the collided object is in the disruptable layers
        if ((collidedObject.layer & (1 << cursorSettings.disrutpableLayers)) != 0)
        {
            OnMouseUp();
        }
    }

    void OnMouseDown()
    {
        if (!isCursorEnabled) return;
        isDrawing = true;
        StartDrawing();
    }

    void OnMouseUp()
    {
        if (!isCursorEnabled || !isDrawing) return;
        isDrawing = false;
        DetachLineRenderer(false);
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
        DetachLineRenderer(true);
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

        return Vector3.zero;
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
            DetachLineRenderer(true);
        }
        currentlyManagedLineRenderer = linePooler.CreateNewLineRendererInstance();
    }

    public void DetachLineRenderer(bool isClosed)
    {
        if (currentlyManagedLineRenderer == null) return;
        linePooler.IsolateCurrentLine(currentlyManagedLineRenderer, isClosed);
        currentlyManagedLineRenderer = null;
    }
    #endregion
}
