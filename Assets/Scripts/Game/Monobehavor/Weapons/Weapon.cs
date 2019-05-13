using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Weapon : MonoBehaviour
{
    private static List<Weapon> types;

    public WeaponInfo info;
    [SerializeField] private Transform spawnPos;
    [SerializeField] protected float detectionRadius;          
    [SerializeField] protected LayerMask targetDetectionLayer;
    [SerializeField] private new SpriteRenderer renderer;
    protected Character owner;
    protected Character target;
    protected BulletPool pool;
    protected bool canFire =true;
    protected bool fire =false;
    protected bool autoAiming = false;
    private bool active;
    private bool suscribedImputEvents;
    Rigidbody2D m_rigidbody;

    public static List<Weapon> Types
    {
        get
        {
            if (types == null)
            {
                types = new List<Weapon>(Resources.LoadAll<Weapon>("Prefabs/Weapons"));
                for (int i = 0; i < Weapon.types.Count; i++)
                {
                    types[i] = Instantiate(types[i], Vector2.one * 200, Quaternion.identity,GameManager.Instance.transform);
                    //types[i].gameObject.name = types[i].info.name;
                }
                
            }
            return types;
        }
    }
    public int ID
    {
        get
        {
            return Types.IndexOf(types.Where(x=>x.info.name==info.name).First());
        }
    }
    public Character Owner
    {
        get
        {
            return owner;
        }

        set
        {
            owner = value;
        }
    }
    public bool AutoAiming
    {
        get
        {
            return autoAiming;
        }
        set
        {
            autoAiming = value;
            if (value)
                InvokeRepeating("SerchTarget", 0, 0.5f);
            else
                CancelInvoke("SerchTarget");
        }
    }
    public bool CanFire
    {
        get
        {
            return canFire;
        }
    }
    public Transform SpawnPos
    {
        get
        {
            return spawnPos;
        }

        set
        {
            spawnPos = value;
        }
    }
    public Rigidbody2D Rigidbody
    {
        get
        {
            return m_rigidbody;
        }

        set
        {
            m_rigidbody = value;
        }
    }
    public bool Active
    {
        get
        {
            return active;
        }

        set
        {
            active = value;
            if (value)
            {
                //GameManager.Instance.OnGamplay &&
                if ( !suscribedImputEvents)
                {
                    suscribedImputEvents = true;
                    ButtonJoystic.Instance.OnJoysticInput += RotateCanon;
                    ButtonJoystic.Instance.OnJoysticButtonRelease += FireTrigger;
                }
            }
            else if(suscribedImputEvents)
            {
                suscribedImputEvents = false;
                ButtonJoystic.Instance.OnJoysticInput -= RotateCanon;
                ButtonJoystic.Instance.OnJoysticButtonRelease -= FireTrigger;
            }

        }
    }
    public SpriteRenderer Renderer
    {
        get
        {
            return renderer;
        }

        set
        {
            renderer = value;
        }
    }

    //public static Weapon CreateWeapon(GameObject where, WeaponInfo info)
    //{
    //    where.SetActive(false);
    //    where.name = info.name;
    //    Weapon myC = where.AddComponent<Weapon>();
    //    GameObject firepoint = new GameObject("firepoint");
    //    firepoint.transform.SetParent(where.transform);
    //    myC.spawnPos = firepoint.transform;
    //    myC.info = info;
    //    where.SetActive(true);

    //    return myC;
    //}
    private void Awake()
    {
        gameObject.name = info.name;
        Renderer = GetComponent<SpriteRenderer>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        SetColor(0);
        Active = false;
        Renderer.enabled = false;
    }
    private void Start()
    {
        //GameManager.Instance.OnLevelLoaded += ()=>Invoke("OnGlameplayStart",1);

    }
    bool GetOwner()
    {
        if (transform.parent == null) return false;
        Owner = transform.parent.GetComponentInParent<Character>();
        if (Owner != null)
        {
            detectionRadius = Owner.DetectRadius;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetColor(int i)
    {
        if (info.Colors.Count>0)
        {
            i = i % info.Colors.Count;
            Renderer.sharedMaterial.SetColor("_Color1", info.Colors[i].firstColor);
            Renderer.sharedMaterial.SetColor("_Color2", info.Colors[i].secondColor);
            Renderer.sharedMaterial.SetColor("_Color3", info.Colors[i].thirdColor);
        }
    }
    public void Upgrade()
    {
        info.Level++;
    }
    public void OnGameplayStart()
    {
        Active = GetOwner();
        if (Active)
        {
            pool = GameplayManager.Instance.GetBulletPool(info.BulletType);
            ActiveAutoAim();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (fire)
        {
            fire = false;
            Fire();
        }
        if(AutoAiming)
        {
            if (target != null)
            {
                Vector2 relative = target.Rigidbody.position - Rigidbody.position;
                float angle =Vector2.SignedAngle(transform.right,relative);
                float dir = angle / Mathf.Abs(angle);
                if (angle < 0) angle += 360;

                if (angle > 5)
                {
                    Rigidbody.rotation += (info.Velocidad_Rotacion * dir * Time.fixedDeltaTime);
                }
            }
        }

    }
    protected void SerchTarget()
    {
        Transform t_Transform = SelectTarget();
        if (t_Transform != null)
        {
            Character n_target = t_Transform.GetComponent<Character>();
            if (n_target != target)
            {
                target = n_target;

            }
        }
        else target = null;
    }
    public void FireTrigger()
    {
        if (canFire)
        {
            fire = true;
        }
    }
    protected virtual void Fire()
    {
        Bullet bullet = pool.GetAt(SpawnPos.position,Rigidbody.rotation);
        if (bullet != null)
        {
            canFire = false;
            bullet.Fire(Owner,info.BulletDamageMultiplier);
            Invoke("ResetCD", info.FireRate);
        }
    }
    protected void ResetCD()
    {
        canFire = true;
    }
    protected virtual void RotateCanon(Vector2 _lookPos)
    {
        if(_lookPos.magnitude != 0)
        {
            PauseAutoAim(2);
            float angle = (Mathf.Atan2(_lookPos.y, _lookPos.x) * Mathf.Rad2Deg);
            Rigidbody.rotation = (angle);
        }
    }
    protected virtual Transform SelectTarget()
    {
        Transform target = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetDetectionLayer);
        if (colliders.Length > 0)
        {
            float nearD = Mathf.Infinity;
            for (int i = 0; i < colliders.Length; i++)
            {
                float d = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (nearD > d)
                {
                    nearD = d;
                    target = colliders[i].transform;
                }
            }
        }
        return target;
    }
    protected void ActiveAutoAim()
    {
        AutoAiming = true;
    }
    protected void PauseAutoAim(float t)
    {
        CancelInvoke("ActiveAutoAim");
        AutoAiming = false;
        Invoke("ActiveAutoAim", t);
    }
    protected virtual void OnDrawGizmos()
    {
        if(target != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(target.transform.position, 1);
        }
        Gizmos.color = Color.Lerp(Color.red, Color.yellow,0.6f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }

}

