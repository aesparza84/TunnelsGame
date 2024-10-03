using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManeuver : MonoBehaviour
{
    //Generator reference
    private PathGenDFS _pathGenerator;

    //The generated map
    private GridNode[,] Map;

    //Player Reference
    private PlayerController _player;

    private void Start()
    {
        if (_pathGenerator == null)
            _pathGenerator = GetComponent<PathGenDFS>();

        //Get copy of Map
        Map = _pathGenerator.GridNodes;
    }

}
