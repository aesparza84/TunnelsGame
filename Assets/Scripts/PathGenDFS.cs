using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public enum OpeningSide { N, E, S, W, NONE }

/// <summary>
/// X - Position X
/// Y - Position Y
/// Entrance - Next opening needed 
/// </summary>
public class Node
{
    public int X;

    public int Y;

    public OpeningSide EntranceSide;
    public Node(int x, int y, OpeningSide OpeningNeeded)
    {
        X = x;
        Y = y;
        EntranceSide = OpeningNeeded;
    }
    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }
}
public class GridNode
{
    public Transform _transform;
    public bool Visited;

    public bool SpawnNode;

    public Point gridPoint;

    public GameObject RoomObj;

    public AdjustRoom roomInfo;
    public GridNode()
    {
        gridPoint = new Point(0, 0);
        Visited = false;
    }

    public void VisitNode(int x, int y)
    {
        gridPoint.X = x;
        gridPoint.Y = y;
        Visited = true;
    }

    public void SetRoomInfo(AdjustRoom r)
    {
        roomInfo = r;
    }

    public void SetRoomObj(GameObject newRoom)
    {
        RoomObj = newRoom;
    }

    public bool HasZeroExits()
    {
        return roomInfo.HasZeroExits();
    }
}
public class PathGenDFS : MonoBehaviour
{
    [Header("Base Room Prefab")]
    [SerializeField] private GameObject AdjustablePrefab;

    [Header("Map Info")]
    [SerializeField] private int Width = 10;
    [SerializeField] private int Height = 10;
    [SerializeField] private float SpaceBetween = 1; //in meters

    [Header("Start Position")]
    [SerializeField] private int startPosX;
    [SerializeField] private int startPosY;
    [SerializeField] private int RecursiveCap;
    private int CurrentI;

    //Map componenets
    private GridNode[,] _gridNodes;
    public GridNode SpawnNode { get; private set; }

    public GridNode[,] GridNodes { get { return _gridNodes; } } //Property readonly

    private List<Node> _nodes;
    private Stack<Node> branchingNodes;

    [Header("Debug")]
    [SerializeField] private GameObject sphere;
    void Awake()
    {
        _gridNodes = new GridNode[Width, Height];
        _nodes = new List<Node>();
        branchingNodes = new Stack<Node>();
        
        CurrentI = 0;

        GenerateGrid();

        CreateRoomsDFS(startPosX, startPosY, OpeningSide.NONE, false);
    }


    private void GenerateGrid()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                //Initialize Gridnode to add
                GridNode node = new GridNode();
                GameObject p = new GameObject(); //Empty GameObject transform

                GameObject inst = Instantiate(p, transform.position, transform.rotation);
                inst.transform.position += new Vector3(j * SpaceBetween, 0, i * SpaceBetween);

                node._transform = inst.transform;
                _gridNodes[j, i] = node;

                inst.transform.SetParent(transform);
                //_transforms[j, i] = inst.transform;


                //Debug for sphere
                GameObject o = Instantiate(sphere, inst.transform.position, transform.rotation);
            }
        }
    }
    private void CreateRoomsDFS(int X, int Y, OpeningSide branchingFrom, bool returning)
    {
        if (CurrentI > RecursiveCap)
        {
            return;
        }

        CurrentI++;

        //AdjustRoom r = null; //Room reference

        if (!returning)
        {
            //Instantiate the room at (X,Y). Default Room Exits/Entrances
            GameObject n = Instantiate(AdjustablePrefab, _gridNodes[X, Y]._transform.position, AdjustablePrefab.transform.rotation);
            _gridNodes[X,Y].SetRoomInfo(n.GetComponent<AdjustRoom>());



            //Cut out entrance from previous Node's direction, if applicable
            if (branchingFrom != OpeningSide.NONE)
            {
                switch (branchingFrom)
                {
                    case OpeningSide.N:
                        _gridNodes[X, Y].roomInfo.SetEntrance(OpeningSide.N);

                        break;
                    case OpeningSide.E:
                        _gridNodes[X, Y].roomInfo.SetEntrance(OpeningSide.E);

                        break;
                    case OpeningSide.S:
                        _gridNodes[X, Y].roomInfo.SetEntrance(OpeningSide.S);

                        break;
                    case OpeningSide.W:
                        _gridNodes[X, Y].roomInfo.SetEntrance(OpeningSide.W);

                        break;
                    case OpeningSide.NONE:

                        break;
                    default:
                        break;
                }

                //r.CutOutDoors();
            }

            //Visit the current node we are in
            _gridNodes[X, Y].VisitNode(X, Y);
            _gridNodes[X, Y].SetRoomObj(n);

            //If this is the first node created
            if (CurrentI == 1)
            {
                SpawnNode = _gridNodes[X, Y];
            }
        }
        

        

        //Check for valid neighboring nodes
        List<Node> n_nodes = CheckNodeneighbors(X, Y);

        if (_gridNodes[X,Y] == null)
        {
            _gridNodes[X,Y].SetRoomInfo(_gridNodes[X, Y].RoomObj.GetComponent<AdjustRoom>());
        }

        //If this Nodes branches, push to stack for checkpoint
        if (n_nodes.Count > 0)
        {            
            branchingNodes.Push(new Node(X, Y, branchingFrom));

            int choice = UnityEngine.Random.Range(0, n_nodes.Count);

            Node nextNode = n_nodes[choice];

            switch (nextNode.EntranceSide)
            {
                case OpeningSide.N:
                    _gridNodes[X,Y].roomInfo.SetExits(new OpeningSide[] { OpeningSide.S });

                    break;
                case OpeningSide.E:
                    _gridNodes[X, Y].roomInfo.SetExits(new OpeningSide[] { OpeningSide.W });

                    break;
                case OpeningSide.S:
                    _gridNodes[X, Y].roomInfo.SetExits(new OpeningSide[] { OpeningSide.N });

                    break;
                case OpeningSide.W:
                    _gridNodes[X, Y].roomInfo.SetExits(new OpeningSide[] { OpeningSide.E });

                    break;
                case OpeningSide.NONE:
                    break;
                default:
                    break;
            }

            //_gridNodes[X, Y].roomInfo.CutOutDoors();

            //Debug.Log($"Next ({nextNode.X}, {nextNode.Y} | {nextNode.EntranceSide})");
            CreateRoomsDFS(nextNode.X, nextNode.Y, nextNode.EntranceSide, false);
        }
        
        if (branchingNodes != null && branchingNodes.Count > 0)
        {            
            //Debug.Log("Going back to last branch node");
            Node node = branchingNodes.Pop();
            CreateRoomsDFS(node.X, node.Y, node.EntranceSide, true);
        }
    }

    private List<Node> CheckNodeneighbors(int X, int Y)
    {
        List<Node> neighbors = new List<Node>();

        //Check N/E/S/W

        if (CheckValidPoint(X,Y + 1)) //North
        {
            neighbors.Add(new Node(X, Y+1, OpeningSide.S));
        }
        if (CheckValidPoint(X + 1, Y)) //East
        {
            neighbors.Add(new Node(X + 1, Y, OpeningSide.W));
        }
        if (CheckValidPoint(X, Y - 1)) //South
        {
            neighbors.Add(new Node(X, Y - 1, OpeningSide.N));

        }
        if (CheckValidPoint(X - 1, Y)) //West
        {
            neighbors.Add(new Node(X - 1, Y, OpeningSide.E));

        }
        return neighbors;
    }

    private bool CheckValidPoint(int x, int y)
    {
        return (x >= 0 && x < Width) && (y >= 0 && y < Height) && !_gridNodes[x, y].Visited;
    }

    //Public for map traversal
    public bool CheckForExistingRoom(int x, int y)
    {
        //First chcek if we are in bounds
        if (!CheckForInBounds(x, y))
            return false;

        return _gridNodes[x, y].Visited;
    }
    private bool CheckForInBounds(int x, int y)
    {
        return (x >= 0 && x < Width) && (y >= 0 && y < Height);
    }
}
