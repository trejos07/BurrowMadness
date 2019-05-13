using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private static List<Bullet> types;

    [SerializeField] private new string name;
    [SerializeField] private Sprite bSprite;
    [SerializeField] private ParticleSystem bparticle;
    [SerializeField] private bool isAnimated;
    [SerializeField] private int baseDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Vector3 size;
    [SerializeField]protected Color color;
    private float damageMultiplier=1;


    private Character instigator;
    private Rigidbody2D m_rigidbody;
    private Collider2D m_collider;
    private Collider2D i_collider;
    private SpriteRenderer m_renderer;
    private TrailRenderer trail;
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
            Trail.Clear();
        }
    }
    public Character Instigator
    {
        get
        {
            return instigator;
        }

        set
        {
            instigator = value;
            if (value != null)
                i_collider = value.GetComponent<Collider2D>();

        }
    }
    public int ID
    {
        get
        {
            return Types.IndexOf(this);
        }
    }
    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }
    public int Damage
    {
        get
        {
            return Mathf.FloorToInt(baseDamage*damageMultiplier);
        }
    }
    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }

        set
        {
            bulletSpeed = value;
        }
    }

    public Rigidbody2D Rigidbody
    {
        get
        {
            return m_rigidbody;
        }
    }

    public Collider2D Collider
    {
        get
        {
            return m_collider;
        }
    }

    public TrailRenderer Trail
    {
        get
        {
            return trail;
        }

        set
        {
            trail = value;
        }
    }

    public static List<Bullet> Types
    {
        get
        {
            if (types == null)
            {
                types = new List<Bullet>(Resources.LoadAll<Bullet>("Prefabs/Bullets"));
            }
            return types;
        }
    }

    public delegate void HitSomething(Transform transform);
    public delegate void BulletAction(Bullet b);
    public static event BulletAction OnInactive;
    public event HitSomething OnHitsomething;
    public event HitSomething OnDamageSomething;

    private void Awake()
    {
        
        m_renderer = GetComponent<SpriteRenderer>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        trail = GetComponent<TrailRenderer>();
        m_renderer.sprite = bSprite;
        m_renderer.color = color;
        transform.localScale = size *0.1f;
    }
    IEnumerator InactiveByTime()
    {
        yield return new WaitForSeconds(5f);
        Inactive();
    }
    public virtual void Fire(Character _instigator,float damageMulti)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        Instigator = _instigator;
        Physics2D.IgnoreCollision(m_collider,i_collider);
        IsFlying = true;
        damageMultiplier = damageMulti;
        trail.enabled = true;
        m_rigidbody.velocity = Vector3.zero;
        trail.Clear();
        m_rigidbody.AddRelativeForce(transform.right * BulletSpeed, ForceMode2D.Impulse);
        StartCoroutine(InactiveByTime());
        
    }
    public void Inactive ()
    {
        IsFlying = false;
        trail.enabled = false;
        trail.Clear();
        gameObject.SetActive(false);
        m_rigidbody.velocity = Vector2.zero;
        damageMultiplier = 1;
        if(Instigator!=null)
        {
            Physics2D.IgnoreCollision(m_collider, i_collider, false);
            Instigator = null;
        }
        if (OnInactive != null)
            OnInactive(this);
        
    }
    protected virtual void DamageTarget(GameObject target)
    {
        IDamagable damagable = target.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.GetDamage(Damage);
            damageMultiplier = 1;
            if (OnDamageSomething != null)
                OnDamageSomething(target.transform);
        }
       
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsFlying)
        {
            StopCoroutine(InactiveByTime());
            DamageTarget(collision.gameObject);
            
            if (OnHitsomething != null)
                OnHitsomething(collision.transform);
            Inactive();
        }
    }
   
}
