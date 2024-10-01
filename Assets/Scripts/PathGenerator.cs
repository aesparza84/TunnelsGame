using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpeningSide {N, E, S, W, NONE }

/// <summary>
/// X - Position X
/// Y - Position Y
/// Entrance - Next opening needed 
/// </summary>
public class Node
{
    public int X;

    public int Y;

    public OpeningSide NextEntranceSide;

    public Roominfo _roominfo;
    public Node(int x, int y, OpeningSide OpeningNeeded, Roominfo roominfo)
    {
        X = x;
        Y = y;
        NextEntranceSide = OpeningNeeded;

        _roominfo = roominfo;
    }
    public Node(int x, int y, OpeningSide OpeningNeeded)
    {
        X = x;
        Y = y;
        NextEntranceSide = OpeningNeeded;
    }
    public Node(int x, int y, Roominfo roominfo)
    {
        X = x;
        Y = y;
        _roominfo = roominfo;
    }
}
public class PathGenerator : MonoBehaviour
{
    [Header("Map Info")]
    [SerializeField] private int Width = 10;
    [SerializeField] private int Height = 10;
    [SerializeField] private float SpaceBetween = 1; //in meters

    [Header("Map Prefabs")]
    [SerializeField] private List<GameObject> N_Openings;
    [SerializeField] private List<GameObject> E_Openings;
    [SerializeField] private List<GameObject> S_Openings;
    [SerializeField] private List<GameObject> W_Openings;

    [Header("Start Position")]
    [SerializeField] private int startPosX;
    [SerializeField] private int startPosY;

    [SerializeField] private GameObject sphere;

    private int RecursiveCap;
    private int CurrentI;

    private Transform[,] _transforms;
    private List<Node> _nodes;
    private Stack<Node> _nodeStack;

    private void Start()
    {
        _transforms = new Transform[Width, Height];
        _nodes = new List<Node>();
        _nodeStack = new Stack<Node>();
        

        RecursiveCap = Width * Height;
        CurrentI = 0;

        GenerateGrid();
        GenerateMapPieces();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                GameObject p = new GameObject();
                GameObject inst = Instantiate(p, transform.position, transform.rotation);
                inst.transform.position += new Vector3(j * SpaceBetween, 0, i * SpaceBetween);

                _transforms[j, i] = inst.transform;

                GameObject o = Instantiate(sphere, inst.transform.position, transform.rotation);
            }
        }
    }

    private void GenerateMapPieces()
    {
        GameObject n = S_Openings[4];
        Instantiate(n, _transforms[startPosX, startPosY].position, Quaternion.identity);

        SpawnRooms(new Node(startPosX, startPosY, OpeningSide.S, n.GetComponent<Roominfo>()));
    }

    private bool CheckValidPoint(int x, int y)
    {
        return (x >= 0 && x <= Width) && (y >=0 && y <= Height);
    }
    private void SpawnRooms(Node _node)
    {
        if (CurrentI > RecursiveCap)
            return;

        if (_nodeStack.Count > 0)
        {
            int stackCount = _nodeStack.Count;
            for (int i = 0; i < stackCount; i++)
            {
                _nodes.Add(_nodeStack.Peek());
                _nodeStack.Pop();
            }

            _nodeStack.Clear();
        }

        CurrentI++;

        int currentX = _node.X;
        int currentY = _node.Y;

        //Generate Initial Room
        //GameObject n = N_Openings[4];
        //Instantiate(n, _transforms[currentX, currentY].position, n.transform.rotation);


        Roominfo room = _node._roominfo;
        //Exit side(s): N
        //Exit side(s): N, W, S

        if (room.ExitSides.Contains(OpeningSide.N))
        {
            if (CheckValidPoint(currentX, currentY - 1))
                _nodes.Add(new Node(currentX, currentY - 1, OpeningSide.S));
        }
        if (room.ExitSides.Contains(OpeningSide.E))
        {
            if (CheckValidPoint(currentX + 1, currentY))
                _nodes.Add(new Node(currentX + 1, currentY, OpeningSide.W));

        }
        if (room.ExitSides.Contains(OpeningSide.S))
        {
            if (CheckValidPoint(currentX, currentY + 1))
                _nodes.Add(new Node(currentX, currentY + 1, OpeningSide.N));

        }
        if (room.ExitSides.Contains(OpeningSide.W))
        {
            if (CheckValidPoint(currentX - 1, currentY))
                _nodes.Add(new Node(currentX - 1, currentY, OpeningSide.E));
        }

        int choice;
        foreach (Node node in _nodes)
        {
            choice = Random.Range(0, 6);
            GameObject nextRoom = null;

            switch (node.NextEntranceSide)
            {
                case OpeningSide.N:
                    nextRoom = Instantiate(N_Openings[choice], new Vector3(node.X,0,node.Y), Quaternion.identity);

                    break;
                case OpeningSide.E:
                    nextRoom = Instantiate(E_Openings[choice], new Vector3(node.X,0,node.Y), Quaternion.identity);

                    break;
                case OpeningSide.S:
                    nextRoom = Instantiate(S_Openings[choice], new Vector3(node.X,0,node.Y), Quaternion.identity);

                    break;
                case OpeningSide.W:
                    nextRoom = Instantiate(W_Openings[choice], new Vector3(node.X,0,node.Y), Quaternion.identity);

                    break;
                case OpeningSide.NONE:
                    break;
                default:
                    break;
            }

            _nodeStack.Push(new Node(node.X, node.Y, nextRoom.GetComponent<Roominfo>()));
        }

        _nodes.Clear();
    }
}
