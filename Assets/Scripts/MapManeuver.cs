using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Point
{
    public int X;
    public int Y;
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }    

    public void SetPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
    public void SetPoint(Point p)
    {
        X = p.X;
        Y = p.Y;
    }
}
public class MapManeuver : MonoBehaviour
{
    [Header("Map Generator")]
    [SerializeField] private PathGenDFS _pathGenerator;

    //The generated map
    private GridNode[,] Map;

    [Header("Player reference")]
    [SerializeField] private PlayerController _player;
    public Point _playerPos { get; private set; }

    [Header("Movement Mask")]
    [SerializeField] private LayerMask HitLayermask;

    //Active state of MapManeuver
    private ActiveState _activeState;

    //Static Event
    public static event EventHandler PlayerReset;
    private void Start()
    {
        if (_pathGenerator == null)
            _pathGenerator = GetComponent<PathGenDFS>();

        _playerPos = new Point(0, 0);

        

        //_pathGenerator.OnNewMapGenerated += UpdateMap;

        LevelMessanger.MapReady += OnMapReady;
    }

    private void OnMapReady(object sender, EventArgs e)
    {
        //Get copy of Map
        Map = _pathGenerator.GridNodes;

        //Move Player to spawn node
        if (_player != null)
        {
            ResetPlayerToNode(_pathGenerator.SpawnNode);
        }

    }
    private void OnEnable()
    {
        if (_player != null)
            _player.OnMove += OnmoveRequest;
    }

    private void OnmoveRequest(object sender, System.Tuple<OpeningSide, Side> e)
    {
        CheckMoveToNode(e.Item1, e.Item2);
    }

    //Hard move player to node, RESET
    private void ResetPlayerToNode(GridNode gridNode)
    {
        _player.transform.position = gridNode._transform.position;
        _player.SetTargetPos(Map[gridNode.gridPoint.X, gridNode.gridPoint.Y]._transform.position);
        _playerPos.SetPoint(gridNode.gridPoint);
        PlayerReset?.Invoke(this, EventArgs.Empty);
    }

    /// 1. Determine 'Node' player wants to move to, based off direction
    /// 2. See if node is blocked by wall, Raycast?
    /// 3. See if player is trying to go ou of bounds.
    /// 

    private void CheckMoveToNode(OpeningSide facingDir, Side arm)
    {
        //Initialize Point
        Point potentialPoint = new Point(0,0);
        
        switch (facingDir)
        {
            case OpeningSide.N:
                potentialPoint = new Point(_playerPos.X, _playerPos.Y + 1);

                break;
            case OpeningSide.E:
                potentialPoint = new Point(_playerPos.X + 1, _playerPos.Y);

                break;
            case OpeningSide.S:
                potentialPoint = new Point(_playerPos.X, _playerPos.Y - 1);

                break;
            case OpeningSide.W:
                potentialPoint = new Point(_playerPos.X - 1, _playerPos.Y);

                break;
            case OpeningSide.NONE:
                break;
            default:
                break;
        }

        if (_pathGenerator.CheckForExistingRoom(potentialPoint.X, potentialPoint.Y))
        {
            Vector3 startPos = Map[_playerPos.X, _playerPos.Y]._transform.position;
            Vector3 finalPos = Map[potentialPoint.X, potentialPoint.Y]._transform.position;

            Vector3 Dir = finalPos - startPos;

            Debug.DrawRay(startPos, Dir, Color.red, 1.5f);

            if (!Physics.Raycast(startPos, Dir, Dir.magnitude, HitLayermask))
            {
                _player.SetTargetPos(finalPos);

                if (_player.Crawl(arm, potentialPoint))
                {
                    // Debug.Log($"From ({_playerPos.X}, {_playerPos.Y})\n To ({potentialPoint.X}, {potentialPoint.Y})");
                    
                    //Update player node position when move
                    _playerPos.SetPoint(potentialPoint.X, potentialPoint.Y);
                }
            }
        }

    }

    //Unsubscribe
    private void OnDisable()
    {
        _player.OnMove -= OnmoveRequest;
        LevelMessanger.MapReady -= OnMapReady;
    }

    
}
