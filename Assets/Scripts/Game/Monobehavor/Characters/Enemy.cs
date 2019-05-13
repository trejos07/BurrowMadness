using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AICharacter
{
    private static List<Enemy> types;
    [SerializeField] EnemyTier tier;

    public static List<Enemy> Types
    {
        get
        {
            if (types == null)
            {
                types = new List<Enemy>(Resources.LoadAll<Enemy>("Prefabs/Characters/Enemys"));
            }
            return types;
        }
    }

    public int ID
    {
        get
        {
            return Types.IndexOf(this);
        }
    }
    public EnemyTier Tier
    {
        get
        {
            return tier;
        }

        set
        {
            tier = value;
        }
    }
    
}

public enum EnemyTier { A=1,B=2,C=3,F}