using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceConnector : MonoBehaviour
{
    [SerializeField] private List<Transform> ConnectNodes;

    [Header("Map Prefabs")]
    [SerializeField] private GameObject one;
    [SerializeField] private GameObject two;
    [SerializeField] private GameObject three;
    [SerializeField] private GameObject four;
    [SerializeField] private GameObject five;
    private int choice;
    private void Start()
    {
        GeneratePieces();
    }

    private void GeneratePieces()
    {

        foreach (Transform t in ConnectNodes)
        {
            if (!MapCheck.CanAddPiece())
            {
                return;
            }

            choice = Random.Range(0, 6);

            if (choice == 0)
            {
                Instantiate(one, t.position, t.rotation);
            }
            else if (choice == 1)
            {
                Instantiate(two, t.position, t.rotation);
            }
            else if (choice == 2)
            {
                Instantiate(three, t.position, t.rotation);
            }
            else if (choice == 3)
            {
                Instantiate(four, t.position, t.rotation);
            }
            else if (choice == 4)
            {
                Instantiate(five, t.position, t.rotation);
            }

            MapCheck.AddedPiece();
        }
    }
}
