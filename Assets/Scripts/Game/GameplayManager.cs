using System.Collections.Generic;
using System;
using UnityEngine;
using Cinemachine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] GameObject WorldRoot;
    [SerializeField] CinemachineVirtualCamera gameplayCamera;
    
    World activeWorld;
    GameObject poolsGos;
    EnemyFactory worldEnemyFactory;
    float gamplayTime;

    Dictionary<Bullet, BulletPool> bulletPools = new Dictionary<Bullet, BulletPool>();

    public World ActiveWorld
    {
        get
        {
            return activeWorld;
        }

        set
        {
            activeWorld = value;
        }
    }
    public float GamplayTime
    {
        get
        {
            return gamplayTime;
        }

        set
        {
            gamplayTime = value;
        }
    }
    public EnemyFactory WorldEnemyFactory
    {
        get
        {
            return worldEnemyFactory;
        }
    }

    public delegate void Action();
    public event Action OnStartGamplay;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        activeWorld = World.CreateWorld(WorldRoot, GameManager.Instance.SelectedWorld);
        poolsGos = new GameObject("Pools");
        poolsGos.transform.SetParent(transform);
        CreatePools();
        
    }
    private void Start()
    {
        worldEnemyFactory = new EnemyFactory(poolsGos);
        
        gameplayCamera.Follow = Player.Instance.transform;
    }
    public void StartGame()
    {
        if (OnStartGamplay != null)
        {
            OnStartGamplay();
        }
    }
    private void Update()
    {
        gamplayTime += Time.deltaTime;
    }
    public void SaveSesion()
    {
        activeWorld.WriteTileInfo();
        FindObjectOfType<Inventory>().SaveInventory();
    }
    public void CreatePools()
    {
        bulletPools = new Dictionary<Bullet, BulletPool>();
        Bullet[] bToPool = Resources.LoadAll<Bullet>("Prefabs/Bullets");
        for (int i = 0; i < bToPool.Length; i++)
        {
            Bullet b = bToPool[i];
            if (b != null)
            {
                GameObject poolGo = new GameObject("Pool "+b.Name);
                poolGo.transform.SetParent(poolsGos.transform);
                bulletPools.Add(b, BulletPool.CreatePool(poolGo, b, 100));
            }
        }
    }
    public BulletPool GetBulletPool(int bulletID)
    {
        return bulletPools[Bullet.Types[bulletID]];
    }
    public BulletPool GetBulletPool(Bullet type)
    {
        return bulletPools[type];
    }
}
