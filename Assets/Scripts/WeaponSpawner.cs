using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Possible Random Weapons")]
    [SerializeField] private int MaxAllowdWeapons;
    [SerializeField] private List<GameObject> WeaponPrefabs;

    //path genertaor ref
    private PathGenDFS _pathGen;
    private GridNode[,] Map;

    private void Awake()
    {
        _pathGen = GetComponent<PathGenDFS>();
        LevelMessanger.MapReady += Updatemap;
    }

    private void Updatemap(object sender, System.EventArgs e)
    {
        Map = _pathGen.GridNodes;
        SpawnRandomWeapons();
    }

    private void SpawnRandomWeapons()
    {
        int x = UnityEngine.Random.Range(0, MaxAllowdWeapons + 1);
        int y = 0;

        for (int i = 0; i < x; i++)
        {
            Point n = GetRandomPoint();

            y = UnityEngine.Random.Range(0, WeaponPrefabs.Count);

            Instantiate(WeaponPrefabs[y], Map[n.X, n.Y]._transform.position, Quaternion.identity);
        }
    }
    private Point GetRandomPoint()
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

            if (CheckValidPoint(r_point.X, r_point.Y))
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

    private bool CheckValidPoint(int x, int y)
    {
        return (x >= 0 && x < Map.GetLength(0)) && (y >= 0 && y < Map.GetLength(1));
    }
}
