using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IFootStep
{
    public event EventHandler OnFootStep;

    public void TriggerFootStep();
}
public class FootStep : MonoBehaviour, IFootStep
{
    public event EventHandler OnFootStep;

    public void TriggerFootStep()
    {
        OnFootStep?.Invoke(this, EventArgs.Empty);
    }
}
