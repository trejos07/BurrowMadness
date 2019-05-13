using UnityEngine;

public class HeavyBullet : Bullet
{
    [SerializeField] LayerMask inmune;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsFlying)
        {
            if (collision.GetComponent<Character>())
                DamageTarget(collision.gameObject);
            else
            {
                base.OnTriggerEnter2D(collision);
            }
        }
    }

}