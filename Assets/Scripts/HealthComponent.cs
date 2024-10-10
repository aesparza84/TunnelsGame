using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IHealth
{
    //Amount of Health
    [Header("Max Health")]
    [SerializeField] private int MaxHealth;
    private int Health;

    public static event EventHandler OnPlayerDeath;
    private void Start()
    {
        Health = MaxHealth;
    }

    public void Heal(int n)
    {
        Health += n;

        if (Health >MaxHealth)
            Health = MaxHealth;
    }

    public void TakeDamage(int n)
    {
        Health -= n;

        Debug.Log($"Took {n} Damage");

        if (Health <= 0 )
        {
            Die();
        }
    }

    public void Die()
    {
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }
}
