using UnityEngine.AI;
using UnityEngine;

public class MoveToDistance : Task
{
     
    [SerializeField] AstarAgent agent;
    [SerializeField] float stopDistance;


    public override bool Execute()
    {
        Transform player = TargetAI.LookForPlayerArround(); 
        float d = Vector3.Distance(TargetAI.transform.position, player.position);
        if (d<=stopDistance)
        {
            agent.IsStopped = true;
            return false;
        }
        else
        {
            agent.IsStopped = false;
            agent.SetDestination(player.position);
            return true;
        }
    }
}