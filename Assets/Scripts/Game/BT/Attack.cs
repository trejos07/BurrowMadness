using UnityEngine;

public class Attack : Task
{
    [SerializeField] Character character;
    [SerializeField] float cD;
    bool canAttack = true;

    public override bool Execute()
    {
        if (canAttack == true)
        {
            canAttack = false;
            //character.SpawnBullet();
            Invoke("resetCD", cD);
            return true;
        }
        else
            return false;
        
    }

    void resetCD()
    {
        canAttack = true;
    }
}