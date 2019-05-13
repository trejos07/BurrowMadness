using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Character : MonoBehaviour, IDamagable
{
    [SerializeField] private new string name;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float atackCoolDown;
    [SerializeField] private float detectRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float movSpeed;
    [SerializeField] protected Weapon weapon;

    //[SerializeField] private Bullet bullet;
    //[SerializeField] private float shootForce = 20F;
    //[SerializeField] private Transform bulletSpawnPosition;
    //blic float ShootForce { get { return shootForce; } }

    public delegate void IntValueChange(int i);
    public delegate void CharacterInteraction(Character character);
    public delegate void FloatValueChange(float f);

    public event Action OnDeath;
    public static event CharacterInteraction OnCharacterDeath;
    public event Action OnAttack;
    public event IntValueChange OnGetDamage;
    public event FloatValueChange OnHealthChange;

    private int hp;
    private new Rigidbody2D rigidbody;
    private Collider2D m_collider;
    SpriteRenderer m_spriteRender;

    public int HP
    {
        get { return hp; }
        protected set {
            hp = value;
            if (OnHealthChange != null)
                OnHealthChange((float)HP / maxHealth);
        }
    }
    public Rigidbody2D Rigidbody
    {
        get
        {
            return rigidbody;
        }

        set
        {
            rigidbody = value;
        }
    }
    public float DetectRadius
    {
        get
        {
            return detectRadius;
        }
    }
    public float MovSpeed
    {
        get
        {
            return movSpeed;
        }
    }
    public string Name
    {
        get
        {
            return name;
        }
    }
    public Collider2D Collider
    {
        get
        {
            return m_collider;
        }
    }
    public float AttackRadius
    {
        get
        {
            return attackRadius;
        }
    }
    public SpriteRenderer SpriteRender
    {
        get
        {
            return m_spriteRender;
        }

        set
        {
            m_spriteRender = value;
        }
    }

    protected virtual void Awake()
    {
        m_spriteRender = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        Transform gunPivot = transform.Find("GunPivot");
        if(gunPivot!= null)
            weapon =gunPivot.GetComponentInChildren<Weapon>();
    }
    protected virtual void Start()
    {
        HP = maxHealth;
    }
    protected virtual void Death()
    {
        if (OnDeath != null) OnDeath();
        if (OnCharacterDeath != null) OnCharacterDeath(this);
        //Destroy(gameObject);
    }
    public abstract void MoveTo(Vector2 _inputPos);
    public virtual void Atack(Character damagable)
    {
        if (weapon != null)
        {
            weapon.FireTrigger();
        }
        else
        {
            damagable.GetDamage(attackDamage);
            if (OnAttack != null)
                OnAttack();
        }
        
        
    }
    public void GetDamage(int _Damage)
    {
        if (OnGetDamage != null)
            OnGetDamage(_Damage);

        if ((HP-_Damage) <= 0F)
        {
            HP = 0;
            Death();
        }
        else
            HP -= _Damage;
    }
    
}

