using UnityEngine.AI;

public static class NavMeshAgentExtensions
{
    public static bool ReachedDestinationOrGaveUp(this NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending == false)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (navMeshAgent.hasPath == false || navMeshAgent.velocity.sqrMagnitude == 0f) 
                {
                    return true; 
                }
            }
        }

        return false; 
    }
}