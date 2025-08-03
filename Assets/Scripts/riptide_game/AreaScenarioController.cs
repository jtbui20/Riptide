using Unity.VisualScripting;
using UnityEngine;

public class AreaScenarioController : MonoBehaviour
{
    // This is the class that the timeline will be calling stuff to
    public JeepMovementLogic jeepObject;
    public CursorSelectionArea cursorObject;
    public StaticCreaturesManager creatureManager;
    public StartingUI startingUI;

    public IsolatedStopwatch stopwatch;

    void Start()
    {
        startingUI.onStartGame += StartGame;
        creatureManager.OnAllCreaturesCaptured += SpawnNextWave;
        stopwatch = new();
    }

    void Update()
    {
        if (stopwatch.isRunning)
        {
            stopwatch.DoTick(Time.deltaTime);
            startingUI.UpdateTimer(stopwatch.GetTimeElapsed());
        }
    }

    public void MoveCharacterToPosition()
    {

    }

    public void SpawnCreaturesSequentially()
    {

    }

    public void SpawnCursor()
    {

    }

    public void StartCountdown()
    {

    }

    public void StartGame()
    {
        cursorObject.EnableCursor();

        creatureManager.EnableCreatures();

        startingUI.UpdateCreatureCount(creatureManager.CreatureCount, creatureManager.MaxCount);

        // Turn on the stop watch
        stopwatch.Start();
    }

    public void PauseGame()
    {

    }

    public void ResumeGame()
    {

    }

    public void SpawnNextWave()
    {
        if (creatureManager.WaveIndex >= creatureManager.SpawnPattern.Count)
        {
            FinishScenario_Success();
            return;
        }
        creatureManager.WaveIndex++;
        creatureManager.SpawnWave();
        startingUI.UpdateCreatureCount(creatureManager.CreatureCount, creatureManager.MaxCount);
        creatureManager.EnableCreatures();
    }

    public void FinishScenario_Success()
    {
        ShowScenarioSummary();
    }

    public void ShowScenarioSummary()
    {
        startingUI.ShowSummaryScreen();
    }


    #region Event Invokers


    #endregion
}