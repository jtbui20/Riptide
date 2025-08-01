using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CursorLoopLinePooler : MonoBehaviour
{
    public GameObject lineRendererPrefab;
    List<TrailAreaBehaviour> lines = new List<TrailAreaBehaviour>();

    void Start()
    {
    }

    void Update()
    {
        List<TrailAreaBehaviour> linesToRemove = lines.FindAll(line => line.shouldProcess);
        foreach (TrailAreaBehaviour line in linesToRemove)
        {
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
