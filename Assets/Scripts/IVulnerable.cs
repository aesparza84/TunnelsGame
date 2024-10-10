using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVulnerable 
{
    public event EventHandler OnVulRelease;
    public void Attack(Vector3 p, int time);
    public void Release();
    public void Retaliate(bool inFront);
    public Vector3 GetLookPoint();
}
