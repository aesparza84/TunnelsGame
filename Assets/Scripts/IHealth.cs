using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
    public void Heal(int n);
    public void TakeDamage(int n);
    public void Die();
}
