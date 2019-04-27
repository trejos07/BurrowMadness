using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAction 
{
    Dialogue dialogue;
    Action[] actions;

    public TutorialAction(Dialogue dialogue, Action[] actions)
    {
        this.dialogue = dialogue;
        this.actions = actions;
    }

    public TutorialAction(Action[] actions)
    {
        this.actions = actions;
    }

    public Dialogue Dialogue
    {
        get
        {
            return dialogue;
        }

        set
        {
            dialogue = value;
        }
    }
    public Action[] Actions
    {
        get
        {
            return actions;
        }

        set
        {
            actions = value;
        }
    }

    public delegate void Action();

    public void ExecuteActions()
    {
        foreach (Action a in Actions)
        {
            a.Invoke();
        }
    }

}
