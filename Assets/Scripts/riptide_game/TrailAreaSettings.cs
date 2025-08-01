using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Trail Area Settings", menuName = "Riptide/TrailAreaSettings", order = 1)]
public class TrailAreaSettings : ScriptableObject
{
    [Header("Cursor Selection Area Settings")]
    public float selectionSensitivity;
    public float closingSensitivity;
    public int minimumSteps;
    public int minimumDistanceToCloseLoop = 2; // Minimum distance to close the loop
    public float verticalOffset;
    public bool allowCutLoops = true; // Allow cutting loops at any point
}
