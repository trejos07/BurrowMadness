using UnityEngine;

public abstract class Composite : RootChild
{
    [SerializeField]
    private RootChild[] children;

    public override void SetTargetAI(AICharacter target)
    {
        base.SetTargetAI(target);

        foreach (BTNode node in children)
        {
            node.SetTargetAI(TargetAI);
        }
    }

    protected abstract bool ValueToBreak { get; }

    public override bool Execute()
    {
        bool result = ValueToBreak;

        foreach (BTNode node in children)
        {
            result = node.Execute();

            if (result == ValueToBreak)
            {
                break;
            }
        }

        //print(string.Format("Result for {0} : {1}", GetType().ToString(), result));

        return result;
    }
}