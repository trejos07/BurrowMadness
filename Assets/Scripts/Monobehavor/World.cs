using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

public class World : MonoBehaviour
{
    
    [SerializeField] Vector2Int worldSizeInChunks;
    [SerializeField] Vector2Int chunkSize;
    [SerializeField] Tile ground,backGround;
    [SerializeField] Item[] worldSources;
    [SerializeField] int maxSpawners;
    [SerializeField] GameObject SpawnerPrefab;


    int width;
    int heigth;

    private List<Spawner> spawners = new List<Spawner>();
    private List<Tilemap> tileMaps;
    private Chunk[,] chunks;
    private Node[,] NodeArray;
    private bool destroingTile = false;


    public delegate void DestructingTile();
    public static event DestructingTile OnDestructingTile;

    private void Awake()
    {
        FindObjectOfType<Player>().OnDiggingToDir += CheckDestruction;
        width = worldSizeInChunks.x;
        heigth = worldSizeInChunks.y;
        chunks = new ProceduralChunk[width, heigth];
        tileMaps = new List<Tilemap>();
        int layers = 2;

        for (int i = 0; i < layers; i++)
        {
            tileMaps.Add(setupTilemap());
        }
        GenerateWorld();
        GetNodes();
        GenerateSpawners();
    }

    #region navmesh

    public Node GetNeighbNode(Node node, Vector2Int dir)
    {
        Node neighbNode = null;
        if (node != null)
        {
            Vector3Int s = tileMaps[0].size;
            int nArrayWidth = s.x;
            int nArrayHeigt = s.y;

            Vector2Int posIndex = GridposToIndex(node.MPosition);
            int icheckX = posIndex.x + dir.x;
            int icheckY = posIndex.y + dir.y;

            if (icheckX >= 0 && icheckX < nArrayWidth&& icheckY >= 0 && icheckY < nArrayHeigt)//If the XPosition is in range of the array
            {
                neighbNode = NodeArray[icheckX, icheckY];
            }

        }
        else
            Debug.Log("el nodo es nulo");

        return neighbNode;
    }

    
    public List<Node>GetNodesArraund(Node node, int radius)
    {
        List<Node> nodes = new List<Node>();

        if (node != null)
        {
            Vector2Int startPoint = GridposToIndex(node.MPosition);
            for (int i = startPoint.x - radius; i < startPoint.x + radius; i++)
            {
                for (int j = startPoint.y - radius; j < startPoint.y + radius; j++)
                {
                    //Debug.Log("se busca el nodo en el index " + i + " , " + j);
                    if (i >= 0 && i < NodeArray.GetLength(0) && j >= 0 && j < NodeArray.GetLength(1))
                    {
                        if (NodeArray[i, j].BIsWall)
                            continue;

                        if (Math.Pow(i - startPoint.x, 2) + Math.Pow(j - startPoint.y, 2) < Math.Pow(radius, 2))
                        {
                            if (!nodes.Contains(NodeArray[i, j]))
                            {
                                nodes.Add(NodeArray[i, j]);
                            }
                        }
                    }
                }
            }

            if (nodes.Contains(node))
                nodes.Remove(node);
        }

        /*
        if(!nodes.Contains(node))
            nodes.Add(node);

        foreach(Node n in GetNeighbNodes(node))
        {
            if (n.BIsWall)
                continue;

            if (!nodes.Contains(n))
                nodes.Add(n);

            if (Length>0)
            {
                foreach (Node gn in GetNodesArraund(n, Length - 1))
                {
                    if (!nodes.Contains(gn))
                        nodes.Add(gn);
                }
            }
        }
        */
        return nodes; 
    }

    public Vector2Int GridposToIndex(Vector3Int gridPos)
    {
        Vector3Int s = tileMaps[0].size;
        int nArrayWidth = s.x;
        int nArrayHeigt = s.y;

        int fit = nArrayWidth % 2 == 0 ? 1 : 0;


        int ix = gridPos.x + (nArrayWidth / 2) - fit;
        int iy = gridPos.y + nArrayHeigt;

        Vector2Int index = new Vector2Int(ix,iy);
        return index;
    }

    public List<Node> GetNeighbNodes(Node _node)
    {
        List<Node> nodes = new List<Node>();

        Node nodeToAdd = GetNeighbNode(_node, Vector2Int.right);
        if (nodeToAdd != null) nodes.Add(nodeToAdd);

        nodeToAdd = GetNeighbNode(_node, Vector2Int.down);
        if (nodeToAdd != null) nodes.Add(nodeToAdd);

        nodeToAdd = GetNeighbNode(_node, Vector2Int.left);
        if (nodeToAdd != null) nodes.Add(nodeToAdd);

        nodeToAdd = GetNeighbNode(_node, Vector2Int.up);
        if (nodeToAdd != null) nodes.Add(nodeToAdd);

        return nodes;

    }

    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        Node node = null;
        Vector2Int index = GridposToIndex(tileMaps[0].WorldToCell(a_vWorldPos));
        node = NodeArray[index.x, index.y];

        //for (int i = 0; i < NodeArray.GetLength(0); i++)
        //{
        //    for (int j = 0; j < NodeArray.GetLength(1); j++)
        //    {
        //        if (NodeArray[i,j].MPosition == nodePosInGrid)
        //        {
        //            node = NodeArray[i,j];
        //            break;
        //        }
        //    }

        //}
        return node;
    }

    public Vector3 WordPointFromNode(Node node)
    {
        return tileMaps[0].GetCellCenterWorld(node.MPosition);
    }

    public void GetNodes()
    {
        int nArrayWidth = tileMaps[0].size.x;
        int nArrayHeigt = tileMaps[0].size.y;
        int fit = nArrayWidth % 2 == 0 ? 1 : 0;

        NodeArray = new Node[nArrayWidth,nArrayHeigt];

        BoundsInt.PositionEnumerator positions = tileMaps[0].cellBounds.allPositionsWithin;
        foreach (Vector3Int pos in positions)
        {
            int ix = pos.x + Mathf.FloorToInt( nArrayWidth / 2)-fit;
            int iy = pos.y + nArrayHeigt;

            //Debug.Log("se creara un nodo en el indice "+ix+","+iy);
            Vector3Int indexpos = new Vector3Int(ix, iy, 0);
            Vector3 worldPoint = tileMaps[0].GetCellCenterWorld(pos);
            TerrainTile tile = tileMaps[0].GetTile(pos) as TerrainTile;
            bool Wall = tile.colliderType!= Tile.ColliderType.None;

            NodeArray[ix,iy] = new Node(pos, worldPoint, Wall);

        }

        for (int i = 0; i < NodeArray.GetLength(0); i++)
        {
            for (int j = 0; j < NodeArray.GetLength(1); j++)
            {
                Debug.Log("el nodo es nulo ?  " + (NodeArray[i,j] == null));
            }
            
        }

       
    }

    #endregion

    public void GenerateSpawners()
    {
        List<Node> emptyNodes = (from Node n in NodeArray where n.BIsWall == false select n).ToList();
        Node spawnNode = null;
        for (int i = 0; i < maxSpawners; i++)
        {
            bool gotaNode = false;
            int trys = 20;
            while (!gotaNode || trys < 0 )
            {
                spawnNode = emptyNodes[UnityEngine.Random.Range(0, emptyNodes.Count)];

                if (spawners.Count > 0)
                {
                    int arround = 0;
                    for (int j = 0; j < spawners.Count; j++)
                    {
                        float d = Vector3.Distance(spawnNode.WorldPos, spawners[0].WorldPos);
                        if (d < spawners[j].RadiusSpawnArea * 2)
                        {
                            arround++;
                        }
                    }
                    if (arround == 0)
                        gotaNode = true;

                }
                else
                    gotaNode = true;

                trys--;
            }

            if(spawnNode!=null)
            {
                GameObject spawner = Instantiate(SpawnerPrefab, spawnNode.WorldPos, Quaternion.identity, transform);
                spawners.Add(spawner.GetComponent<Spawner>());
            }

        }

    }

    public void GenerateWorld()
    {
        for (int y = 0; y < heigth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int chunkPosition = new Vector2Int((-x + (width / 2)) * chunkSize.x, -y * chunkSize.y);

                if (chunks[x,y]==null)
                {
                    Item[] chunkItems = worldSources;
                    chunks[x, y] = new ProceduralChunk(40, 60, 3, 6, 2, 4, chunkSize, chunkItems, ground, backGround);
                }

                setTiles(chunks[x, y].TileInfo, chunkPosition);
            }
        }
    }

    Tilemap setupTilemap()
    {
        GameObject tm = new GameObject();
        Tilemap tilemap =  tm.AddComponent<Tilemap>();
        tm.AddComponent<TilemapRenderer>();
        tm.AddComponent<TilemapCollider2D>();
        tm.transform.position = transform.position;
        tm.transform.SetParent(transform);

        return tilemap;
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
                        tileMaps[z].SetTile(new Vector3Int(-x + chunkSize.x / 2 + chunkPos.x, y - chunkSize.y + chunkPos.y, 0), chunkTileInfo[x, y, z]);
                        Debug.Log("se puso un " + chunkTileInfo[x, y, z].name);
                    }

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
            float cd=0;
            for (int i = 0; i < tileMaps.Count; i++)
            {
                TileBase tile=null;
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
                else if (tile is TerrainTile)
                {
                    if (cd < 0.1f)cd = 0.1f;
                }
            }
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

                    Vector2Int nodeIndex = GridposToIndex(position);
                    if (nodeIndex.x >= 0 && nodeIndex.y >= 0 && nodeIndex.x < NodeArray.GetLength(0) && nodeIndex.y < NodeArray.GetLength(1))
                        NodeArray[nodeIndex.x, nodeIndex.y].BIsWall = false;

                }

                else
                    tileMaps[i].SetTile(position, null);
            }
        }
        destroingTile = false;
    }
    
    private void OnDrawGizmos()
    {
        if (NodeArray != null)//If the grid is not empty
        {
            foreach (Node n in NodeArray)//Loop through every node in the grid
            {
                if (n != null)
                {
                    if (n.BIsWall)//If the current node is a wall node
                    {
                        Gizmos.color = Color.yellow;//Set the color of the node
                    }
                    else
                    {
                        Gizmos.color = Color.white;//Set the color of the node
                    }

                    Gizmos.DrawWireCube(WordPointFromNode(n), Vector3.one);//Draw the node at the position of the node.
                }
            }
        }
    }


}
