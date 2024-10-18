using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IHealth
{
    //Amount of Health
    [Header("Max Health")]
    [SerializeField] private int _MaxHealth;
    public int Health { get; private set; }
    public int MaxHealth { get { return _MaxHealth; } }

    public event EventHandler OnPlayerDeath;
    public static event EventHandler OnHeal;

    private bool Dead;
    private void Start()
    {
        Dead = false;
        Health = _MaxHealth;
    }

    public void Heal(int n)
    {
        OnHeal?.Invoke(this, EventArgs.Empty);
        Health += n;

        if (Health >_MaxHealth)
            Health = _MaxHealth;
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
        if (!Dead)
        {
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            Dead = true;
        }
    }
}
