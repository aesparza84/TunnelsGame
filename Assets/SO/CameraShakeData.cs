using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shake Data")]
public class CameraShakeData : ScriptableObject
{
    [Header("Source Data")]
    public float _impulseDuration;
    public float _impulseForce;
    public Vector3 _defaultVelocity;

    [Header("Reaction Data")]
    public float _listenerAmplitude;
    public float _listenerFrequency;
    public float _listenerDuration;
}
