using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandom : Task
{
    [SerializeField] AstarAgent agent;
    public override bool Execute()
    {
        if(agent.IsStopped)
        {
            agent.MoveRandom();
            return true;
        }
        return false;
    }
}
