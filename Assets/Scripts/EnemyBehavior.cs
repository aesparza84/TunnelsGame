using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    //Pathfinder componenet
    private PathFinder _pathFinder;

    [SerializeField] private Vector2 newPos;

    private void Start()
    {
        _pathFinder = GetComponent<PathFinder>();   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _pathFinder.SetNewDestination(new Point((int)newPos.x, (int)newPos.y));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _pathFinder.TraversePath();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _pathFinder.StopTraverse();
        }
    }
}
