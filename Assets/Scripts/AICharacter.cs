using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(AstarAgent))]
public class AICharacter : Character
{
    private State currentState;
    [SerializeField] State startState;
    [SerializeField] AstarAgent agent;
    [SerializeField] LayerMask playerLayer;

    protected AstarAgent Agent
    {
        get
        {
            return agent;
        }

        set
        {
            agent = value;
        }
    }

    public delegate void colliding(Collision collision);
    public event colliding OnCollision;

    protected override void Awake()
    {
        base.Awake();
        Agent = GetComponent<AstarAgent>();
        
    }
    protected override void Start()
    {
        base.Start();
        currentState = startState;
        State[] states = GetComponents<State>();
        foreach (State s in states)
        {
            s.OnStateEnable += SetNewState;
        }
        ExecuteCurrentState();
    }
    protected void Reset()
    {
        Start();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (OnCollision != null)
            OnCollision(collision);
    }
    protected override void Death()
    {
        base.Death();
        //AisPool.Instance.ReturnToPool(this);
    }
    protected void ExecuteCurrentState()
    {
        currentState.Execute();
        Invoke("ExecuteCurrentState", currentState.StateCheckTime);
    }
    protected void StopExecuteCurrentState()
    {
        CancelInvoke("ExecuteCurrentState");
    }
    public override void MoveTo(Vector3 _Pos)
    {
        if (_Pos.magnitude != 0)
        {
            Vector2 movingDir = transform.InverseTransformPoint(_Pos);
            movingDir.Normalize();
            Rigidbody.MovePosition(Rigidbody.position + movingDir * movSpeed * Time.deltaTime);
        }
    }
    public Transform LookForPlayerArround()
    {
        Transform Player = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, DetectRadius, playerLayer);
        if (colliders.Length > 0)
        {
            if (colliders[0] != null)
            {
                Vector3 playerDir = transform.InverseTransformPoint(colliders[0].transform.position);
                float playerDistance = playerDir.magnitude;
                Ray playerRay = new Ray(transform.position, playerDir);
                if (!Physics.SphereCast(playerRay, 0.5f, playerDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);


    }
}
