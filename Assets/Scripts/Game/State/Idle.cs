using UnityEngine;

public class Idle : State
{
    [Range(0, 1)] public float IldeChance;
    public override void Execute()
    {
        StateCheckTime = Random.Range(3f,5f);
        Transform Player = Owner.LookForPlayerArround(50);

        if (Player != null)
        {
            Player = Owner.LookForPlayerArround(Owner.DetectRadius);

            if (Player != null)
            {
                NextState = Owner.GetState("AgroBT");
                SwitchToNextState();
                return;
            }

            float randomToIdle = Random.Range(0f, 1f);

            if (IldeChance <= randomToIdle)
            {
                NextState = Owner.GetState("PatrolBT");
                SwitchToNextState();
                return;
            }
            
        }
        else
        {
            NextState = Owner.GetState("Inactive");
            SwitchToNextState();
            return;
        }
       
    }
}