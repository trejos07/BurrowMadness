using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] GameObject WorldRoot;
    World activeWorld;


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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        activeWorld = World.CreateWorld(WorldRoot, GameManager.ins.SelectedWorld);
    }

    public void SaveSesion()
    {
        activeWorld.WriteTileInfo();
        FindObjectOfType<Inventory>().SaveInventory();
    }

}
