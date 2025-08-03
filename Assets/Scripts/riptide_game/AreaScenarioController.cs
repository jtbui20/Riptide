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
    public int currentScore;

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
        startingUI.DoCountdown();
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

    public void SpawnFirstWave()
    {
        creatureManager.WaveIndex = 0;
        creatureManager.SpawnWave();
        startingUI.UpdateCreatureCount(creatureManager.CreatureCount, creatureManager.MaxCount);
        startingUI.UpdateWaveCount(creatureManager.WaveIndex, creatureManager.SpawnPattern.Count);
        creatureManager.DisableCreatures();
    }

    public void SpawnNextWave()
    {
        if (creatureManager.WaveIndex >= creatureManager.SpawnPattern.Count - 1)
        {
            FinishScenario_Success();
            return;
        }
        creatureManager.IncrementWaveIndex();
        creatureManager.SpawnWave();
        startingUI.UpdateCreatureCount(creatureManager.CreatureCount, creatureManager.MaxCount);
        creatureManager.EnableCreatures();
    }

    public void FinishScenario_Success()
    {
        AudioManager.Instance.PlayAudioClip(AudioManager.Instance.victoryClip);
        ShowScenarioSummary();
    }

    public void ShowScenarioSummary()
    {
        startingUI.ShowSummaryScreen();
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        startingUI.UpdateScore(currentScore);
    }


    #region Event Invokers


    #endregion
}