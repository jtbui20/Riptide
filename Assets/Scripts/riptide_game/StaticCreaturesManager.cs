using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticCreaturesManager : MonoBehaviour
{
    List<BasicCreatureBehaviour> allCreatures;
    public int CreatureCount => allCreatures.Count;
    public int MaxCount = 0;
    public bool AreAllCreaturesCaptured => allCreatures.Count == 0;

    public event System.Action<int, int> OnCreatureCaptured;
    public void Start()
    {
        TryGetAllCreatures();
        DisableCreatures();
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
                OnCreatureCaptured?.Invoke(MaxCount - allCreatures.Count, MaxCount);
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