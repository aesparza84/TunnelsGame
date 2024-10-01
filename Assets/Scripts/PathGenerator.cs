using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public OpeningSide EntranceSide;

    public Roominfo _roominfo;
    public Node(int x, int y, OpeningSide OpeningNeeded, Roominfo roominfo)
    {
        X = x;
        Y = y;
        EntranceSide = OpeningNeeded;

        _roominfo = roominfo;
    }
    public Node(int x, int y, OpeningSide OpeningNeeded)
    {
        X = x;
        Y = y;
        EntranceSide = OpeningNeeded;
    }
    public Node(int x, int y, Roominfo roominfo)
    {
        X = x;
        Y = y;
        _roominfo = roominfo;
    }
}

public struct GridPoint
{
    public int X;
    public int Y;
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

    private List<GridPoint> VisitedNodes;
    private void Start()
    {
        _transforms = new Transform[Width, Height];
        _nodes = new List<Node>();
        _nodeStack = new Stack<Node>();
        VisitedNodes = new List<GridPoint>();

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

        SpawnRooms(new Node(startPosX, startPosY, n.GetComponent<Roominfo>()));
    }

    private bool CheckValidPoint(int x, int y)
    {
        return (x >= 0 && x <= Width) && (y >=0 && y <= Height);
    }
    private void SpawnRooms(Node currentNode)
    {
        if (CurrentI > RecursiveCap)
            return;

        CurrentI++;

        //First we get the current position in grid
        int currentX = currentNode.X;
        int currentY = currentNode.Y;

        GridPoint p;
        p.X = currentX;
        p.Y = currentY;

        if (!VisitedNodes.Contains(p))
        {
            VisitedNodes.Add(p);
        }
        else
        {
            return;
        }
        

        //Then we see what kind of exits we have (N, S, W exit...) at this node
        Roominfo room = currentNode._roominfo;

        //If node has an exit at 'X-side' check if we can move further.
        //If so, then add that node to the List<Node> of nodes to affect
        if (room.ExitSides.Contains(OpeningSide.N))
        {
            if (CheckValidPoint(currentX, currentY + 1))
                _nodes.Add(new Node(currentX, currentY + 1, OpeningSide.S));
        }
        if (room.ExitSides.Contains(OpeningSide.E))
        {
            if (CheckValidPoint(currentX + 1, currentY))
                _nodes.Add(new Node(currentX + 1, currentY, OpeningSide.W));

        }
        if (room.ExitSides.Contains(OpeningSide.S))
        {
            if (CheckValidPoint(currentX, currentY - 1))
                _nodes.Add(new Node(currentX, currentY - 1, OpeningSide.N));

        }
        if (room.ExitSides.Contains(OpeningSide.W))
        {
            if (CheckValidPoint(currentX - 1, currentY))
                _nodes.Add(new Node(currentX - 1, currentY, OpeningSide.E));
        }


        //For each node we added, add a piece that has entrance on side 'X'
        int choice;
        foreach (Node newNode in _nodes)
        {           
            choice = UnityEngine.Random.Range(0, 5);
            GameObject nextRoom = null;

            switch (newNode.EntranceSide)
            {
                case OpeningSide.N:
                    nextRoom = Instantiate(N_Openings[choice], _transforms[newNode.X, newNode.Y].position, N_Openings[choice].transform.rotation);

                    break;
                case OpeningSide.E:
                    nextRoom = Instantiate(E_Openings[choice], _transforms[newNode.X, newNode.Y].position, E_Openings[choice].transform.rotation);

                    break;
                case OpeningSide.S:
                    nextRoom = Instantiate(S_Openings[choice], _transforms[newNode.X, newNode.Y].position, S_Openings[choice].transform.rotation);

                    break;
                case OpeningSide.W:
                    nextRoom = Instantiate(W_Openings[choice], _transforms[newNode.X, newNode.Y].position, W_Openings[choice].transform.rotation);

                    break;
                case OpeningSide.NONE:
                    break;
                default:
                    break;
            }

            _nodeStack.Push(new Node(newNode.X, newNode.Y, nextRoom.GetComponent<Roominfo>()));
        }

        _nodes.Clear();

        if (_nodeStack.Count > 0)
        {
            SpawnRooms(_nodeStack.Pop());
        }

    }
}
