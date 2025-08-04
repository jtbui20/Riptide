using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Scenario Config", menuName = "Riptide/ScenarioConfig", order = 1)]
public class ScenarioConfig : ScriptableObject
{
    public List<int> SpawnPattern = new List<int> { 1, 1, 2, 2, 3, 3, 4 };
    public List<GameObject> CreaturePrefabs;
}
