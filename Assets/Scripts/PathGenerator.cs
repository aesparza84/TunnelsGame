using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [Header("Map Info")]
    [SerializeField] private int Width = 10;
    [SerializeField] private int Height = 10;
    [SerializeField] private float SpaceBetween = 1; //in meters

    //[Header("Map Prefabs")]
    //[SerializeField] private GameObject Straight;
    //[SerializeField] private GameObject ThreeDoor;
    //[SerializeField] private GameObject FourDoor;
    //[SerializeField] private GameObject EndPlug;

    [SerializeField] private GameObject sphere;

    //For choosing piece
    private int choice;

    private Transform[,] Nodes;

    private void Start()
    {
        Nodes = new Transform[Width, Height];

        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                GameObject p = new GameObject();
                GameObject inst = Instantiate(p, transform.position, transform.rotation);
                inst.transform.position += new Vector3(j * SpaceBetween, 0, i * SpaceBetween);

                GameObject o = Instantiate(sphere, inst.transform.position, transform.rotation);
            }
        }
    }
}
