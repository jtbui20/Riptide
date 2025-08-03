using System.Collections;
using TMPro;
using UnityEngine;

public class StartingUI : MonoBehaviour
{
    [Header("Starting UI Elements")]
    public TextMeshProUGUI readyText;
    public TextMeshProUGUI goText;

    [Header("Gameplay State UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI creatureCountText;

    [Header("Summary UI")]
    public GameObject summaryPanel;
    public AudioSource bgm;

    public event System.Action onStartGame;

    void Start()
    {
        StartCoroutine(ShowBatchStart());
        StaticCreaturesManager creatureManager = FindAnyObjectByType<StaticCreaturesManager>();
        if (creatureManager != null)
        {
            creatureManager.OnCreatureCaptured += UpdateCreatureCount;
        }
    }

    IEnumerator ShowBatchStart()
    {
        // Show "Ready" text
        readyText.gameObject.SetActive(true);
        yield return FadeInText(readyText, 1f); // Fade in over 1 second
        yield return new WaitForSeconds(1f); // Wait for 1 second
        readyText.gameObject.SetActive(false); // Hide "Ready" text
        // Show "Go" text
        goText.gameObject.SetActive(true);
        yield return FadeInText(goText, 1f); // Fade in over 1 second
        yield return new WaitForSeconds(1f); // Wait for 1 second
        goText.gameObject.SetActive(false); // Hide "Go" text

        // Start the game logic
        ActuallyStartTheGame();
    }

    IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        float elapsed = 0f;
        Color color = text.color;
        color.a = 0f;
        text.color = color;
        text.gameObject.SetActive(true);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            text.color = color;
            yield return null;
        }
    }

    void ActuallyStartTheGame()
    {
        onStartGame?.Invoke();
    }

    public void UpdateTimer(float time)
    {
        // Shwo time in seconds
        timerText.text = "Time: " + Mathf.FloorToInt(time) + "s";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateCreatureCount(int current, int max)
    {
        creatureCountText.text = "Creatures: " + current + "/" + max;
    }

    public void ShowSummaryScreen()
    {
        // Hide the gameplay UI
        timerText.gameObject.SetActive(false);
        // scoreText.gameObject.SetActive(false);
        creatureCountText.gameObject.SetActive(false);

        // Show the summary screen
        summaryPanel.SetActive(true);
    }
}