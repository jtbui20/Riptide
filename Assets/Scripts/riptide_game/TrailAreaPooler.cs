using System.Collections.Generic;
using UnityEngine;

public class CursorLoopLinePooler : MonoBehaviour
{
    public GameObject lineRendererPrefab;
    List<TrailAreaBehaviour> lines = new List<TrailAreaBehaviour>();

    StaticCreaturesManager staticCreaturesManager;

    void Start()
    {
        staticCreaturesManager = FindAnyObjectByType<StaticCreaturesManager>();
    }

    void Update()
    {
        List<TrailAreaBehaviour> linesToProcess = lines.FindAll(line => line.shouldProcess);
        foreach (TrailAreaBehaviour line in linesToProcess)
        {
            if (line.isSafe && line.shouldProcess)
            {
                foreach (GameObject obj in line.objectsInside)
                {
                    if (obj == null) continue; // Skip if the object is null
                    SelectableObjectBehaviour selectableBehaviour = obj.GetComponent<SelectableObjectBehaviour>();
                    if (selectableBehaviour != null)
                    {
                        if (obj.TryGetComponent(out BasicCreatureBehaviour creatureBehaviour))
                        {
                            staticCreaturesManager.CapturedCreature(creatureBehaviour);
                        }
                        else if (obj.TryGetComponent(out JeepMovementLogic vehicleBehaviour))
                        {
                            // Do something with the vehicle
                        }
                    }
                }
                Debug.Log("Collected " + line.objectsInside.Count + " objects inside the trail area.");
            }
            RemoveLine(line);
        }
    }

    public LineRenderer CreateNewLineRendererInstance()
    {
        LineRenderer newLine = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        lines.Add(newLine.GetComponent<TrailAreaBehaviour>());
        return newLine;
    }

    public void IsolateCurrentLine(LineRenderer lineRenderer, bool isClosed = false)
    {
        // Grab the behaviour
        TrailAreaBehaviour trailBehaviour = lineRenderer.GetComponent<TrailAreaBehaviour>();
        if (trailBehaviour != null)
        {
            trailBehaviour.SetConfirmed(isClosed);
        }
    }

    public void RemoveLine(TrailAreaBehaviour lineRenderer)
    {
        lines.Remove(lineRenderer);
        Destroy(lineRenderer.gameObject);
    }
}
