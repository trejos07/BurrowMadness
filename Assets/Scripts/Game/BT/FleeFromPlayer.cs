using UnityEngine;
using UnityEngine.AI;

public class FleeFromPlayer : Task
{
    Transform player;
    [SerializeField] NavMeshAgent agent;

    private void Awake()
    {
        player = FindObjectOfType<Player>().transform;
    }

    public override bool Execute()
    {
        Vector3 dir = TargetAI.transform.position - player.position;
        agent.SetDestination(TargetAI.transform.position + dir);
        return true;
    }
}