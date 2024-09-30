using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] private GameObject FourWay;
    [SerializeField] private GameObject ThreeWay;
    [SerializeField] private GameObject Path;
    [SerializeField] private GameObject End;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Instantiate(Path, transform.position, transform.rotation);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Instantiate(FourWay, transform.position, transform.rotation);

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Instantiate(ThreeWay, transform.position, transform.rotation);

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Instantiate(End, transform.position, transform.rotation);

        }
    }
}
