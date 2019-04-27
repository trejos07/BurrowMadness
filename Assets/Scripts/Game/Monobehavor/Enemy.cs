using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AICharacter
{
    private Transform Target;                         
    
    
    private bool atacking;

    protected override void Awake()
    {
        base.Awake();   
    }
    protected override void Death()
    {
        base.Death();
    }
    public override void MoveTo(Vector3 _inputPos)
    {
        
    }

    //task for BT
    public IEnumerator DoingDamage()
    {
        atacking = true;
        DoDamage();
        yield return new WaitForSeconds(atackCoolDown);
        atacking = false;
    }
    public virtual void DoDamage()
    {
        IDamagable Damagable = Target.GetComponent<IDamagable>();
        if (Damagable != null)
        {
            Damagable.GetDamage(attackDamage);
        }
    }

    //States
    

    //debug info
    
}
