using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Possible Random Weapons")]
    [SerializeField] private int MaxAllowdWeapons;
    [SerializeField] private List<GameObject> WeaponPrefabs;

    [Header("Cheese Item")]
    [SerializeField] private GameObject CheeseObj;

    //path genertaor ref
    private PathGenDFS _pathGen;
    private GridNode[,] Map;

    //trakced spawned Items
    private List<GameObject> trackeditems;

    private void Awake()
    {
        trackeditems = new List<GameObject>();

        _pathGen = GetComponent<PathGenDFS>();
        LevelMessanger.MapReady += Updatemap;
    }

    private void Start()
    {
        LevelMessanger.DifficultyIncrease += DifficultSpawnCap;
    }

    private void DifficultSpawnCap(object sender, System.EventArgs e)
    {
        MaxAllowdWeapons = 2;
    }

    private void OnDisable()
    {
        LevelMessanger.MapReady -= Updatemap;
        LevelMessanger.DifficultyIncrease -= DifficultSpawnCap;
    }

    private void Updatemap(object sender, System.EventArgs e)
    {
        if (trackeditems.Count > 0)
        {
            ClearItems();
        }

        Debug.Log("Updaet map clalled");
        Map = _pathGen.GridNodes;
        SpawnRandomWeapons();
        SpawnCheese();
    }

    private void SpawnRandomWeapons()
    {
        int x = UnityEngine.Random.Range(0, MaxAllowdWeapons + 1);
        int y = 0;

        for (int i = 0; i < x; i++)
        {
            Point n = GetRandomPoint();

            y = UnityEngine.Random.Range(0, WeaponPrefabs.Count);

            GameObject p = Instantiate(WeaponPrefabs[y], Map[n.X, n.Y]._transform.position, Quaternion.identity);
            trackeditems.Add(p);
        }
    }

    private void SpawnCheese()
    {
        Point p = GetRandomPoint();
        GameObject c = Instantiate(CheeseObj, Map[p.X, p.Y]._transform.position, Quaternion.identity);
        trackeditems.Add(c);
    }

    private void ClearItems()
    {
        foreach (GameObject item in trackeditems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        trackeditems.Clear();
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
