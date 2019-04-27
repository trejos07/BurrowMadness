using UnityEngine;

public class AgroBT: RunBT
{
    [Range(0, 1)] public float IldeChance;
    public override void Execute()
    {
        Transform Player = Owner.LookForPlayerArround();
        if (Player != null)
        {
            base.Execute();
        }
        else
        {
            float randomToIdle = Random.Range(0f, 1f);

            if (IldeChance <= randomToIdle) NextState = transform.GetComponent<PatrolBT>();//patrol
            else NextState = transform.GetComponent<Idle>();

            SwitchToNextState();
        }
    }
}
