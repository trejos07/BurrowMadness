using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class NodeNavMesh : MonoBehaviour
{
    Tilemap worldTerrain;
    Vector3Int size;
    int width;
    int heigth;
    Node[,] nodesArray;
    public List<Node> FinalPath = new List<Node>();

    public Node[,] NodesArray
    {
        get
        {
            return nodesArray;
        }

        set
        {
            nodesArray = value;
        }
    }

    public static NodeNavMesh CreateNavMesh(GameObject where, Tilemap worldTerrain)
    {
        where.SetActive(false);
        NodeNavMesh myC = where.AddComponent<NodeNavMesh>();
        myC.worldTerrain = worldTerrain;
        where.SetActive(true);

        return myC;
    }

	#region navmesh

	private void Awake()
	{
        width = worldTerrain.size.x;
        heigth = worldTerrain.size.y;
        GetNodes();
	}
	public Node GetNeighbNode(Node node, Vector2Int dir)
    {
        Node neighbNode = null;
        if (node != null)
        {
            Vector2Int posIndex = node.GPos;
            int icheckX = posIndex.x + dir.x;
            int icheckY = posIndex.y + dir.y;

            if (icheckX >= 0 && icheckX < width && icheckY >= 0 && icheckY < heigth)//If the XPosition is in range of the array
            {
                neighbNode = NodesArray[icheckX, icheckY];
            }
        }
        else
            Debug.Log("el nodo es nulo");

        return neighbNode;
    }
    public List<Node> GetNodesArraund(Node node, int radius)
    {
        List<Node> nodes = new List<Node>();

        if (node != null)
        {
            Vector2Int startPoint = node.GPos;
            for (int i = startPoint.x - radius; i < startPoint.x + radius; i++)
            {
                for (int j = startPoint.y - radius; j < startPoint.y + radius; j++)
                {
                    //Debug.Log("se busca el nodo en el index " + i + " , " + j);
                    if (i >= 0 && i < NodesArray.GetLength(0) && j >= 0 && j < NodesArray.GetLength(1))
                    {
                        if (NodesArray[i, j].BIsWall)
                            continue;

                        if (Math.Pow(i - startPoint.x, 2) + Math.Pow(j - startPoint.y, 2) < Math.Pow(radius, 2))
                        {
                            if (!nodes.Contains(NodesArray[i, j]))
                            {
                                nodes.Add(NodesArray[i, j]);
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
    public Vector2Int GridposToIndex(Vector2Int gridPos)
    {
        int fit = width % 2 == 0 ? 1 : 0;
        int ix = gridPos.x + (width / 2);
        int iy = gridPos.y + heigth;

        return new Vector2Int(ix, iy); ;
    }
    public Node[] GetNeighbNodes(Node _node)
    {
        Node[] nodes = new Node[4];

        Node nodeToAdd = GetNeighbNode(_node, Vector2Int.right);
        if (nodeToAdd != null) nodes[0]=nodeToAdd;

        nodeToAdd = GetNeighbNode(_node, Vector2Int.down);
        if (nodeToAdd != null) nodes[1] = nodeToAdd;

        nodeToAdd = GetNeighbNode(_node, Vector2Int.left);
        if (nodeToAdd != null) nodes[2] = nodeToAdd;

        nodeToAdd = GetNeighbNode(_node, Vector2Int.up);
        if (nodeToAdd != null) nodes[3] = nodeToAdd;

        return nodes;

    }
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        Node node = null;
        Vector2Int index = GridposToIndex((Vector2Int)worldTerrain.WorldToCell(a_vWorldPos));
        node = NodesArray[index.x, index.y];

        return node;
    }
    public Vector3 WordPointFromNode(Node node)
    {
        return worldTerrain.GetCellCenterWorld(node.MPosition);
    }
    public void GetNodes()
    {
        NodesArray = new Node[width, heigth];
        BoundsInt.PositionEnumerator positions = worldTerrain.cellBounds.allPositionsWithin;

        foreach (Vector3Int pos in positions)
        {
            Vector2Int gpos = GridposToIndex((Vector2Int)pos);


            Vector3 worldPoint = worldTerrain.GetCellCenterWorld(pos);
            TerrainTile tile = worldTerrain.GetTile(pos) as TerrainTile;
            bool Wall = tile.colliderType != Tile.ColliderType.None;

            NodesArray[gpos.x, gpos.y] = new Node(pos, gpos, worldPoint, Wall);

        }
    }
    private void OnDrawGizmos()
    {
        if (NodesArray != null)//If the grid is not empty
        {
            foreach (Node n in NodesArray)//Loop through every node in the grid
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
    #endregion
}
