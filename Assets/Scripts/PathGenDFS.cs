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
    public bool IsExit { get { return roomInfo.MazeExit; } }
    public bool IsHideSpot { get { return roomInfo.HideRoom; } }


    public Point gridPoint;

    public GameObject RoomObj;
    public AdjustRoom roomInfo;

    public bool DeadEnd { get; private set; }
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

    public void MarkDeadEnd()
    {
        DeadEnd = true;
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
    [SerializeField] private int MaxDimension = 20;
    [SerializeField] private int MinDimension = 20;
    [SerializeField] private int Width = 10;
    [SerializeField] private int Height = 10;
    [SerializeField] private float SpaceBetween = 1; //in meters
    [SerializeField] private int AllowedHidingSpots; //how many hiding spots we will generate
    [SerializeField] private bool SquareMap;

    [Header("Start Position")]
    [SerializeField] private int startPosX;
    [SerializeField] private int startPosY;
    [SerializeField] private int RecursiveCap;
    private int CurrentI;


    //Map componenets
    private GridNode[,] _gridNodes;
    public List<GridNode> DeadEnds { get; private set;}
    public GridNode SpawnNode { get; private set; }

    public GridNode[,] GridNodes { get { return _gridNodes; } } //Property readonly

    private List<Node> _nodes;
    private Stack<Node> branchingNodes;

    [Header("Debug")]
    [SerializeField] private GameObject sphere;

    public static event EventHandler OnNewMapGenerated;
    void Awake()
    {
        _gridNodes = new GridNode[Width, Height];
        _nodes = new List<Node>();
        branchingNodes = new Stack<Node>();
        DeadEnds = new List<GridNode>();

        CurrentI = 0;

        GenerateGrid();
        CreateRoomsDFS(startPosX, startPosY, OpeningSide.NONE, false);

        //Add Dead-End nodes that might not have been marked properly
        MarkMissingDeadEnds();

        CreateSpawnNode();
        CreatelevelExit();
        CreateRandomHideSpots();
    }

    

    private void Start()
    {
        //Static subscription
        LevelMessanger.LevelFinished += GenerateNewLevel;

        CallNewLevel();
    }

    private void GenerateNewLevel(object sender, EventArgs e)
    {
        ResetMap();

        GenerateGrid();
        CreateRoomsDFS(startPosX, startPosY, OpeningSide.NONE, false);
        
        //Add Dead-End nodes that might not have been marked properly
        MarkMissingDeadEnds();
        
        CreateSpawnNode();
        CreatelevelExit();
        CreateRandomHideSpots();

        CallNewLevel();
    }

    private void OnDisable()
    {
        LevelMessanger.LevelFinished -= GenerateNewLevel;
    }
    private void CallNewLevel()
    {
        OnNewMapGenerated?.Invoke(this, EventArgs.Empty);
    }

    private void ResetMap()
    {
        //Clear Rooms
        //Clear Nodes
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Destroy(_gridNodes[j, i].RoomObj);
                Destroy(_gridNodes[j, i]._transform.gameObject);
            }
        }

        //Reset map iterator
        CurrentI = 0;

        //Reset Data structures, Except gridnodes (in generateGrid)
        _nodes.Clear();
        branchingNodes.Clear();
        DeadEnds.Clear();
    }
    private void GenerateGrid()
    {
        if (!SquareMap)
        {
            Width = UnityEngine.Random.Range(MinDimension, MaxDimension+1);
            Height = UnityEngine.Random.Range(MinDimension, MaxDimension+1);
        }
        else
        {
            Width = UnityEngine.Random.Range(MinDimension, MaxDimension + 1);
            Height = Width;
        }
        _gridNodes = new GridNode[Width, Height];

        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                //Initialize Gridnode to add
                GridNode node = new GridNode();

                //Empty GameObject transform
                GameObject p = new GameObject(); 

                //Set node transform pos.
                p.transform.SetParent(transform);
                p.name = $"Transform {j}";
                p.transform.position += new Vector3(j * SpaceBetween, 0, i * SpaceBetween);
                node._transform = p.transform;
                _gridNodes[j, i] = node;


                //Debug for sphere
                GameObject o = Instantiate(sphere, p.transform.position, transform.rotation);
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
        else
        {
            if (!returning)
            {
                _gridNodes[X, Y].MarkDeadEnd();
                DeadEnds.Add(_gridNodes[X, Y]);
            }            
        }
        
        if (branchingNodes != null && branchingNodes.Count > 0)
        {            
            //Debug.Log("Going back to last branch node");
            Node node = branchingNodes.Pop();
            CreateRoomsDFS(node.X, node.Y, node.EntranceSide, true);
        }
    }
    private void MarkMissingDeadEnds()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (_gridNodes[j, i].roomInfo.ExitList.Count == 1 &&
                    !_gridNodes[j, i].DeadEnd)
                {
                    _gridNodes[j, i].MarkDeadEnd();
                    DeadEnds.Add(_gridNodes[j, i]);
                }
            }
        }
    }
    private void CreateSpawnNode()
    {
        SpawnNode = DeadEnds[0];
        _gridNodes[SpawnNode.gridPoint.X, SpawnNode.gridPoint.Y].roomInfo.SetHideWall(_gridNodes[SpawnNode.gridPoint.X, SpawnNode.gridPoint.Y].roomInfo.EntranceSide);
    }
    private void CreateRandomHideSpots()
    {
        //Dont run if no dead ends
        if (DeadEnds.Count < 1)
            return;

        int choice = 0;
        
        if (AllowedHidingSpots > DeadEnds.Count)
        {
            AllowedHidingSpots = DeadEnds.Count;
        }

        for (int i = 0; i < AllowedHidingSpots; i++)
        {
            choice = UnityEngine.Random.Range(0, DeadEnds.Count);

            Point tempPoint = DeadEnds[choice].gridPoint;

            if (!_gridNodes[tempPoint.X, tempPoint.Y].IsExit && !_gridNodes[tempPoint.X, tempPoint.Y].SpawnNode)
            {
                _gridNodes[tempPoint.X, tempPoint.Y].roomInfo.SetHideWall(_gridNodes[tempPoint.X, tempPoint.Y].roomInfo.EntranceSide);
            }
            else
            {
                i = 0;
            }
        }
    }

    private void CreatelevelExit()
    {
        //Dont run if no dead ends
        if (DeadEnds.Count < 1)
            return;

        int choice = 0;
        int CheckingCap = AllowedHidingSpots + 10; //Arbitrary loop cap


        for (int i = 0; i < CheckingCap; i++)
        {
            choice = UnityEngine.Random.Range(0, DeadEnds.Count);
            Point tempPoint = DeadEnds[choice].gridPoint;

            if (!_gridNodes[tempPoint.X, tempPoint.Y].IsHideSpot && !_gridNodes[tempPoint.X, tempPoint.Y].SpawnNode)
            {
                _gridNodes[tempPoint.X, tempPoint.Y].roomInfo.SetMazeExit();
                return;
            }
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
