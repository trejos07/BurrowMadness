using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Character : MonoBehaviour, IDamagable
{
    [SerializeField] protected new string name;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float atackCoolDown;
    [SerializeField] private float detectRadius;
    [SerializeField] protected float attackRadius;
    [SerializeField] protected float movSpeed;

    //[SerializeField] private Bullet bullet;
    //[SerializeField] private float shootForce = 20F;
    //[SerializeField] private Transform bulletSpawnPosition;
    //blic float ShootForce { get { return shootForce; } }

    public delegate void IntValueChange(int i);
    public delegate void FloatValueChange(float f);

    public event Action OnDeath;
    public event Action OnAttack;
    public event IntValueChange OnGetDamage;
    public event FloatValueChange OnHealthChange;

    private int hp;
    private new Rigidbody2D rigidbody;


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

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start()
    {
        HP = maxHealth;
    }
    protected virtual void Death()
    {
        if (OnDeath != null) OnDeath();
        Destroy(gameObject);
    }
    public abstract void MoveTo(Vector3 _inputPos);
    public virtual void Atack(IDamagable damagable)
    {
        damagable.GetDamage(attackDamage);
        if (OnAttack != null)
            OnAttack();
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

