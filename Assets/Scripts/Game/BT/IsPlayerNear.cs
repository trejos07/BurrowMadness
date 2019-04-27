using UnityEngine;

public class IsPlayerNear : Selector
{

    [SerializeField] float LookDistance;
     Transform player;

    private void Awake()
    {
        player = FindObjectOfType<Player>().transform;
    }

    protected override bool CheckCondition()
    {
        float d = Vector3.Distance(TargetAI.transform.position, player.position);
        if (d <= LookDistance)
        {
            return true;
        }
        else
            return false;
        
    }


}