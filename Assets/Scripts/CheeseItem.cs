using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheeseItem : MonoBehaviour, IPickUp    
{
    public static event EventHandler CheesePickedUp;
    public void Pickup(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController p))
        {
            CheesePickedUp?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Pickup(other);
    }
}
