using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVulnerable 
{
    public event EventHandler<Vector3> OnVulRelease;
    public void Attack(Vector3 p);
    public void Release();
    public Vector3 GetLookPoint();
}
