using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Scoring Config", menuName = "Riptide/ScoringConfig", order = 1)]
public class ScoringConfig : ScriptableObject
{
    public int BaseScore = 100; // This is configuration details
    public float ScoreMultiplier = 1.25f; // This means I get an additional 25% score for each other creature I capture in the same line
    public int ComboChainIncrement = 10;

    public int CalculateScore(int NumberOfObjects, int ComboCount)
    {
        int scoreToAdd = Mathf.FloorToInt(BaseScore * NumberOfObjects * (1 + (NumberOfObjects - 1) * ScoreMultiplier)) + (ComboChainIncrement * ComboCount);
        Debug.Log("Adding score: " + scoreToAdd);
        return scoreToAdd;
    }
}
