using UnityEngine;

public class AgroBT: RunBT
{
    [Range(0, 1)] public float IldeChance;
    public override void Execute()
    {
        Transform Player = Owner.LookForPlayerArround(Owner.DetectRadius);
        if (Player != null)
        {
            base.Execute();
        }
        else
        {
            float randomToIdle = Random.Range(0f, 1f);
            
            if (IldeChance <= randomToIdle) NextState = Owner.GetState("PatrolBT"); //patrol
            else NextState = NextState = Owner.GetState("Idle"); 

            SwitchToNextState();
        }
    }
}
