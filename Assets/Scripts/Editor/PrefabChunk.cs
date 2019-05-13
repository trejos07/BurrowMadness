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
    [SerializeField] bool ShowGrid=false;
    [SerializeField] private int layers=1;
    int width;
    int heigth;

    List<Tilemap> tilemaps = new List<Tilemap>();
    Vector2Int chunkSize;
    BoundsInt myBounds;
    [SerializeField] ChunkType type;

    Item[] chunkSources;

    [HideInInspector]
    public ChunkInfo ChunkInfo;


    public int Layers
    {
        get
        {
            return layers;
        }

        set
        {
            layers = value;
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Chunks/Prefab Chunk", false, 10)]
    static void CreateChunk(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("Prefab Chunk");
        
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
        go.AddComponent<PrefabChunk>();
    }
#endif

    private void Start()
    {
        tilemaps = new List<Tilemap>(GetComponentsInChildren<Tilemap>());
        Grid g = GetComponent<Grid>();
        g.cellSize = Vector3.one* 1.75f;
        chunkSize = new Vector2Int(20, 20);
        width = chunkSize.x;
        heigth = chunkSize.y;
        CheckBounds();


        updateLayers();
       
    }

    private void OnValidate()
    {
        updateLayers();
        for (int i = 0; i < tilemaps.Count; i++)
        {
            tilemaps[i].RefreshAllTiles();
        }
    }

    public void CheckBounds()
    {
        Vector3Int pos = Vector3Int.FloorToInt(tilemaps[0].transform.position) - new Vector3Int(width / 2, heigth / 2, 0);
        myBounds = new BoundsInt(pos, new Vector3Int(width, heigth, 1));
    }

    public void updateLayers()
    {
        int _layers = tilemaps.Count;

        if (_layers != layers)
        {
            if(tilemaps.Count > layers)
            {
                for (int i = 0; i < _layers-layers; i++)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(tilemaps[tilemaps.Count - 1].gameObject);
                        tilemaps.RemoveAt(tilemaps.Count - 1);
                    };
                }
            }
            else
            {
                for (int i = 0; i < layers - _layers; i++)
                {
                    tilemaps.Add(setupTilemap());
                }
            }
                
        }
        
    }

    Tilemap setupTilemap()
    {
        GameObject tm = new GameObject("Layer_" + tilemaps.Count);
        Tilemap tilemap = tm.AddComponent<Tilemap>();
        Collider2D ctm = tm.AddComponent<TilemapCollider2D>();
        ctm.usedByComposite = true;
        Rigidbody2D rtm = tm.AddComponent<Rigidbody2D>();
        rtm.position = transform.position;
        rtm.bodyType = RigidbodyType2D.Static;
        TilemapRenderer cctm = tm.AddComponent<TilemapRenderer>();
        tm.transform.SetParent(transform);
        return tilemap;
    }

    public void ClearTilemaps()
    {
        for (int i = 0; i < tilemaps.Count; i++)
        {
            tilemaps[i].ClearAllTiles();
        }
    }

    public void GetTileInfo()
    {
        int zSize = tilemaps.Count;

        Tile[,,] chunkTileInfo = new Tile[width, heigth, zSize];
        
        for (int i = 0; i < zSize; i++)
        {
            TileBase[] tileArray = tilemaps[i].GetTilesBlock(myBounds);

            for (int x = 0; x < tileArray.Length; x++)
            {
                int y = x / width;
                //Debug.Log(x % width+" - "+ y + " - " + i);
                chunkTileInfo[x % width, y, i] = tileArray[x] as Tile;
            }
        }
        ChunkInfo = new ChunkInfo(World.TilesToIds(chunkTileInfo),name,type );
        ChunkInfo.name = name;
    }

    public void setTiles(Tile[,,] chunkTileInfo, Vector2Int chunkPos)
    {
        for (int y = 0; y < chunkTileInfo.GetLength(1); y++)
        {
            for (int x = 0; x < chunkTileInfo.GetLength(0); x++)
            {
                for (int z = 0; z < chunkTileInfo.GetLength(2); z++)
                {
                    if (chunkTileInfo[x, y, z] != null)
                    {
                        int i = (x + chunkPos.x - (chunkSize.x / 2));
                        int j = y - chunkSize.y + chunkPos.y;

                        tilemaps[z].SetTile(new Vector3Int(i, j, 0), chunkTileInfo[x, y, z]);
                        //Debug.Log("se puso un " + chunkTileInfo[x, y, z].name);
                    }

                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color lowAlpha = new Color(1, 1, 1, 0.1f);
        if(ShowGrid)
        {
            Gizmos.color = Color.cyan * lowAlpha;
            foreach (Vector3Int pos in myBounds.allPositionsWithin)
            {
                Gizmos.DrawWireCube((Vector3)pos * 1.75f + Vector3.one * 0.5f * 1.75f, Vector3.one * 1.75f);
            }
        }
        

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(myBounds.center, (Vector3)myBounds.size * 1.75f);

    }
    
    public void LoadWorldInfo(ChunkInfo info)
    {
        ClearTilemaps();
        Start();

        ChunkInfo = info;
        gameObject.name = info.name;
        layers = info.tileInfo[0][0].GetLength(0);
        updateLayers();

        Vector2Int pos = new Vector2Int(0,chunkSize.y/2);
        setTiles(World.IdsToTiles(ChunkInfo.tileInfo), pos);
    }
}
