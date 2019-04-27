using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AstarAgent))]
public class EnemyViejo : MonoBehaviour, IDamagable
{
    [SerializeField] float detectionRadius;           // Radio de detección en que el enemigo "siente" al jugador
    [SerializeField] LayerMask playerlayer;           // Layer en donde se encuentra el jugador
    [SerializeField] private float aggroDistance;    // Distancia del enemigo al jugador en la que lo puedo atacar (varía entre melee y rango)
    [SerializeField] private float checkTime;        // Cada cuánto reviso qué condiciones de estado se cumple. (en qué estado debería estar)
    [SerializeField] private float speedMov;      // Velocidad de movimiento del enemigo
    [SerializeField] int maxHealth = 50;
    [SerializeField] float atackCD;
    [SerializeField] int atackdamage;
    private float checktimeinicial;                  // almaceno el valor del checktime por si su valor cambia durante el script.
    private int health;
    private IAStates MyState = IAStates.IdleState;
    

    private Transform Target;                                // Almacena una posición a la que va el Enemigo 
    private Rigidbody2D rigidbody;
    private AstarAgent agent;
    private List<Vector3> path = new List<Vector3>();        // Esquinas que representan diferentes targets dentro de la inteligencia de navegación
    private bool followingPath;
    private bool atacking;

    public enum IAStates { WalkState, AggroState, FollowState, IdleState }
    public enum MoveStates { normal, fly, dig }

    public delegate void HealthChange(float _newHealth);
    public event HealthChange OnHealthChange;

    public int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
            if (OnHealthChange != null)
                OnHealthChange((float)health / maxHealth);
        }
    }

    void Start()
    {
        MyState = IAStates.WalkState;
        checktimeinicial = checkTime;              // Llamo la corrutina para que se ejecute a modo de "update" una vez cada Checktime Segundos.
        agent = GetComponent<AstarAgent>();
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitForCheck());
        Health = maxHealth;
    }
    void die ()
    {
        Destroy(gameObject);
    }
    public void MoveTo(Vector3 _inputPos)
    {
        if (_inputPos.magnitude != 0)
        {
            Vector2 movingDir = transform.InverseTransformPoint(_inputPos);
            movingDir.Normalize();
            rigidbody.MovePosition(rigidbody.position+movingDir*speedMov*Time.deltaTime);
            Debug.DrawRay(transform.position, movingDir, Color.cyan);
        }
           
        
    }
    public Transform LookForPlayer()                     // Este método "percibe al jugador" para perseguirlo de ser posible
    {
        Transform Player = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerlayer);

        if (colliders.Length>0 )
        {
            if (colliders[0] != null)
                Player = colliders[0].transform;
        }

        if (Player != null)
        {
            StartCoroutine(agent.LookForPathTo(Player.position));
            path = agent.GetPathCornersInWorld();
        }

        return Player;
    }
    private void CheckState()    // Checkeo mi estado dependiendo de si tengo o no target
    {
        int randomToIdle = Random.Range(0, 10);
        if (Target != null)                                              // Si hay target
        {
            if (Vector3.Distance(transform.position, Target.position) < aggroDistance)
            {
                if (Physics2D.Raycast(transform.position, transform.InverseTransformPoint(Target.position), aggroDistance, playerlayer))
                {
                    MyState = IAStates.AggroState;
                }
            }
            else
            {
                MyState = IAStates.FollowState;
            }

        }
        else if(!followingPath)
        {
            if (randomToIdle > 8)    // No hay target y saqué entre 9 y 10 en el dado
            {
                MyState = IAStates.IdleState;
            }
            else                                                             // No hay target y el dado dio entre 1 y 8
            {
                MyState = IAStates.WalkState;
            }
        }

    }
    public void moveRandom()
    {
        if(!followingPath)
        {
            StartCoroutine(agent.LookForPathTo(agent.GetRandomPointArround(transform.position,6)));
            path = agent.GetPathCornersInWorld();
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());

        }
    }
    public void ExecuteState()   // Método que ejecuta la acción que tiene cada estado.
    {
        switch (MyState)
        {
            case (IAStates.WalkState):
                checkTime = checktimeinicial;
                moveRandom();
                break;
            case (IAStates.AggroState):
                checkTime = checktimeinicial;
                if(!atacking)
                    StartCoroutine(DoingDamage());
                break;
            case (IAStates.FollowState):
                checkTime = checktimeinicial;
                if(!followingPath)
                {
                    StopCoroutine(FollowPath());
                    StartCoroutine(FollowPath());
                }
                break;
            case (IAStates.IdleState):
                checkTime = 3;
                // Reproduce una animación (Graciosa) de Idle
                break;
        }
    }
    public IEnumerator WaitForCheck()        // Corrutina para que ejecute Checkstate() cada checktime segundos.
    {
        while (true)
        {
            Target = LookForPlayer();

            CheckState();
            ExecuteState();
            yield return new WaitForSeconds(checkTime);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroDistance);


    }
    public IEnumerator FollowPath()
    {
        followingPath = true;
        while (path.Count > 0)
        {
            float distanceToNextNode = Vector3.Distance(transform.position, path[0]);
            
            if (distanceToNextNode < 0.25f)
                path.Remove(path[0]);
            else
                MoveTo(path[0]);

            yield return null;
        }
        followingPath = false;

    }
    public IEnumerator DoingDamage()
    {
        atacking = true;
        DoDamage();
        yield return new WaitForSeconds(atackCD);
        atacking = false;

    }
    public virtual void DoDamage()
    {
        IDamagable Damagable = Target.GetComponent<IDamagable>();
        if (Damagable != null)
        {
            Damagable.GetDamage(atackdamage);
        }
    }
    public void GetDamage(int _Damage)
    {
        if (Health - _Damage <= 0)
            die();
        else
            Health -= _Damage;
    }
}
