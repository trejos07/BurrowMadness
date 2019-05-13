using UnityEngine;

public class PatrolBT: RunBT
{
    [Range(0, 1)] public float IldeChance;

    public override void Execute()
    {
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

            if (IldeChance <= randomToIdle) base.Execute();
            else
            {
                NextState = Owner.GetState("Idle");
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