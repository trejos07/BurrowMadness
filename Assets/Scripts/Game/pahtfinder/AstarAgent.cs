﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class AstarAgent : MonoBehaviour
{
    [SerializeField] float stopDistance=1.5f;
    AICharacter owner;
    NodeNavMesh nodeMesh;//For referencing the grid class
    private Vector3 startPosition;//Starting position to pathfind from
    private Vector3 targetPosition;//Starting position to pathfind to
    private List<Node> finalPath;
    private List<Vector3> path = new List<Vector3>();
    private bool isStopped;

    public bool IsStopped
    {
        get
        {
            return isStopped;
        }

        set
        {
            isStopped = value;
        }
    }

    public float StopDistance
    {
        get
        {
            return stopDistance;
        }

        set
        {
            stopDistance = value;
        }
    }

    private void Awake()
    {
        owner = GetComponent<AICharacter>();
        nodeMesh = FindObjectOfType<NodeNavMesh>();//Get a reference to the game manager
        isStopped = true;
    }
    void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = nodeMesh.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
        Node TargetNode = nodeMesh.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position

        if (StartNode != null && TargetNode != null)
        {
            FastPriorityQueue<Node> OpenList = new FastPriorityQueue<Node>(200);
            //SimplePriorityQueue<Node> OpenList = new FastPriorityQueue<Node>(maxN_Calculations);//List of nodes for the open list
            HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

            OpenList.Enqueue(StartNode,StartNode.FCost);//Add the starting node to the open list to begin the program
            while (OpenList.Count > 0)//Whilst there is something in the open list
            {
                Node CurrentNode = OpenList.Dequeue();//Create a node and set it to the first item in the open list
                OpenList.ResetNode(CurrentNode);
                //OpenList.Remove(CurrentNode);//Remove that from the open list
                ClosedList.Add(CurrentNode);//And add it to the closed list

                if (CurrentNode == TargetNode)//If the current node is the same as the target node
                {
                    GetFinalPath(StartNode, TargetNode);//Calculate the final path
                }
                //Debug.Log("el nodo en cuestion tiene x vecinos" + CurrentNode.NeighbNodes.Count);
                Node[] nNodes = nodeMesh.GetNeighbNodes(CurrentNode);
                for (int i = 0; i < nNodes.Length; i++)
                {
                    Node NeighborNode = nNodes[i];
                    if (NeighborNode == null) continue;
                    if (NeighborNode.BIsWall || ClosedList.Contains(NeighborNode)) continue;//If the neighbor is a wall or has already been checked // Skip it

                    int MoveCost = CurrentNode.IgCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

                    if (MoveCost < NeighborNode.IgCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                    {
                        NeighborNode.IgCost = MoveCost;//Set the g cost to the f cost
                        NeighborNode.IhCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                        NeighborNode.ParentNode1 = CurrentNode;//Set the parent of the node for retracing steps

                        if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                        {
                            //Add it to the list
                            OpenList.Enqueue(NeighborNode, NeighborNode.FCost);
                        }
                        
                    }
                }
                
            }
            //while (OpenList.Count > 0)
            //{
            //    OpenList.ResetNode(OpenList.Dequeue());
            //}
        }

    }
    void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        List<Node> FinalPath = new List<Node>();//List to hold the path sequentially 
        Node CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.ParentNode1;//Move onto its parent node
        }

        FinalPath.Reverse();//Reverse the path to get the correct order
        if(FinalPath.Count>1)
            FinalPath.Remove(FinalPath[FinalPath.Count - 1]);

        finalPath = FinalPath;//Set the final path
        

    }
    int GetManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.MPosition.x - a_nodeB.MPosition.x);//x1-x2
        int iy = Mathf.Abs(a_nodeA.MPosition.y - a_nodeB.MPosition.y);//y1-y2

        return ix + iy;//Return the sum
    }

    public void SetDestination (Vector3 _pos)
    {
        startPosition = transform.position;
        targetPosition = _pos;
        FindPath(startPosition, targetPosition);
        path = GetPathCornersInWorld();
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }
    public void MoveRandom()
    {
        if (IsStopped)
        {
            startPosition = transform.position;
            targetPosition = GetRandomPointArround(transform.position, 6);
            FindPath(startPosition, targetPosition);
            path = GetPathCornersInWorld();
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());

        }
    }
    public List<Vector3> GetPathCornersInWorld()
    {
        List<Vector3> corners = new List<Vector3>();
        if (finalPath != null)
        {
            for (int i = 0; i < finalPath.Count; i++)
            {
                Vector3 corner = finalPath[i].WorldPos;
                corners.Add(corner);
            }
        }

        return corners;
    }
    public Vector3 GetRandomPointArround(Vector3 pos, int radius)
    {
        Vector3 Destination = pos;

        Node mNode = nodeMesh.NodeFromWorldPoint(pos);
        List<Node> nodes = nodeMesh.GetNodesArraund(mNode, radius);

        int trys = 10;
        Node DestinationNode = null;

        if (nodes != null && nodes.Count != 0)
        {

            DestinationNode = nodes[Random.Range(0, nodes.Count)];
            Destination = DestinationNode.WorldPos;

        }

        return Destination;
    }
    public IEnumerator FollowPath()
    {
        IsStopped = false;
        while (path.Count > 0&&!isStopped)
        {
            float distanceToNextNode = Vector3.Distance(owner.Rigidbody.position, path[0]);
            float distanceToLast = Vector3.Distance(owner.Rigidbody.position, path[path.Count-1]);

            if (distanceToNextNode < 0.4f || distanceToLast<stopDistance)
                path.Remove(path[0]);
            else
                owner.MoveTo(path[0]);

            yield return null;
        }
        IsStopped = true;
    }

    #region BackUp
    //void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    //{
    //    Node StartNode = nodeMesh.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
    //    Node TargetNode = nodeMesh.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position

    //    if (StartNode != null && TargetNode != null)
    //    {
    //        FastPriorityQueue<Node> OpenList = new FastPriorityQueue<Node>(maxN_Calculations);//List of nodes for the open list
    //        HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

    //        OpenList.Enqueue(StartNode,);//Add the starting node to the open list to begin the program
    //        while (OpenList.Count > 0)//Whilst there is something in the open list
    //        {
    //            Node CurrentNode = OpenList[0];//Create a node and set it to the first item in the open list
    //            for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
    //            {
    //                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].IhCost < CurrentNode.IhCost)//If the f cost of that object is less than or equal to the f cost of the current node
    //                {
    //                    CurrentNode = OpenList[i];//Set the current node to that object
    //                }
    //            }
    //            OpenList.Remove(CurrentNode);//Remove that from the open list
    //            ClosedList.Add(CurrentNode);//And add it to the closed list

    //            if (CurrentNode == TargetNode)//If the current node is the same as the target node
    //            {
    //                GetFinalPath(StartNode, TargetNode);//Calculate the final path
    //            }
    //            //Debug.Log("el nodo en cuestion tiene x vecinos" + CurrentNode.NeighbNodes.Count);
    //            Node[] nNodes = nodeMesh.GetNeighbNodes(CurrentNode);
    //            for (int i = 0; i < nNodes.Length; i++)
    //            {
    //                Node NeighborNode = nNodes[i];
    //                if (NeighborNode == null) continue;
    //                if (NeighborNode.BIsWall || ClosedList.Contains(NeighborNode))//If the neighbor is a wall or has already been checked
    //                {
    //                    continue;//Skip it
    //                }
    //                int MoveCost = CurrentNode.IgCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

    //                if (MoveCost < NeighborNode.IgCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
    //                {
    //                    NeighborNode.IgCost = MoveCost;//Set the g cost to the f cost
    //                    NeighborNode.IhCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
    //                    NeighborNode.ParentNode1 = CurrentNode;//Set the parent of the node for retracing steps

    //                    OpenList.Add(NeighborNode);
    //                }
    //            }

    //            #region Foreach
    //            //foreach (Node NeighborNode in nNodes)//Loop through each neighbor of the current node
    //            //{
    //            //    if (NeighborNode == null) continue;
    //            //    if (NeighborNode.BIsWall || ClosedList.Contains(NeighborNode))//If the neighbor is a wall or has already been checked
    //            //    {
    //            //        continue;//Skip it
    //            //    }
    //            //    int MoveCost = CurrentNode.IgCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

    //            //    if (MoveCost < NeighborNode.IgCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
    //            //    {
    //            //        NeighborNode.IgCost = MoveCost;//Set the g cost to the f cost
    //            //        NeighborNode.IhCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
    //            //        NeighborNode.ParentNode1 = CurrentNode;//Set the parent of the node for retracing steps

    //            //        OpenList.Add(NeighborNode);
    //            //        //if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
    //            //        //{
    //            //        //    //Add it to the list

    //            //        //}
    //            //    }
    //            //} 
    //            #endregion

    //        }
    //    }

    //}
    #endregion


    private void OnDrawGizmosSelected()
    {
        if (finalPath != null)//If the final path is not empty
        {
            Gizmos.color = Color.green;
            foreach (Vector3 p in GetPathCornersInWorld())
            {
                Gizmos.DrawWireCube(p, Vector3.one);
            }

        }
    }

}
