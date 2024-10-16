using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class PathNode
{
    public float F { get; private set; } //Total Cost, G + H
    public float G { get; private set; } //Dist. from start node
    public float H { get; private set; } //Hueristic Dist to end node

    public PathNode NodeFrom { get; private set; }
    public Point gridPoint { get; private set; } //PathNode point
    public PathNode(Point thisPoint, Point startPoint, Point endNode)
    {
        gridPoint = thisPoint;

        //Calc G
        G = Mathf.Abs((startPoint.X - thisPoint.X)) + Mathf.Abs((startPoint.Y - thisPoint.Y));

        //Calc H
        H = Mathf.Abs(endNode.X - thisPoint.X) + Mathf.Abs(endNode.Y - thisPoint.Y);

        //Total
        F = H + G;
    }
    public PathNode() { }

    public void SetParentNode(PathNode p)
    {
        NodeFrom = p;
    }

    public void HardSetF()
    {
        F = 0.0f;
    }
}

public class PathFinder : MonoBehaviour, IPathFinder, ICompActivate
{
    [Header("Maze generator")]
    [SerializeField] private PathGenDFS _pathGenerator;

    //Map copy
    private GridNode[,] Map;
    private PathNode finalNode;
    private PathNode startNode;
    private PathNode currentNode;

    //PathNode & GridNode data structures
    private List<GridNode> OpenNodes;
    private List<PathNode> OpenPathNodes;
    private List<GridNode> ClosedNodes;
    private Stack<PathNode> PathStack;
    private PathNode TargetNode;

    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;
    private Point CurrentPoint;
    private Vector3 TargetPos;

    //Path moving
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MoveCoolDown;
    private float currentCooldown;
    private Vector3 NextDir;


    //Path state properties
    public bool Traverse { get; private set; }
    public bool Calculating { get; private set; }
    private bool Moving;
    private int PathCount;
    private bool Found;

    //Active state of pathfinding 
    public ActiveState _activeState { get; private set; }

    public event EventHandler OnReachedPoint;

    private void Awake()
    {
        LevelMessanger.MapReady += UpdateMap;
    }
    private void Start()
    {
        //_pathGenerator.OnNewMapGenerated += UpdateMap;

        //Set the map
        Map = _pathGenerator.GridNodes;

        OpenNodes = new List<GridNode>();
        OpenPathNodes = new List<PathNode>();
        ClosedNodes = new List<GridNode>();
        PathStack = new Stack<PathNode>();

        currentNode = new PathNode();//null node
        TargetNode = new PathNode();//null node

        currentCooldown = MoveCoolDown;


        //Start DISABLED
        DisableComponent();

    }

    //Get Updated copy of the maze Map
    private void UpdateMap(object sender, System.EventArgs e)
    {
        Map = _pathGenerator.GridNodes;
    }


    #region Behavior Toggle
    public void ActivateComponent()
    {
        SpawnAtRandomEnd();
        _activeState = ActiveState.ACTIVE;        
    }

    public void WarmStartPosition()
    {
        SpawnAtRandomEnd();
    }

    public void DisableComponent()
    {
        StopAllCoroutines();//Stop pathfinding coroutine
        Calculating = false;

        StopTraverse(); //Stop update-loop methods

        ResetDataStructures();
        CurrentPoint = null;
        _activeState = ActiveState.DISABLED;

        //Move out of map
        transform.position = new Vector3(-100,0,-100);
    }
    #endregion


    private void ResetDataStructures()
    {
        //Clear all Data Structures
        OpenNodes.Clear();
        ClosedNodes.Clear();
        OpenPathNodes.Clear();
        PathStack.Clear();
        currentNode = new PathNode();//null node
    }

    //Used Swfit's article as guide https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
    private IEnumerator FindBestPath(Point start, Point end)
    {
        //Set Calculating state
        Calculating = true;

        Found = false;
        endPos.x = end.X;
        endPos.y = end.Y;

        //Return if final point isn't valid
        if (!RoomExists(end))
            yield return null;


        //Set start & end nodes
        startNode = new PathNode(start, start, end);
        startNode.HardSetF();

        finalNode = new PathNode(end, start, end);
        

        currentNode = startNode;

        //Add start initially
        OpenNodes.Add(Map[startNode.gridPoint.X, startNode.gridPoint.Y]);
        OpenPathNodes.Add(currentNode);

        int iteration = 0;

        while (OpenNodes.Count > 0  && !Found)
        {
            //Debug.Log("Calulating");
            iteration++;

            //Set current pathNode as next lowest F
            currentNode = FindLowestF(ref OpenPathNodes);

            //Check if current is finalNode
            if ((currentNode.gridPoint.X == finalNode.gridPoint.X) && (currentNode.gridPoint.Y == finalNode.gridPoint.Y))
            {
                //path complete, trace steps
                //Debug.Log("Found Path!");
                Found = true;


                //Remove is hideSpot
                if (Map[finalNode.gridPoint.X, finalNode.gridPoint.Y].IsHideSpot ||
                    Map[finalNode.gridPoint.X, finalNode.gridPoint.Y].IsExit)
                {
                    finalNode = currentNode.NodeFrom;
                    PathStack.Push(finalNode);
                }
                else
                {
                    PathStack.Push(currentNode);
                }

                yield return null;
            }

            //Remove current node from Open List, add to Closed List
            if (ListContainsPoint(currentNode.gridPoint, ref OpenNodes, out int opind))
            {
                GridNode swap = OpenNodes[opind];
                OpenNodes.RemoveAt(opind);
                OpenPathNodes.RemoveAt(opind);
                ClosedNodes.Add(swap);
            }

            //Get neighbors of current node
            List<PathNode> neighbors = GetSurroundingNeighbors(currentNode.gridPoint);

            //IF we have neighbors, find next lowest F
            if (neighbors.Count > 0)
            {

                for (int i = 0; i < neighbors.Count; i++)
                {
                    //Move on if node was already checked
                    if (ListContainsPoint(neighbors[i].gridPoint, ref ClosedNodes, out int uuu))
                    {
                        continue;
                    }
                    //Else, Add neighbor to open list
                    else if (!ListContainsPoint(neighbors[i].gridPoint, ref OpenNodes, out int uu))
                    {
                        neighbors[i].SetParentNode(currentNode);
                        OpenNodes.Add(Map[neighbors[i].gridPoint.X, neighbors[i].gridPoint.Y]);
                        OpenPathNodes.Add(neighbors[i]);
                    }
                }                
            }

            yield return null;
        }

        //Debug.Log("Done");

        ShowBranchNode(currentNode);

        TraversePath();

        //Set Calculating state
        Calculating = false;
        yield return null;
    }

    private void ShowBranchNode(PathNode node)
    {
        if (node.NodeFrom != null)
        {
            //Debug.Log($"From ({node.gridPoint.X},{node.gridPoint.Y}) \n To ({node.NodeFrom.gridPoint.X},{node.NodeFrom.gridPoint.Y})");
            PathStack.Push(node.NodeFrom);
            ShowBranchNode(node.NodeFrom);
        }
    }


    private PathNode FindLowestF(ref List<PathNode> list)
    {
        /* Old using Gridnode list
        //Initialize pathNode list
        List<PathNode> converted = new List<PathNode>();

        //Convert GridNodes to PathNodes
        for (int i = 0; i < list.Count; i++)
        {
            converted.Add(new PathNode(list[i].gridPoint, startNode.gridPoint, finalNode.gridPoint));
        }

        //Sift through nodes for lowest
        PathNode lowest = converted[0];
        for (int i = 0; i < converted.Count; i++)
        {
            if (converted[i].F <= lowest.F)
            {
                lowest = converted[i];
            }
        }
        */

        PathNode lowest = new PathNode();
        lowest = list[0];

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].F <= lowest.F)
            {
                lowest = list[i];
            }
        }

        return lowest;
    }
    private List<PathNode> GetSurroundingNeighbors(Point p)
    {
        List<PathNode> n_nodes = new List<PathNode>();

        //Neighbor points
        Point N_point = new Point(p.X, p.Y + 1);
        Point E_point = new Point(p.X + 1, p.Y);
        Point S_point = new Point(p.X, p.Y - 1);
        Point W_point = new Point(p.X - 1, p.Y);

        for (int i = 0; i < Map[p.X, p.Y].roomInfo.ExitList.Count; i++)
        {
            switch (Map[p.X, p.Y].roomInfo.ExitList[i])
            {
                case OpeningSide.N:
                    if (RoomExists(N_point))
                    {
                        if (Map[N_point.X, N_point.Y].roomInfo.ContainsOpenings(OpeningSide.S,OpeningSide.S))
                        {
                            n_nodes.Add(new PathNode(N_point, startNode.gridPoint, finalNode.gridPoint));
                        }
                    }

                    break;
                case OpeningSide.E:
                    if (RoomExists(E_point))
                    {
                        if (Map[E_point.X, E_point.Y].roomInfo.ContainsOpenings(OpeningSide.W, OpeningSide.W))
                        {
                            n_nodes.Add(new PathNode(E_point, startNode.gridPoint, finalNode.gridPoint));

                        }
                    }

                    break;
                case OpeningSide.S:
                    if (RoomExists(S_point))
                    {
                        if (Map[S_point.X, S_point.Y].roomInfo.ContainsOpenings(OpeningSide.N,OpeningSide.N))
                        {
                            n_nodes.Add(new PathNode(S_point, startNode.gridPoint, finalNode.gridPoint));

                        }
                    }

                    break;
                case OpeningSide.W:
                    if (RoomExists(W_point))
                    {
                        if (Map[W_point.X, W_point.Y].roomInfo.ContainsOpenings(OpeningSide.E, OpeningSide.E))
                        {
                            n_nodes.Add(new PathNode(W_point, startNode.gridPoint, finalNode.gridPoint));

                        }
                    }

                    break;
                case OpeningSide.NONE:
                    break;
                default:
                    break;
            }
        }

        return n_nodes;
    }

    private bool RoomExists(Point p)
    {
        //First, check if in bounds
        if ( (p.X >= 0 && p.X < Map.GetLength(0)) && (p.Y >= 0 && p.Y < Map.GetLength(1)) )
        {
            //Second, check if node isn't empty
            if (Map[p.X, p.Y].Visited)
            {
                return true;
            }
        }
        return false;
    }

    private bool ListContainsPoint(Point p, ref List<GridNode> list, out int index)
    {
        index = -1;

        for (int i = 0; i < list.Count; i++)
        {
            if ((list[i].gridPoint.X == p.X) && (list[i].gridPoint.Y == p.Y))
            {
                index = i;
                return true;
            }

        }

        return false;
    }

    private void SpawnAtRandomEnd()
    {
        foreach (var item in _pathGenerator.DeadEnds)
        {
            if (!item.IsExit && !item.IsHideSpot)
            {
                HardMoveToPoint(item.gridPoint);
                CurrentPoint = item.gridPoint;
                break;
            }
        }

        if (CurrentPoint == null)
        {
            CurrentPoint = GetRandomPoint();
            HardMoveToPoint(CurrentPoint);
        }

        //Set target pos to self to not move at start
        TargetPos = Map[CurrentPoint.X, CurrentPoint.Y]._transform.position;
    }
    private void HardMoveToPoint(Point p)
    {
        transform.position = Map[p.X, p.Y]._transform.position;
    }

    private void Update()
    {
        if (Traverse)
        {
            if (Moving)
            {
                if (transform.position != TargetPos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, TargetPos, MoveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(NextDir), 1000 * Time.deltaTime);
                }
                else
                {
                    if (currentCooldown < MoveCoolDown)
                    {
                        currentCooldown += Time.deltaTime;
                    }
                    else
                    {
                        Moving = false;
                    }

                    //If we reached finalNode, stop
                    if (transform.position == Map[finalNode.gridPoint.X, finalNode.gridPoint.Y]._transform.position)
                    {
                        Traverse = false;
                        OnReachedPoint?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            else
            {
                //If we still have more nodes to visit, queue them
                if (PathStack.Count > 0)
                {
                    Point n = PathStack.Pop().gridPoint;
                    CurrentPoint = n;
                    TargetPos = Map[n.X, n.Y]._transform.position;
                    currentCooldown = 0.0f;
                    Vector3 dirTo = (TargetPos - transform.position).normalized;
                    if (dirTo == Vector3.right)
                    {
                        NextDir = new Vector3 (0, 90, 0);
                    }
                    else if (dirTo == -Vector3.right)
                    {
                        NextDir = new Vector3(0, -90, 0);
                    }
                    else if (dirTo == Vector3.forward)
                    {
                        NextDir = new Vector3(0, 0, 0);
                    }
                    else if (dirTo == -Vector3.forward)
                    {
                        NextDir = new Vector3(0, -180, 0);
                    }

                    Moving = true;
                }
            }


            

        }
    }

    //Public interface methods
    public void SetNewDestination(Point endPoint)
    {
        StopTraverse();
        ResetDataStructures();

        //Check if endPoint exists
        if (RoomExists(endPoint))
        {
            StartCoroutine(FindBestPath(CurrentPoint, endPoint));
        }
    }

    public void SetSpeed(float speed)
    {
        MoveSpeed = speed;
    }

    public void SetCoolDown(float newCool)
    {
        MoveCoolDown = newCool;
    }

    public void TraversePath()
    {
        //Return if no path queued
        if (PathStack.Count < 1)
            return;

        Traverse = true;
        PathCount = PathStack.Count;
    }
    public void StopTraverse()
    {
        Traverse = false;
        //transform.position = Map[CurrentPoint.X, CurrentPoint.Y]._transform.position;
    }
    public Point GetRandomPoint()
    {
        Point r_point = new Point(0, 0);

        int cap = Map.GetLength(0) * Map.GetLength(1);

        int x = 0;
        int y = 0;

        for (int i = 0; i < cap; i++)
        {
            x = UnityEngine.Random.Range(0, Map.GetLength(0));
            y = UnityEngine.Random.Range(0, Map.GetLength(1));

            r_point.SetPoint(x, y);

            if (RoomExists(r_point))
            {
                if (!Map[r_point.X, r_point.Y].IsHideSpot && !Map[r_point.X, r_point.Y].IsExit)
                {
                    //Found valid point, leave
                    break;
                }
            }

            r_point.SetPoint(0, 0);
        }

        return r_point;
    }

    //Unsubscribes
    private void OnDisable()
    {
        LevelMessanger.MapReady -= UpdateMap;
    }
}
