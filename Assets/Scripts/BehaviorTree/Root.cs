using UnityEngine;

public class Root : BTNode
{
    [SerializeField]
    private Composite child;

    public override bool Execute()
    {
        bool result = true;

        if (child != null)
        {
            result = child.Execute();
        }

        return result;
    }
}