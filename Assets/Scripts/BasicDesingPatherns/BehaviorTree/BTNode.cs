using UnityEngine;

public abstract class BTNode : MonoBehaviour
{
    [SerializeField]
    private AICharacter targetAI;

    public AICharacter TargetAI
    {
        get { return targetAI; }
        protected set { targetAI = value; }
    }

    public virtual void SetTargetAI(AICharacter target)
    {
        if (TargetAI == null)
        {
            TargetAI = target;
        }
    }

    public abstract bool Execute();
}