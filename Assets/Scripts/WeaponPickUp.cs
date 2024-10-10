using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour, IPickUp
{
    [SerializeField] private Weapon _weapon;
    public void Pickup(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController p))
        {
            if (_weapon != null)
            {
                p.SetWeapon(_weapon);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Pickup(other);
    }
}
