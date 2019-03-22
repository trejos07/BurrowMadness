using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node {

    private Vector3Int mPosition;//X Position in the Node Array
    private Vector3 worldPos;


    private bool bIsWall;//Tells the program if this node is being obstructed.

    private Node ParentNode;//For the AStar algoritm, will store what node it previously came from so it cn trace the shortest path.

    private int igCost;//The cost of moving to the next square.
    private int ihCost;//The distance to the goal from this node.

    public Node(Vector3Int mPosition,Vector3 worldPos, bool bIsWall)
    {
        this.mPosition = mPosition;
        //Debug.Log("se puso un nodo en " + mPosition);
        this.bIsWall = bIsWall;
        this.worldPos = worldPos;
        ParentNode = null;
    }

    public int FCost { get { return IgCost + IhCost; } }//Quick get function to add G cost and H Cost, and since we'll never need to edit FCost, we dont need a set function.

    #region Acsesores 
    public Vector3Int MPosition
    {
        get
        {
            return mPosition;
        }
    }


    public bool BIsWall
    {
        get
        {
            return bIsWall;
        }

        set
        {
            bIsWall = value;
        }
    }

    public int IgCost
    {
        get
        {
            return igCost;
        }

        set
        {
            igCost = value;
        }
    }

    public int IhCost
    {
        get
        {
            return ihCost;
        }

        set
        {
            ihCost = value;
        }
    }

    public Node ParentNode1
    {
        get
        {
            return ParentNode;
        }

        set
        {
            ParentNode = value;
        }
    }

    public Vector3 WorldPos
    {
        get
        {
            return worldPos;
        }

        set
        {
            worldPos = value;
        }
    }
    #endregion


}
