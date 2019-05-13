using UnityEngine;

public class ShotGun : Weapon
{
    [SerializeField] int nPellets;
    [SerializeField] float spreadAngle;

    protected override void Fire()
    {
        for (int i = 0; i < nPellets; i++)
        {
            Bullet bullet = pool.GetAt(SpawnPos.position, Rigidbody.rotation);
            if (bullet != null)
            {
                float z = Random.Range(-180f,180f);
                bullet.Rigidbody.rotation += ((spreadAngle / 180) * z);
                Debug.Log("disparo");
                canFire = false;
                bullet.Fire(Owner,info.BulletDamageMultiplier);
                Invoke("ResetCD", info.FireRate);
            }
        }
       
    }

}