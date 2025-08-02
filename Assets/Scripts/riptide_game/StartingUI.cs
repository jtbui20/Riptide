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

    void UpdateTimer(float time)
    {
        timerText.text = "Time: " + time.ToString("F2") + "s";
    }

    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateCreatureCount(int current, int max)
    {
        creatureCountText.text = "Creatures: " + current + "/" + max;
    }
}