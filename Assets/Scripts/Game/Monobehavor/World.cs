using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

public class World : MonoBehaviour
{
    #region atributos
    private WorldInfo worldInfo;
    private Vector2Int chunkSize;
    private int width;
    private int heigth;
    Color bg;
    private Tile ground, backGround;
    private List<Tilemap> tileMaps = new List<Tilemap>();
    private NodeNavMesh navMesh;


    private List<Spawner> spawners = new List<Spawner>();
    private GameObject SpawnerPrefab;
    private bool destroingTile = false;

    private Material pixelShader;

    public Tilemap GroundTileMap
    {
        get
        {
            return tileMaps[0];
        }
    }

    public WorldInfo WorldInfo
    {
        get
        {
            return worldInfo;
        }
        
    }

    #endregion

    public delegate void DestructingTile();
    public static event DestructingTile OnDestructingTile;

    #region static Methods

    public static World CreateWorld(GameObject where, WorldInfo info)
    {
        where.SetActive(false);
        where.name = info.name;
        World myC = where.AddComponent<World>();
        myC.worldInfo = info;
        where.SetActive(true);

        return myC;
    }
    public static Tile[,,] IdsToTiles(int[][][] ids)
    {
        int xSizes = ids.GetLength(0);
        int ySizes = ids[0].GetLength(0);
        int zSizes = ids[0][0].GetLength(0);

        Tile[,,] tiles = new Tile[xSizes, ySizes, zSizes];

        for (int y = 0; y < ySizes; y++)
        {
            for (int x = 0; x < xSizes; x++)
            {
                for (int z = 0; z < zSizes; z++)
                {
                    if (z == 0)
                    {
                        tiles[x, y, z] = TerrainTile.GetTerrainTileOfID(ids[x][y][z]);
                    }
                    if (z == 1)
                    {
                        Item item = Item.GetItemOfID(ids[x][y][z]);
                        if (item != null)
                            tiles[x, y, z] = new ItemTile(item);
                    }

                }
            }
        }

        return tiles;
    }
    public static int[][][] TilesToIds(Tile[,,] tiles)
    {
        int xSizes = tiles.GetLength(0);
        int ySizes = tiles.GetLength(1);
        int zSizes = tiles.GetLength(2);

        int[][][] ids = new int[xSizes][][];

        for (int i = 0; i < xSizes; i++)
        {
            ids[i] = new int[ySizes][];
            for (int j = 0; j < ySizes; j++)
            {
                ids[i][j] = new int[zSizes];
            }
        }


        for (int y = 0; y < ySizes; y++)
        {
            for (int x = 0; x < xSizes; x++)
            {
                for (int z = 0; z < zSizes; z++)
                {
                    if (z == 0)
                    {
                        Tile tile = tiles[x, y, z];
                        TerrainTile ttile = tile as TerrainTile;
                        if (tile != null)
                            ids[x][y][z] = ttile.Id;
                    }
                    if (z == 1)
                    {
                        ItemTile item = tiles[x, y, z] as ItemTile;
                        if (item != null)
                        {
                            if (item.Item != null)
                                ids[x][y][z] = item.Item.Id;
                        }

                    }

                }
            }
        }

        return ids;


    }
    #endregion

    #region methods
    private void Awake()
    {
        pixelShader = Resources.Load<Material>("Materials/PixelSnapMat");
        ground = Resources.Load<Tile>("Tiles/Ground_Tile");
        backGround = Resources.Load<Tile>("Tiles/BackGround_Tile");
        SpawnerPrefab = Resources.Load<GameObject>("Prefabs/Spawner");
        chunkSize = GameManager.Instance.Settings.chunkSize;

        width = worldInfo.worldSizeInChunks.x;
        heigth = worldInfo.worldSizeInChunks.y;

       if(worldInfo.chunks==null)
        {
            worldInfo.chunks = new ChunkInfo[width][];
            for (int i = 0; i < width; i++)
            {
                worldInfo.chunks[i] = new ChunkInfo[heigth];
            }
        }


        SetWorldColor(worldInfo.color);

        Physics2D.gravity = new Vector2(0, worldInfo.gravity);

        int layers = 2;

        for (int i = 0; i < layers; i++)
        {
            tileMaps.Add(SetupTilemap());
        }
        SetDungeons();
        SetUpWorld();
        navMesh = NodeNavMesh.CreateNavMesh(gameObject, tileMaps[0]);
        tileMaps[0].GetComponent<TilemapRenderer>().material = Resources.Load<Material>("Materials/GoundMat");
        tileMaps[0].GetComponent<TilemapRenderer>().material.SetColor("_Color1", worldInfo.color);
        
        SetSpawners();
        SetWorldLimits();
        FindObjectOfType<Player>().OnDiggingToDir += CheckDestruction;
        worldInfo.Save();
        TransformUtility.ChangeLayersRecursively(transform, gameObject.layer);
    }
    public void SetUpWorld()
    {
        int wr = width * chunkSize.x / 2;
        int nWRsorces = worldInfo.worldSources.Length;
        List <Item> chunkItems = new List<Item>();
        Item[] worldItems = Item.ItemsFromIds(worldInfo.worldSources);

        for (int y = 0; y < heigth; y++)
        {
            int nItems = nWRsorces>1? ((y*(nWRsorces - 1) )/heigth)+1:1;
            int toAdd =  nItems - chunkItems.Count;
            while (toAdd > 0)
            {
                Item posibleItem = Item.ChanceRandomItem(worldItems);
                if(posibleItem!=null)
                {
                    if (!chunkItems.Contains(posibleItem))
                    {
                        chunkItems.Add(posibleItem);
                        toAdd--;
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                if(chunkItems==null || chunkItems.Count<=0)
                {
                    Debug.Log("is null"); 
                }
                Vector2Int chunkPosition = new Vector2Int(-wr + chunkSize.x / 2 + x * chunkSize.x, -y * chunkSize.y);

                if (worldInfo.chunks[x][y] == null)
                {
                    Chunk chunk = new ProceduralChunk(40, 60, 3, 6, 2, 4, chunkSize, chunkItems.ToArray(), ground, backGround, chunkPosition);
                    worldInfo.chunks[x][y] = chunk.ChunkInfo;

                    SetTiles(IdsToTiles(worldInfo.chunks[x][y].tileInfo), chunkPosition);
                }
                else
                {
                    worldInfo.chunks[x][y].gPos = chunkPosition;
                    SetTiles(IdsToTiles(worldInfo.chunks[x][y].tileInfo), chunkPosition);
                }
            }
        }
    }
    public void SetWorldLimits()
    {
        
        float dx = width * (chunkSize.x / 2)* 1.75f;
        float dy = -(chunkSize.y) * (heigth) * 1.75f;

        GameObject DeathLimits = new GameObject("DeathLimits");
        DeathLimits.transform.parent = transform;

        DeathLimit.CreateDeathLimit(DeathLimits, Vector3.down, new Vector3(-dx, dy/2+(chunkSize.y / 2), 0), new Vector3(150,heigth+1,0));
        DeathLimit.CreateDeathLimit(DeathLimits, Vector3.up, new Vector3(dx, dy/2+(chunkSize.y / 2), 0), new Vector3(150, heigth+1, 0));
        DeathLimit.CreateDeathLimit(DeathLimits, Vector3.left, new Vector3(0, chunkSize.y, 0), new Vector3(width, 150, 0));
        DeathLimit.CreateDeathLimit(DeathLimits, Vector3.right, new Vector3(0, dy, 0), new Vector3(width,150 , 0));

    }
    public void SetWorldColor(Color color)
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(color, out h, out s, out v);
        //Debug.Log(h + " , " + s + " , " + v);
        s -= UnityEngine.Random.Range(0.15f, 0.3f);  
        v-= UnityEngine.Random.Range(0.15f, 0.35f);
        bg = Color.HSVToRGB(h, s, v);
        Color.RGBToHSV(bg, out h, out s, out v);
        //Debug.Log(h + " , " + s + " , " + v);
        //bg.a = 1;
        backGround.color = bg;
        FindObjectOfType<Camera>().backgroundColor = bg;
        ground.color = color;
    }
    public void SetTiles(Tile[,,] chunkTileInfo, Vector2Int chunkPos)
    {
        for (int y = 0; y < chunkTileInfo.GetLength(1); y++)
        {
            for (int x = 0; x < chunkTileInfo.GetLength(0); x++)
            {
                for (int z = 0; z < chunkTileInfo.GetLength(2); z++)
                {
                    if (chunkTileInfo[x, y, z] != null)
                    {
                        int i = (x + chunkPos.x- (chunkSize.x / 2));
                        int j = y - chunkSize.y + chunkPos.y;

                        tileMaps[z].SetTile(new Vector3Int(i, j, 0), chunkTileInfo[x, y, z]);
                        //Debug.Log("se puso un " + chunkTileInfo[x, y, z].name);
                    }

                }
            }
        }
    }
    public void SetDungeons()
    {
        if(worldInfo.nroDungeons>0)
        {
            List<ChunkInfo> posibleDungeons = XMLManager.LoadFromResourcesData<ChunkInfo>(XMLManager.CHUNKINFO_FOLDER_NAME.Remove(XMLManager.CHUNKINFO_FOLDER_NAME.Length - 1));
            for (int i = 0; i < worldInfo.nroDungeons; i++)
            {
                int x=UnityEngine.Random.Range(1,width);
                int y=UnityEngine.Random.Range(1,heigth);
                worldInfo.chunks[x][y] = posibleDungeons[UnityEngine.Random.Range(0,posibleDungeons.Count)];
            }
        }
    }
    public void SetSpawners()
    {
        if(worldInfo.spawnersPos.Count==0&&worldInfo.nroSpawners>0)
        {
            List<Node> emptyNodes = (from Node n in navMesh.NodesArray where n.BIsWall == false select n).ToList();
            Node spawnNode = null;

            for (int i = 0; i < worldInfo.nroSpawners; i++)
            {
                bool gotaNode = false;
                int trys = 30;
                while (!gotaNode  )
                {
                    if (trys <= 0)
                        break;

                    spawnNode = emptyNodes[UnityEngine.Random.Range(0, emptyNodes.Count)];

                    if (spawners.Count > 0)
                    {
                        int arround = 0;
                        for (int j = 0; j < spawners.Count; j++)
                        {
                            float d = Vector3.Distance(spawnNode.WorldPos, spawners[j].WorldPos);
                            if (d < spawners[j].RadiusSpawnArea* 0.75f)
                            {
                                arround++;
                            }
                        }
                        if (arround == 0)
                            gotaNode = true;

                    }
                    else
                    {
                        float d = Vector3.Distance(spawnNode.WorldPos, Vector3.down*(chunkSize.y*heigth/2));
                        if (d > (chunkSize * worldInfo.worldSizeInChunks).magnitude*(1f/2f))
                        {
                            gotaNode = true;
                        }
                    }
                    trys--;
                }

                if (spawnNode != null)
                {
                    GameObject spawner = Instantiate(SpawnerPrefab, spawnNode.WorldPos, Quaternion.identity, transform);
                    spawners.Add(spawner.GetComponent<Spawner>());
                    worldInfo.spawnersPos.Add(spawnNode.WorldPos);
                }

            }
        }
        else
        {
            for (int i = 0; i < worldInfo.nroSpawners; i++)
            {
                GameObject spawner = Instantiate(SpawnerPrefab, worldInfo.spawnersPos[i], Quaternion.identity, transform);
                spawners.Add(spawner.GetComponent<Spawner>());
            }
        }
        

    }
    public void WriteTileInfo()
    {
        int c_width = chunkSize.x;
        int c_heigth = chunkSize.y;
        int zSize = tileMaps.Count;
        int wr = width * (c_width / 2);


        for (int wy = 0; wy < heigth; wy++)
        {
            for (int wx = 0; wx < width; wx++)
            {
                if(worldInfo.chunks[wx][wy] != null)
                {
                    Vector3Int pos = new Vector3Int(-wr + wx * c_width, -wy * c_heigth - c_heigth,0);
                    BoundsInt myBounds = new BoundsInt(pos, new Vector3Int(c_width, c_heigth, 1));

                    Tile[,,] chunkTileInfo = new Tile[c_width, c_heigth, zSize];
                    for (int i = 0; i < zSize; i++)
                    {

                        TileBase[] tileArray = tileMaps[i].GetTilesBlock(myBounds);

                        for (int x = 0; x < tileArray.Length; x++)
                        {
                            int y = x / c_width;
                            //Debug.Log(x % width+" - "+ y + " - " + i);
                            chunkTileInfo[x % c_width, y, i] = tileArray[x] as Tile;
                        }
                    }
                    worldInfo.chunks[wx][wy].tileInfo = World.TilesToIds(chunkTileInfo);
                }
            }
        }
    }
    public void CheckDestruction(Vector3 pos)
    {
        if (!destroingTile)
        {
            destroingTile = true;
            Vector3Int currentCell = tileMaps[0].WorldToCell(pos);
            float cd = 0;
            for (int i = 0; i < tileMaps.Count; i++)
            {
                TileBase tile = null;
                foreach (var p in new BoundsInt(0, 0, 0, 1, 1, 1).allPositionsWithin)//brush
                {
                    tile = tileMaps[i].GetTile(currentCell + p);
                }
                if (tile is ItemTile)
                {
                    ItemTile item = tile as ItemTile;
                    if (cd < item.Item.hardness)
                        cd = item.Item.hardness;
                }

            }
            if (0 > cd) cd = 0.025f;

            StartCoroutine(DeleteTiles(currentCell, cd));

        }
    }
    public IEnumerator DeleteTiles(Vector3Int position, float Cd)
    {
        yield return new WaitForSeconds(Cd);
        for (int i = 0; i < tileMaps.Count; i++)
        {
            foreach (var p in new BoundsInt(0, 0, 0, 1, 1, 1).allPositionsWithin)//brush
            {
                if (i == 0)
                {
                    tileMaps[i].SetTile(position, null);
                    tileMaps[i].SetTile(position, backGround);

                    Vector2Int nodeIndex = navMesh.GridposToIndex((Vector2Int)position);
                    if (nodeIndex.x >= 0 && nodeIndex.y >= 0 && nodeIndex.x < navMesh.NodesArray.GetLength(0) && nodeIndex.y < navMesh.NodesArray.GetLength(1))
                        navMesh.NodesArray[nodeIndex.x, nodeIndex.y].BIsWall = false;

                }

                else
                    tileMaps[i].SetTile(position, null);
            }
        }
        destroingTile = false;
    }
    private Tilemap SetupTilemap()
    {
        GameObject tm = new GameObject("worldLayer");
        Tilemap tilemap = tm.AddComponent<Tilemap>();
        //tilemap.origin = Vector3Int.FloorToInt(transform.position);
        tm.AddComponent<TilemapRenderer>().material = pixelShader;
        Collider2D ctm = tm.AddComponent<TilemapCollider2D>();
        //ctm.usedByComposite = true;
        Rigidbody2D rtm = tm.AddComponent<Rigidbody2D>();
        rtm.position = transform.position;
        rtm.bodyType = RigidbodyType2D.Static;
        //CompositeCollider2D cctm = tm.AddComponent<CompositeCollider2D>();
        //cctm.generationType = CompositeCollider2D.GenerationType.Synchronous;
        tm.transform.SetParent(transform);
        return tilemap;

    }
    
    #endregion

}

public class TransformUtility
{
    public static void ChangeLayersRecursively(Transform trans, LayerMask layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child,layer);
        }
    }

}
