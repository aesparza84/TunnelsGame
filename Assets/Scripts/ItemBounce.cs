using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBounce : MonoBehaviour
{
    [Header("Bounce Attributes")]
    [SerializeField] private float BounceHeight;
    [SerializeField] private float BounceSpeed;

    [Header("Item Mesh")]
    [SerializeField] private GameObject ItemMesh;

    //Moving Pos
    private Vector3 bouncePos;
    private Vector3 rotation;

    private void Start()
    {
        bouncePos = Vector3.zero;   
    }

    void Update()
    {
        bouncePos = ItemMesh.transform.localPosition;
        rotation = ItemMesh.transform.localRotation.eulerAngles;

        bouncePos.y = BounceHeight * Mathf.Abs(Mathf.Sin(BounceSpeed * Time.time));
        rotation.y = Mathf.Sin(Time.time);

        ItemMesh.transform.localPosition = bouncePos;
        ItemMesh.transform.localRotation = Quaternion.Euler(rotation);
    }
}
