using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StaticCreaturesManager : MonoBehaviour
{
    List<BasicCreatureBehaviour> allCreatures;
    [Header("Dump Creatures here")]
    public List<GameObject> creaturePrefabs;
    public int CreatureCount => allCreatures.Count;
    public int MaxCount = 0;
    public bool AreAllCreaturesCaptured => allCreatures.Count == 0;

    public Collider spawnAreaCollider;

    public event System.Action<int, int> OnCreatureCaptured;
    public event System.Action OnAllCreaturesCaptured;

    public int WaveIndex = 0;

    public List<int> SpawnPattern = new List<int> { 1, 1, 2, 2, 3, 3, 4 };
    public void Start()
    {
        // TryGetAllCreatures();
        // SpawnWave();
        // TryGetAllCreatures();
        // DisableCreatures();
    }

    public void SpawnWave()
    {
        List<BasicCreatureBehaviour> creaturesToSpawn = new List<BasicCreatureBehaviour>();
        // Select number of creatures based on pattern
        int numberToSpawn = WaveIndex < SpawnPattern.Count ? SpawnPattern[WaveIndex] : 1;

        for (int i = 0; i < numberToSpawn; i++)
        {
            // Pick a random location within the spawn area
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaCollider.bounds.min.x, spawnAreaCollider.bounds.max.x),
                0, // Assuming y is the height of the spawn area
                Random.Range(spawnAreaCollider.bounds.min.z, spawnAreaCollider.bounds.max.z)
            );

            // Sample to NavMesh to ensure the position is valid
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 50f, NavMesh.AllAreas))
            {
                // Pick a random creature prefab from the list
                GameObject creaturePrefab = creaturePrefabs[Random.Range(0, creaturePrefabs.Count)];
                // Create and initialize the creature
                BasicCreatureBehaviour newCreature = Instantiate(creaturePrefab, hit.position, Quaternion.identity).GetComponent<BasicCreatureBehaviour>();
                creaturesToSpawn.Add(newCreature);
            }
        }

        // Add the newly spawned creatures to the allCreatures list
        if (allCreatures == null)
        {
            allCreatures = new List<BasicCreatureBehaviour>();
        }
        allCreatures.AddRange(creaturesToSpawn);
        MaxCount = allCreatures.Count;
    }

    public void EnableCreatures()
    {
        foreach (var creature in allCreatures)
        {
            creature.IsBehaviourEnabled = true;
        }
    }

    public void DisableCreatures()
    {
        foreach (var creature in allCreatures)
        {
            creature.IsBehaviourEnabled = false;
        }
    }

    public void TryGetAllCreatures()
    {
        allCreatures = FindObjectsByType<BasicCreatureBehaviour>(FindObjectsSortMode.None).ToList();

        if (allCreatures.Count == 0)
        {
            Debug.LogWarning("No creatures found in the scene.");
            return;
        }
        MaxCount = allCreatures.Count;
        OnCreatureCaptured?.Invoke(0, MaxCount);
        Debug.Log("Found " + allCreatures.Count + " creatures in the scene.");
    }

    public void CapturedCreature(BasicCreatureBehaviour creature)
    {
        creature.OnSelected();
        if (creature.isFullySelected)
        {
            if (allCreatures.Contains(creature))
            {
                allCreatures.Remove(creature);
                // Then I need to destroy it
                Destroy(creature.gameObject);
                OnCreatureCaptured?.Invoke(MaxCount - allCreatures.Count, MaxCount);
            }

            if (AreAllCreaturesCaptured)
            {
                Debug.Log("All creatures captured!");
                OnAllCreaturesCaptured?.Invoke();

            }
        }
    }
}