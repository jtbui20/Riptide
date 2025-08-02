using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticCreaturesManager : MonoBehaviour
{
    List<BasicCreatureBehaviour> allCreatures;
    public int CreatureCount => allCreatures.Count;
    public bool AreAllCreaturesCaptured => allCreatures.Count == 0;
    public void Start()
    {
        TryGetAllCreatures();
    }

    public void TryGetAllCreatures()
    {
        allCreatures = FindObjectsByType<BasicCreatureBehaviour>(FindObjectsSortMode.None).ToList();

        if (allCreatures.Count == 0)
        {
            Debug.LogWarning("No creatures found in the scene.");
            return;
        }

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
            }
            if (allCreatures.Count == 0)
            {
                Debug.Log("All creatures have been captured.");
            }
        }

    }
}