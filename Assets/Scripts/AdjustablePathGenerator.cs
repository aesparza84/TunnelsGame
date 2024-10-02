using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustablePathGenerator : MonoBehaviour
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

    //North Entrance Options
    List<OpeningSide[]> N_Options;
    OpeningSide[] N_Down = new OpeningSide[] {OpeningSide.S};
    OpeningSide[] N_Left = new OpeningSide[] {OpeningSide.W};
    OpeningSide[] N_Right = new OpeningSide[] {OpeningSide.E};
    OpeningSide[] N_TWay = new OpeningSide[] {OpeningSide.E, OpeningSide.W};
    OpeningSide[] N_All = new OpeningSide[] {OpeningSide.E, OpeningSide.W, OpeningSide.S};

    //East Entrance Options
    List<OpeningSide[]> E_Options;
    OpeningSide[] E_Left = new OpeningSide[] { OpeningSide.W};
    OpeningSide[] E_Up = new OpeningSide[] { OpeningSide.N};
    OpeningSide[] E_Down = new OpeningSide[] { OpeningSide.S};
    OpeningSide[] E_TWay = new OpeningSide[] {OpeningSide.N, OpeningSide.S};
    OpeningSide[] E_All = new OpeningSide[] { OpeningSide.N, OpeningSide.W, OpeningSide.S};

    //South Entrance Options
    List<OpeningSide[]> S_Options;
    OpeningSide[] S_Up = new OpeningSide[] { OpeningSide.N};
    OpeningSide[] S_Left = new OpeningSide[] { OpeningSide.W};
    OpeningSide[] S_Right = new OpeningSide[] { OpeningSide.E};
    OpeningSide[] S_TWay = new OpeningSide[] {OpeningSide.W, OpeningSide.E};
    OpeningSide[] S_All = new OpeningSide[] { OpeningSide.W, OpeningSide.N, OpeningSide.E};

    //West Entrance Options
    List<OpeningSide[]> W_Options;
    OpeningSide[] W_Up = new OpeningSide[] { OpeningSide.N};
    OpeningSide[] W_Right = new OpeningSide[] {OpeningSide.E};
    OpeningSide[] W_Down = new OpeningSide[] {OpeningSide.S};
    OpeningSide[] W_TWay = new OpeningSide[] {OpeningSide.N, OpeningSide.S};
    OpeningSide[] W_All = new OpeningSide[] {OpeningSide.N, OpeningSide.E, OpeningSide.S};

    List<OpeningSide[]> X_MinOptions;
    List<OpeningSide[]> X_MaxOptions;
    List<OpeningSide[]> Y_MinOptions;
    List<OpeningSide[]> Y_MaxOptions;


    private GridNode[,] _gridNodes;
    private List<Node> _nodes;
    private Stack<Node> _nodeStack;

    [Header("Debug")]
    [SerializeField] private GameObject sphere;
    private void Start()
    {
        _gridNodes = new GridNode[Width, Height];
        _nodes = new List<Node>();
        _nodeStack = new Stack<Node>();

        #region Base Layouts
        //Layout Lists
        N_Options = new List<OpeningSide[]>();
        N_Options.Add(N_Down);
        N_Options.Add(N_Left);
        N_Options.Add(N_Right);
        N_Options.Add(N_TWay);
        N_Options.Add(N_All);

        E_Options = new List<OpeningSide[]>();
        E_Options.Add(E_Left);
        E_Options.Add(E_Up);
        E_Options.Add(E_Down);
        E_Options.Add(E_TWay);
        E_Options.Add(E_All);

        S_Options = new List<OpeningSide[]>();
        S_Options.Add(S_Up);
        S_Options.Add(S_Left);
        S_Options.Add(S_Right);
        S_Options.Add(S_TWay);
        S_Options.Add(S_All);

        W_Options = new List<OpeningSide[]>();
        W_Options.Add(W_Up);
        W_Options.Add(W_Right);
        W_Options.Add(W_Down);
        W_Options.Add(W_TWay);
        W_Options.Add(W_All);
        #endregion

        #region Boundary Pieces
        X_MinOptions = new List<OpeningSide[]>();
        X_MinOptions.Add(E_TWay);
        X_MinOptions.Add(N_Down);

        X_MaxOptions = new List<OpeningSide[]>();
        X_MaxOptions.Add(W_TWay);
        X_MaxOptions.Add(S_Up);

        Y_MinOptions = new List<OpeningSide[]>();
        Y_MinOptions.Add(W_Right);
        Y_MinOptions.Add(W_Up);
        Y_MinOptions.Add(N_TWay);

        Y_MaxOptions = new List<OpeningSide[]>();
        Y_MaxOptions.Add(W_Down);
        Y_MaxOptions.Add(W_Right);
        Y_MaxOptions.Add(S_TWay);
        #endregion

        CurrentI = 0;

        GenerateGrid();

        CreateRoom(startPosX, startPosY, OpeningSide.S, new OpeningSide[] { OpeningSide.E, OpeningSide.N});
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

    private void GenerateRooms()
    {
        //Create initial room
        CreateRoom(startPosX, startPosY, OpeningSide.S, new OpeningSide[] { OpeningSide.N});
    }
    
    private void CreateRoom(int X, int Y, OpeningSide Entr, OpeningSide[] Exits)
    {
        if (CurrentI > RecursiveCap)
        {
            return;
        }

        CurrentI++;

        //Instantiate the room at (X,Y).
        GameObject n = Instantiate(AdjustablePrefab, _gridNodes[X,Y]._transform.position, AdjustablePrefab.transform.rotation);
        AdjustRoom r = n.GetComponent<AdjustRoom>();

        //Create Entrance and Exits
        r.SetEntrance(Entr);
        r.SetExits(Exits);
        r.CutOutDoors();

        //Mark current node 'Visited'
        _gridNodes[X, Y].VisitNode();

        //If node has an exit at 'X-side' check if we can move further.
        //If so, then add that node to the stack<Node> of nodes to affect.
        //If a node has been visited OR out of bounds, skip.
        for (int i = 0; i < Exits.Length; i++)
        {
            switch (Exits[i])
            {
                case OpeningSide.N:
                    if (CheckValidPoint(X,Y+1))
                    {
                        //_nodes.Add(new Node(X, Y+1, OpeningSide.S));
                        _nodeStack.Push(new Node(X, Y+1, OpeningSide.S));
                    }

                    break;
                case OpeningSide.E:
                    if (CheckValidPoint(X+1, Y))
                    {
                       //_nodes.Add(new Node(X+1, Y, OpeningSide.W));
                        _nodeStack.Push(new Node(X+1, Y, OpeningSide.W));
                    }

                    break;
                case OpeningSide.S:
                    if (CheckValidPoint(X, Y - 1))
                    {
                        //_nodes.Add(new Node(X, Y - 1, OpeningSide.N));
                        _nodeStack.Push(new Node(X, Y - 1, OpeningSide.N));
                    }

                    break;
                case OpeningSide.W:
                    if (CheckValidPoint(X-1, Y))
                    {
                        //_nodes.Add(new Node(X-1, Y, OpeningSide.E));
                        _nodeStack.Push(new Node(X-1, Y, OpeningSide.E));
                    }

                    break;
                case OpeningSide.NONE:
                    break;
                default:
                    break;
            }
        }

        //For each node we added, add a piece that has entrance on side 'X'
        int choice;

        if (_nodeStack != null && _nodeStack.Count > 0)
        {
            Node newNode = _nodeStack.Pop();

            //1. Straight
            //2. L-Turn
            //3. R-Turn
            //4. T-Intersection (3 Way)
            //5. + Intersection (4 Way)
            //6. Dead End
            choice = UnityEngine.Random.Range(0, 5);

            //If the next node will be on the (0,y) X_Min
            if (newNode.X == 0)
            {
                choice = UnityEngine.Random.Range(0, X_MinOptions.Count);
                CreateRoom(newNode.X, newNode.Y, Entr, X_MinOptions[choice]);
            }
            //If the next node will be on the (x,0) Y_Min
            else if (newNode.Y == 0)
            {
                choice = UnityEngine.Random.Range(0, Y_MinOptions.Count);
                CreateRoom(newNode.X, newNode.Y, Entr, Y_MinOptions[choice]);
            }
            //If the next node will be on the (MAX,0) X_Max
            else if (newNode.X == Width-1)
            {
                choice = UnityEngine.Random.Range(0, X_MaxOptions.Count);
                CreateRoom(newNode.X, newNode.Y, Entr, X_MaxOptions[choice]);
            }
            //If the next node will be on the (x,MAX) Y_Max
            else if (newNode.Y == Height-1)
            {
                choice = UnityEngine.Random.Range(0, Y_MaxOptions.Count);
                CreateRoom(newNode.X, newNode.Y, Entr, Y_MaxOptions[choice]);
            }
            else
            {
                switch (newNode.EntranceSide)
                {
                    case OpeningSide.N:
                        CreateRoom(newNode.X, newNode.Y, OpeningSide.N, N_Options[choice]);
                        break;
                    case OpeningSide.E:
                        CreateRoom(newNode.X, newNode.Y, OpeningSide.E, E_Options[choice]);

                        break;
                    case OpeningSide.S:
                        CreateRoom(newNode.X, newNode.Y, OpeningSide.S, S_Options[choice]);

                        break;
                    case OpeningSide.W:
                        CreateRoom(newNode.X, newNode.Y, OpeningSide.W, W_Options[choice]);

                        break;
                    case OpeningSide.NONE:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    
    private bool CheckValidPoint(int x, int y)
    {
        return (x >= 0 && x < Width) && (y >= 0 && y < Height) && !_gridNodes[x, y].Visited;
    }
}
