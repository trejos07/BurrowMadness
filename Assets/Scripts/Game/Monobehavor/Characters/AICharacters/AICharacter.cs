using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(AstarAgent))]
public class AICharacter : Character
{
    public float fut = 0;
    private bool active;
    private State currentState;
    [SerializeField] State startState;
    [SerializeField] State[] states;
    [SerializeField] AstarAgent agent;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform firePoint;
    [SerializeField] int bulletID;
    BulletPool pool;

    public AstarAgent Agent
    {
        get
        {
            return agent;
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
            Collider.enabled = value;
            
            //Rigidbody.simulated = value;
            //if (value)
            //    //ExecuteState();
            //else
            //{
            //    Agent.IsStopped = true;
            //    CancelInvoke("ExecuteState");
            //}
                
        }
    }

    public delegate void AICharacterInterction(AICharacter character);
    public delegate void colliding(Collision collision);
    public event colliding OnCollision;
    public static event AICharacterInterction OnInactiveAI;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<AstarAgent>();
        states = GetComponents<State>();
        if(AttackRadius>3.5f)
            pool = GameplayManager.Instance.GetBulletPool(bulletID);

    }
    protected override void Start()
    {
        base.Start();
        currentState = startState;
        //currentState.Execute();
        State[] states = GetComponents<State>();
        foreach (State s in states)
        {
            s.OnStateEnable += SetNewState;
        }
        Active = false;
    }
    protected void Reset()
    {
        Start();
    }
    protected void ExecuteState()
    {
        currentState.Execute();
        Invoke("ExecuteState", currentState.StateCheckTime);
    }
    protected virtual void FixedUpdate()
    {
        if (Active)
        {
            fut += Time.fixedDeltaTime;
            if (fut >= currentState.StateCheckTime)
            {
                currentState.Execute();
                fut = 0;
            }
        }
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (OnCollision != null)
            OnCollision(collision);
    }
    protected override void Death()
    {
        Active = false;
        if (OnInactiveAI != null) OnInactiveAI(this);
        base.Death();
        //AisPool.Instance.ReturnToPool(this);
    }
    public State GetState(string state)
    {
        return states.Where(x => x.Name == state).FirstOrDefault();
    }
    public override void MoveTo(Vector2 _Pos)
    {
        if (_Pos.magnitude != 0&&Active)
        {
            Vector2 movingDir = transform.InverseTransformPoint(_Pos);
            movingDir.Normalize();
            Rigidbody.MovePosition(Rigidbody.position + movingDir * MovSpeed * Time.deltaTime);
        }
    }
    public Transform LookForPlayerArround(float d)
    {
        Transform Player = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, d, playerLayer);
        if (colliders.Length > 0)
        {
            if (colliders[0] != null)
            {
                Vector3 playerDir = transform.InverseTransformPoint(colliders[0].transform.position);
                float playerDistance = playerDir.magnitude;
                Ray playerRay = new Ray(transform.position, playerDir);
                if (!Physics.SphereCast(playerRay, 0.5f, playerDistance, LayerMask.NameToLayer("World"), QueryTriggerInteraction.Ignore))
                {
                    Player = colliders[0].transform;
                }
            }
        }
        return Player;
    }
    public void SetNewState(State newState)
    {
        currentState = newState;
    }
    public override void Atack(Character damagable)
    {
        if (weapon == null&&AttackRadius>3.5f)
        {
            ShootBullet(damagable.Rigidbody.position);
        }
        else
            base.Atack(damagable);
    }
    public void ShootBullet(Vector2 pos)
    {
        if (pool != null && firePoint != null)
        {
            Vector2 relative = pos - Rigidbody.position;
            float rot = (float)Math.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
            pool.GetAt(firePoint.position, rot).Fire(this,1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);


    }
}
