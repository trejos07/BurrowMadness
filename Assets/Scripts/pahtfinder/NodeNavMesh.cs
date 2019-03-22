using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeNavMesh : MonoBehaviour
{
    Tilemap worldTerrain;
    Node[,] NodeArray;
    public List<Node> FinalPath = new List<Node>();


    public NodeNavMesh(Tilemap worldTerrain)
    {
        this.worldTerrain = worldTerrain;
        GetNodes();
    }

    public void GetNodes()
    {
        int nArrayWidth = worldTerrain.size.x;
        int nArrayHeigt = worldTerrain.size.y;
        int fit = nArrayWidth % 2 == 0 ? 1 : 0;

        NodeArray = new Node[nArrayWidth, nArrayHeigt];

        BoundsInt.PositionEnumerator positions = worldTerrain.cellBounds.allPositionsWithin;
        foreach (Vector3Int pos in positions)
        {
            int ix = pos.x + Mathf.FloorToInt(nArrayWidth / 2) - fit;
            int iy = pos.y + nArrayHeigt;
            //Debug.Log("se creara un nodo en el indice "+ix+","+iy);
            Vector3 worldPoint = worldTerrain.GetCellCenterWorld(pos);
            TerrainTile tile = worldTerrain.GetTile(pos) as TerrainTile;
            bool Wall = tile.colliderType != Tile.ColliderType.None;

            NodeArray[ix, iy] = new Node(pos, worldPoint, Wall);

        }

        for (int i = 0; i < NodeArray.GetLength(0); i++)
        {
            for (int j = 0; j < NodeArray.GetLength(1); j++)
            {
                Debug.Log("el nodo es nulo ?  " + (NodeArray[i, j] == null));
            }

        }


    }

    public Node GetNeighbNode(Node node, Vector2Int dir)
    {
        Node neighbNode = null;
        if (node != null)
        {
            int nArrayWidth = worldTerrain.size.x;
            int nArrayHeigt = worldTerrain.size.y;

            int fit = nArrayWidth % 2 == 0 ? 1 : 0;

            int icheckX = node.MPosition.x + Mathf.FloorToInt(nArrayWidth / 2) + dir.x - fit;
            int icheckY = node.MPosition.y + nArrayHeigt + dir.y;


            if (icheckX >= 0 && icheckX < nArrayWidth)//If the XPosition is in range of the array
            {
                if (icheckY >= 0 && icheckY < nArrayHeigt)//If the YPosition is in range of the array
                {
                    neighbNode = NodeArray[icheckX, icheckY];//Add the grid to the available neighbors list
                }
            }
        }
        else
            Debug.Log("el nodo es nulo");

        return neighbNode;
    }



    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        Node node = null;
        Vector3Int nodePosInGrid = worldTerrain.WorldToCell(a_vWorldPos);
        for (int i = 0; i < NodeArray.GetLength(0); i++)
        {
            for (int j = 0; j < NodeArray.GetLength(1); j++)
            {
                if (NodeArray[i, j].MPosition == nodePosInGrid)
                {
                    node = NodeArray[i, j];
                    break;
                }
            }

        }
        return node;
    }

    public Vector3 WordPointFromNode(Node node)
    {
        return worldTerrain.GetCellCenterWorld(node.MPosition);
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


                    if (FinalPath != null)//If the final path is not empty
                    {
                        if (FinalPath.Contains(n))//If the current node is in the final path
                        {
                            Gizmos.color = Color.green;//Set the color of that node
                        }

                    }

                    Gizmos.DrawWireCube(WordPointFromNode(n), Vector3.one);//Draw the node at the position of the node.
                }
            }
        }
    }

}
