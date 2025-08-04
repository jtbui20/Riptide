using UnityEngine;

public class AreaScenarioController : MonoBehaviour
{
    // This is the class that the timeline will be calling stuff to
    public CursorSelectionArea cursorObject;
    public StaticCreaturesManager creatureManager;
    public SummaryPanel summaryPanel;

    public IsolatedStopwatch stopwatch;
    public int currentScore;

    public event System.Action<int> OnScoreAdd;

    void Start()
    {
        creatureManager.OnAllCreaturesCaptured += SpawnNextWave;
        stopwatch = new();
    }

    void Update()
    {
        if (stopwatch.isRunning)
        {
            stopwatch.DoTick(Time.deltaTime);
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
        // This is now handled by the timeline
    }

    public void StartGame()
    {
        cursorObject.EnableCursor();

        creatureManager.EnableCreatures();

        // Event hook will now make this happen
        // startingUI.UpdateCreatureCount(creatureManager.CreatureCount, creatureManager.MaxCount);

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
        creatureManager.SpawnWave();
        creatureManager.DisableCreatures();
    }

    public void SpawnNextWave()
    {
        if (creatureManager.WaveIndex >= creatureManager.NumberOfWaves - 1)
        {
            FinishScenario_Success();
            return;
        }
        creatureManager.IncrementWaveIndex();
        creatureManager.SpawnWave();
        creatureManager.EnableCreatures();
    }

    public void FinishScenario_Success()
    {
        AudioManager.Instance.PlayAudioClip(AudioManager.Instance.victoryClip);
        ShowScenarioSummary();
    }

    public void ShowScenarioSummary()
    {
        summaryPanel.ShowSummary();
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        OnScoreAdd?.Invoke(currentScore);
    }


    #region Event Invokers


    #endregion
}