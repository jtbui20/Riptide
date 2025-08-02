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
        stopwatch = new();
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

        // Turn on the stop watch
        stopwatch.Start();
    }

    public void PauseGame()
    {

    }

    public void ResumeGame()
    {

    }

    public void FinishScenario_Success()
    {

    }

    public void ShowScenarioSummary()
    {

    }


    #region Event Invokers


    #endregion
}