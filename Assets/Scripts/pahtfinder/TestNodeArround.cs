using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNodeArround : MonoBehaviour
{
    World world;
    List<Node> nodes = new List<Node>();

    private void Awake()
    {
        world= FindObjectOfType<World>();
    }

    private void Update()
    {
        GetNodes();
    }

    void GetNodes()
    {
        Node mNode = world.NodeFromWorldPoint(transform.position);
        nodes = world.GetNodesArraund(mNode, 6);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (nodes != null)//If the grid is not empty
        {
            foreach (Node n in nodes)//Loop through every node in the grid
            {
                if (n != null)
                {
                    Gizmos.DrawWireSphere(world.WordPointFromNode(n), 0.8f);//Draw the node at the position of the node.
                }
            }
        }
    }
}
