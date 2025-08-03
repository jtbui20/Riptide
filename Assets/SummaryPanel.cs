using UnityEngine;
using TMPro;

public class SummaryPanel : MonoBehaviour
{

    [Header("Summary UI Elements")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalTimeText;

    [SerializeField]
    private AreaScenarioController areaScenarioController;

    public void ShowSummary()
    {
        float finalTime = areaScenarioController.stopwatch.GetTimeElapsed();
        int timeSeconds = Mathf.FloorToInt(finalTime);
        float msFraction = finalTime - timeSeconds;

        // Update the final score and time text
        finalTimeText.text = $"Time: {timeSeconds}.{(int)(msFraction * 100)} seconds";
        finalScoreText.text = $"Score: {areaScenarioController.currentScore}";

        // Activate the summary panel
        gameObject.SetActive(true);
    }
}
