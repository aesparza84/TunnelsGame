using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 10);
    }
}
