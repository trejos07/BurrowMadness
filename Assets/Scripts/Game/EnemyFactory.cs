using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory
{
    int poolsSizes = 50;
    int timeToHardDiff = 3;
    GameObject poolsGos;
    Dictionary<Enemy, EnemyPool> enemysPools = new Dictionary<Enemy, EnemyPool>();
    Enemy[] abilitableEnemies;

    public EnemyFactory(GameObject poolsGos)
    {
        this.poolsGos = poolsGos;
        
        SetAbilitables();
        CreatePools();
    }

    public EnemyPool GetEnemyPool(Enemy type)
    {
        return enemysPools[type];
    }

    public void CreatePools()
    {
        enemysPools = new Dictionary<Enemy, EnemyPool>();
        //Enemy[] eToPool = Resources.LoadAll<Enemy>("Prefabs/Characters/Enemys");
        for (int i = 0; i < abilitableEnemies.Length; i++)
        {
            Enemy e = abilitableEnemies[i];
            if (e != null)
            {
                GameObject poolGo = new GameObject("Pool " + e.Name);
                poolGo.transform.SetParent(poolsGos.transform);
                enemysPools.Add(e, EnemyPool.CreatePool(poolGo, e, poolsSizes));
            }
        }
    }

    public void SetAbilitables()
    {
        int d = (int)GameplayManager.Instance.ActiveWorld.WorldInfo.dificulty;
        abilitableEnemies = Enemy.Types.Where(x => (int)x.Tier <= d).OrderBy(x => x.Tier).ToArray();
    }
    public Enemy GetAbilitableEnemy()
    {
        float t = GameplayManager.Instance.GamplayTime;
        int disponimeEnemys = abilitableEnemies.Count();
        int minutes = ((int)t / 60);

        int range = 4;
        int maxRange = minutes < timeToHardDiff ? ((minutes * (disponimeEnemys - 1)) / timeToHardDiff) + 1 : disponimeEnemys;
        int minRange = maxRange-range > 0 ? ((minutes * (disponimeEnemys - 1)) / timeToHardDiff) + 1-range : 0;

        int r = Random.Range(minRange, maxRange);
        return abilitableEnemies[r];
        
    }

    public Enemy GetEnemyAt(Vector2 pos, float zRot)
    {
        EnemyPool pool = GetEnemyPool(GetAbilitableEnemy());
        Enemy e= pool.GetAt(pos, zRot);
        e.Active = true;
        return e;

    }
}