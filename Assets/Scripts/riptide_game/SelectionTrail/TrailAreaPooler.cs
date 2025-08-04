using System.Collections.Generic;
using UnityEngine;

public class CursorLoopLinePooler : MonoBehaviour
{
    // Generator prefab
    public GameObject lineRendererPrefab;
    List<TrailAreaBehaviour> lines = new List<TrailAreaBehaviour>();
    StaticCreaturesManager staticCreaturesManager; // Needs to be a one way relationship

    [SerializeField]
    ScoringConfig scoringConfig; // Can probably load in from the scenario config
    AreaScenarioController areaScenarioController; // Must be a function to tell the scenario manager to add score

    public int comboCount = 0; // If I move score handling to the scenario controller, this can be removed.

    // ! Temporary
    public TrailAreaSettings settings;

    void Start()
    {
        staticCreaturesManager = FindAnyObjectByType<StaticCreaturesManager>();
        areaScenarioController = FindAnyObjectByType<AreaScenarioController>();
    }

    void Update()
    {
        List<TrailAreaBehaviour> linesToProcess = lines.FindAll(line => line.shouldProcess);
        foreach (TrailAreaBehaviour line in linesToProcess)
        {
            if (line.shouldProcess)
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
                        // else if (obj.TryGetComponent(out JeepMovementLogic vehicleBehaviour))
                        // {
                        //     // Do something with the vehicle
                        // }
                    }
                }
                Debug.Log("Collected " + line.objectsInside.Count + " objects inside the trail area.");
            }
            RemoveLine(line);
        }
    }

    public void AddScore(int NumberOfObjects = 1)
    {
        int scoreToAdd = scoringConfig.CalculateScore(NumberOfObjects, comboCount);
        areaScenarioController.AddScore(scoreToAdd);
    }

    public LineRenderer CreateNewLineRendererInstance()
    {
        LineRenderer newLine = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        TrailAreaBehaviour trailAreaBehaviour = newLine.GetComponent<TrailAreaBehaviour>();
        trailAreaBehaviour.LoadSettings(FindAnyObjectByType<TrailAreaSettings>());
        lines.Add(trailAreaBehaviour);
        return newLine;
    }

    public void IsolateCurrentLine(LineRenderer lineRenderer, bool isClosed = false)
    {
        // Grab the behaviour
        TrailAreaBehaviour trailBehaviour = lineRenderer.GetComponent<TrailAreaBehaviour>();
        if (trailBehaviour == null) return;
        if (isClosed == true)
        {
            trailBehaviour.objectsInside = trailBehaviour.FindObjectsInArea();
            int numberOfObjects = trailBehaviour.objectsInside.Count;
            if (numberOfObjects > 0)
            {
                AddScore(numberOfObjects);
                comboCount++;
                trailBehaviour.SetConfirmed(true);
                AudioManager.Instance.PlayAudioClipPitched(AudioManager.Instance.loopCounterClip, 1f + (Mathf.Min(comboCount, 20) * 0.1f));
                return;
            }
        }
        else
        {
            comboCount = 0;
        }

        trailBehaviour.SetConfirmed(false);
        AudioManager.Instance.PlayAudioClip(AudioManager.Instance.loopBrokenClip);
    }

    public void RemoveLine(TrailAreaBehaviour lineRenderer)
    {
        lines.Remove(lineRenderer);
        Destroy(lineRenderer.gameObject);
    }
}
