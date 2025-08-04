using UnityEngine;
using UnityEngine.AI;

public class BasicCreatureBehaviour : SelectableObjectBehaviour
{

    public bool IsBehaviourEnabled = false;
    NavMeshAgent agent;
    BasicCreatureStates startingState;
    BasicCreatureStates currentState = BasicCreatureStates.Passive;
    GameObject playerTarget;

    public float randomLocationRadius = 50f; // Need to move this to a configuration setting
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = startingState;
    }

    void Update()
    {
        if (!IsBehaviourEnabled) return;
        if (agent.remainingDistance < agent.stoppingDistance || agent.destination == null)
        {
            switch (currentState)
            {
                case BasicCreatureStates.Passive:
                    PassiveIntention();
                    break;
                case BasicCreatureStates.Evasive:
                    EvasiveIntention();
                    break;
                case BasicCreatureStates.AggressiveTrail:
                    AggressiveTrailIntention();
                    break;
                case BasicCreatureStates.AggressiveVehicle:
                    AggressiveVehicleIntention();
                    break;
            }
        }
    }

    void PassiveIntention()
    {
        // Randomly wander around the navmesh surface

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * randomLocationRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, randomLocationRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position + new Vector3(0, 0.04680252f, 0));
    }

    void EvasiveIntention()
    {
        // Move away from the player target if it is within a certain distance
        if (playerTarget != null && Vector3.Distance(transform.position, playerTarget.transform.position) < 10f)
        {
            Vector3 directionAwayFromPlayer = (transform.position - playerTarget.transform.position).normalized;
            Vector3 newDestination = transform.position + directionAwayFromPlayer * 5f;
            agent.SetDestination(newDestination);
        }
    }

    void AggressiveTrailIntention()
    {
        // Chase the player target if it is within a certain distance
        if (playerTarget != null && Vector3.Distance(transform.position, playerTarget.transform.position) < 15f)
        {
            agent.SetDestination(playerTarget.transform.position);
        }
    }

    void AggressiveVehicleIntention()
    {
        // Chase the player target if it is within a certain distance
        if (playerTarget != null && Vector3.Distance(transform.position, playerTarget.transform.position) < 20f)
        {
            agent.SetDestination(playerTarget.transform.position);
        }
    }

    public void OnSelectedByTrailArea()
    {

    }
}
