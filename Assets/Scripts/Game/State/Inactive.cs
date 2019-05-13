using UnityEngine;

public class Inactive : State
{
    [Range(0, 1)] public float IldeChance;
    float t;

    public override void Execute()
    {
        t += StateCheckTime;
        Transform Player = Owner.LookForPlayerArround(50);

        if (Player != null)
        {
            StateCheckTime = 0.1f;
            SwitchToNextState();
            return;
        }
        else if (Owner.Active)
        {
            if (t > 10)
            {
                t = 0;
                Teleport();
            }
        }
        
        
    }

    void Teleport()
    {
        Vector3 rPos = Owner.Rigidbody.position;
        Vector3 nPos = Owner.Agent.GetRandomPointArround(rPos, 10);
        if (rPos == nPos) return;
        Owner.Rigidbody.position = nPos;
    }

}