using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] BulletType bulletType;
    Rigidbody2D rigidbody;
    SpriteRenderer renderer;
    TrailRenderer trail;
    bool isFlying;

    public bool IsFlying
    {
        get
        {
            return isFlying;
        }

        set
        {
            isFlying = value;
        }
    }

    public delegate void HitSomething(Transform transform);
    public static event HitSomething OnHitsomething;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
    }

    IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(5f);
        Inactive();
    }

    public void Fire(Vector2 _dir)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        IsFlying = true;
        trail.Clear();
        trail.enabled = true;
        rigidbody.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        rigidbody.AddForce(_dir * bulletType.Velocidad_Bala, ForceMode2D.Impulse); // Por esto se necesitan rigidbodies
        StartCoroutine(ReturnAfterTime());
        
    }

    public void Inactive ()
    {
        IsFlying = false;
        trail.enabled = false;
        gameObject.SetActive(false);
        rigidbody.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopCoroutine(ReturnAfterTime());
        Inactive();
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.GetDamage(bulletType.damage);
        }

        if (OnHitsomething != null)
            OnHitsomething(collision.transform);

        
    }

}
