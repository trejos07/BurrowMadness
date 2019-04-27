using UnityEngine;

public class Idle : State
{
    [Range(0, 1)] public float IldeChance;
    public override void Execute()
    {
        StateCheckTime = Random.Range(3f,5f);
        Transform Player = Owner.LookForPlayerArround();

        if (Player != null)
        {
            NextState = transform.GetComponent<AgroBT>();//agro
            //StartCoroutine(Agent.LookForPathTo(Player.position));
            //path = Agent.GetPathCornersInWorld();
        }
        else
        {
            float randomToIdle = Random.Range(0f, 1f);

            if (IldeChance <= randomToIdle) NextState = transform.GetComponent<PatrolBT>();//patrol
            else return;

        }
        SwitchToNextState();
    }
}