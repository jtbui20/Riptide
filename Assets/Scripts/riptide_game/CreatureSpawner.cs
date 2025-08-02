using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureSpawner : MonoBehaviour
{
    public List<BasicCreatureBehaviour> creatures;


    List<BasicCreatureBehaviour> ManagedCreatures;

    public void SpawnACreature()
    {
        // Pick a creature that is in creatures, but not in managed creatures

    }

    public void SpawnCreatures()
    {
        ManagedCreatures = new List<BasicCreatureBehaviour>();
        foreach (BasicCreatureBehaviour creature in creatures)
        {
            if (creature == null) continue;
            GameObject creatureObject = Instantiate(creature.gameObject, GetRandomSpawnPosition(), Quaternion.identity);
            BasicCreatureBehaviour creatureBehaviour = creatureObject.GetComponent<BasicCreatureBehaviour>();

            // Start the creature's behaviour
            creatureBehaviour.IsBehaviourEnabled = true;
            ManagedCreatures.Add(creatureBehaviour);
        }
    }

    public Vector3 GetRandomSpawnPosition()
    {
        // Find a position on the NavMesh
        Vector3 randomPosition = new Vector3(
            Random.Range(-50f, 50f),
            0f, // Assuming the ground is at y = 0
            Random.Range(-50f, 50f)
        );

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If no valid position was found, return a default value
        return Vector3.zero;
    }
}
