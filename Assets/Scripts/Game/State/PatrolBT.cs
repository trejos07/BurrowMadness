using UnityEngine;

public class PatrolBT: RunBT
{
    [Range(0, 1)] public float IldeChance;

    public override void Execute()
    {
        Transform Player = Owner.LookForPlayerArround();
        if (Player != null)
        {
            NextState = transform.GetComponent<AgroBT>();
            SwitchToNextState();
        }
        else
        {
            float randomToIdle = Random.Range(0f, 1f);

            if (IldeChance <= randomToIdle) base.Execute();
            else
            {
                NextState = transform.GetComponent<Idle>();
                SwitchToNextState();
            }
        }
    }

}