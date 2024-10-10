using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPickUp
{
    public void Pickup(Collider other);
}
public class HealthPickup : MonoBehaviour, IPickUp
{
    [SerializeField] private int HealthRestored;
    public void Pickup(Collider other)
    {
        if (other.TryGetComponent<IHealth>(out IHealth h))
        {
            h.Heal(HealthRestored);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Pickup(other);
    }
}
