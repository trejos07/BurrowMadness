using UnityEngine;

public class MachineGun : Weapon
{
    public MachineGun()
    {
    }

    protected override void RotateCanon(Vector2 _lookPos)
    {
        base.RotateCanon(_lookPos);
        if (_lookPos.magnitude != 0)
        {
            FireTrigger();
        }
    }

}
