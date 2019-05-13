using UnityEngine;

public class Attack : Task
{
    [SerializeField] float cD;
    bool canAttack = true;

    public override bool Execute()
    {
        if (canAttack == true)
        {
            Transform player = TargetAI.LookForPlayerArround(TargetAI.AttackRadius);
            if(player !=null)
            {
                Player p = player.GetComponent<Player>();
                if (p != null)
                {
                    canAttack = false;
                    TargetAI.Atack(p);
                    Invoke("resetCD", cD);
                    return true;
                }
            }
            return false;
        }
        else
            return false;
    }

    void resetCD()
    {
        canAttack = true;
    }
}