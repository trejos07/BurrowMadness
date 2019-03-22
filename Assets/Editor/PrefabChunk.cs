using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


[ExecuteInEditMode]
[RequireComponent (typeof(Transform))]
[RequireComponent (typeof(Grid))]
public class PrefabChunk : MonoBehaviour
{
    [SerializeField] private int layers=1;
    int width;
    int heigth;

    [SerializeField] List<Tilemap> tilemaps = new List<Tilemap>();
    [SerializeField] Vector2Int chunkSize;
    Tile[,,] tileInfo;

    Item[] chunkSources;
    
    public Tile[,,] TileInfo
    {
        get
        {
            return tileInfo;
        }

        set
        {
            tileInfo = value;
            
        }
    }
    
    public int Layers
    {
        get
        {
            return layers;
        }

        set
        {
            layers = value;
            updateLayers();
        }
    }


    [MenuItem("GameObject/Chunks/Chunk", false, 10)]
    static void CreateChunk(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("Prefab Chunk");
        go.AddComponent<PrefabChunk>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }


    private void Start()
    {
        width = chunkSize.x;
        heigth = chunkSize.y;
        tileInfo = new Tile[width, heigth, layers];

        updateLayers();

       
    }

    private void Update()
    {
        updateLayers();
        if(chunkSize != Vector2Int.zero)
        {
            width = chunkSize.x;
            heigth = chunkSize.y;
            tileInfo = new Tile[width, heigth, layers];
        }



    }

    public void updateLayers()
    {
        while (tilemaps.Count > Layers)
        {
            tilemaps.RemoveAt(tilemaps.Count - 1);
        }
        while (tilemaps.Count < Layers)
        {
            tilemaps.Add(setupTilemap());
        }
    }

    Tilemap setupTilemap()
    {
        GameObject tm = new GameObject();
        Tilemap tilemap = tm.AddComponent<Tilemap>();
        tm.AddComponent<TilemapRenderer>();
        tm.AddComponent<TilemapCollider2D>();
        tm.transform.position = transform.position;
        tm.transform.SetParent(transform);

        return tilemap;
    }

    public void GetTileInfo()
    {
        BoundsInt mb = new BoundsInt(Vector3Int.FloorToInt(transform.position), new Vector3Int(width, heigth, 1));
        for (int i = 0; i < tilemaps.Count; i++)
        {
            TileBase[] tileArray = tilemaps[i].GetTilesBlock(mb);

            for (int x = 0; x < tileArray.Length; x++)
            {
                int y = x / width;
                Debug.Log(x % width+" - "+ y + " - " + i);
                TileInfo[x % width, y, i] = tileArray[x] as Tile;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, heigth, 0)*1.8f);
#endif
    }
    
}
