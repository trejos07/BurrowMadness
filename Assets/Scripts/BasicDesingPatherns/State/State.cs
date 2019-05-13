using UnityEngine;
using System;

public abstract class State : MonoBehaviour
{
    [SerializeField]private State nextState;
    [SerializeField] protected float baseStateChecktime;
    private float stateCheckTime;
    private AICharacter owner;

    public string Name
    {
        get
        {
            return this.GetType().Name;
        }
    }
    public float StateCheckTime
    {
        get
        {
            return stateCheckTime;
        }

        set
        {
            stateCheckTime = value;
        }
    }
    public State NextState
    {
        get
        {
            return nextState;
        }

        set
        {
            nextState = value;
        }
    }
    public AICharacter Owner
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

    public delegate void StateChange(State state);
    public event StateChange OnStateEnable;
    
    protected virtual void  Awake()
    {
        StateCheckTime = baseStateChecktime;
        Owner = GetComponent<AICharacter>();
    }
    public abstract void Execute();
    public void SwitchToNextState()
    {
        if (NextState != null)
        {
            Toggle(false);
            NextState.Toggle(true);
        }
    }
    public void Toggle(bool val)
    {
        this.enabled = val;

        if (val)
        {
            if(OnStateEnable!=null)
            {
                OnStateEnable(this);
            }
        }
    }
}