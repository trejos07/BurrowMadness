using UnityEngine;

public class TestTask : Task
{
    [SerializeField]
    private bool succeedTask;

    [SerializeField]
    private string taskName;

    public override bool Execute()
    {
        print(string.Format("Task {1} will {0}", succeedTask, string.IsNullOrEmpty(taskName) ? "Invalid" : taskName));

        return succeedTask;
    }
}