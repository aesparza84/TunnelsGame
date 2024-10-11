using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVulnerable 
{
    public event EventHandler OnVulRelease;
    public event EventHandler<Side> OnVulRetaliate;
    public void Attack(Vector3 p, int time);
    public void Release();
    public void Retaliate(bool inFront, ref Weapon w);
    public Vector3 GetLookPoint();
}
