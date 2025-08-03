using System;
using System.Collections.Generic;
using UnityEngine;

public class CursorLoopLinePooler : MonoBehaviour
{
    public GameObject lineRendererPrefab;
    List<TrailAreaBehaviour> lines = new List<TrailAreaBehaviour>();

    StaticCreaturesManager staticCreaturesManager;

    public int BaseScore = 100;
    public float ScoreMultiplier = 1.25f; // This means I get an additional 25% score for each other creature I capture in the same line
    public int ComboChainIncrement = 10;

    public event Action<int> OnScoreAdd;
    AreaScenarioController areaScenarioController;

    void Start()
    {
        staticCreaturesManager = FindAnyObjectByType<StaticCreaturesManager>();
        areaScenarioController = FindAnyObjectByType<AreaScenarioController>();
    }

    public int comboCount = 0;

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

    public void AddScore(int NumberOfObjects = 1)
    {
        int scoreToAdd = Mathf.FloorToInt(BaseScore * NumberOfObjects * (1 + (NumberOfObjects - 1) * ScoreMultiplier)) + (ComboChainIncrement * comboCount);
        Debug.Log("Adding score: " + scoreToAdd);
        areaScenarioController.AddScore(scoreToAdd);
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
            if (isClosed == true)
            {

                // Grab the number of objects inside the line
                int numberOfObjects = trailBehaviour.objectsInside.Count;
                AddScore(numberOfObjects);
                comboCount++;

                // max of 20 combo count
                AudioManager.Instance.PlayAudioClipPitched(AudioManager.Instance.loopCounterClip, 1f + (Mathf.Min(comboCount, 20) * 0.1f));
            }
            else
            {
                comboCount = 0;

                AudioManager.Instance.PlayAudioClip(AudioManager.Instance.loopBrokenClip);
            }
        }
    }

    public void RemoveLine(TrailAreaBehaviour lineRenderer)
    {
        lines.Remove(lineRenderer);
        Destroy(lineRenderer.gameObject);
    }
}
