using UnityEngine;

public class TestSelector : Selector
{
    [SerializeField]
    private bool checkCondition = false;

    protected override bool CheckCondition()
    {
        print(string.Format("Selector will succeed: {0}", checkCondition));

        return checkCondition;
    }
}