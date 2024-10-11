using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraFollow : MonoBehaviour
{
    [Header("Player Obejct")]
    [SerializeField] private GameObject PlayerObj;

    private Vector3 UpdatePos;

    private void LateUpdate()
    {
        UpdatePos = PlayerObj.transform.position;
        UpdatePos.y = transform.position.y;
        transform.position = UpdatePos;
    }
}
