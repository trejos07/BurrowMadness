using UnityEngine;

public class RunBT : State
{
    [SerializeField] protected Root btRoot;

    public override void Execute()
    {
        //CancelInvoke("RunRoot");
        //InvokeRepeating("RunRoot",0.001f,0.001f);
        RunRoot();
    }
    public void RunRoot()
    {
        if (btRoot != null)
        {
            btRoot.Execute();
        }
    }
    
}
