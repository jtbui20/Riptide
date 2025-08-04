using TMPro;
using UnityEngine;

public class GameplayInformationUI : MonoBehaviour
{
    [Header("UI Element References")]
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI creatureCountText;
    [SerializeField]
    TextMeshProUGUI waveCountText;

    void Start()
    {
        BindScenarioInformation();
        BindCreatureManagerEvents();
    }

    public void ShowGameplayUI()
    {
        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        creatureCountText.gameObject.SetActive(true);
        waveCountText.gameObject.SetActive(true);
    }

    public void HideGameplayUI()
    {
        timerText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        creatureCountText.gameObject.SetActive(false);
        waveCountText.gameObject.SetActive(false);
    }

    void BindScenarioInformation()
    {
        AreaScenarioController scenarioController = FindAnyObjectByType<AreaScenarioController>();
        if (scenarioController != null)
        {
            scenarioController.OnScoreAdd += UpdateScore;
            scenarioController.stopwatch.OnTick += UpdateTimer;
        }
    }

    void BindCreatureManagerEvents()
    {
        StaticCreaturesManager creatureManager = FindAnyObjectByType<StaticCreaturesManager>();
        if (creatureManager != null)
        {
            creatureManager.OnCreatureCaptured += UpdateCreatureCount;
            creatureManager.onWaveUpdated += UpdateWaveCount;
        }
    }

    void UpdateTimer(float time)
    {
        timerText.text = "Time: " + Mathf.FloorToInt(time) + "s";
    }

    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateCreatureCount(int current, int max)
    {
        creatureCountText.text = "Creatures: " + current + " / " + max;
    }

    void UpdateWaveCount(int newWaveIndex, int max)
    {
        waveCountText.text = "Wave: " + (newWaveIndex + 1) + " / " + max;
    }
}