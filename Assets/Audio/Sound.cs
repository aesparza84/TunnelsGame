using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip soundClip;

    [Range(0, 256)]
    public int Priority;

    [Range(0f, 1f)]
    public float Volume = 1;

    [Range(-3f, 3f)]
    public float Pitch = 1;

    [Range(-1f, 1f)]
    public float StereoPan;

    [Header("2D - 3D")]
    [Range(0f, 1f)]
    public float SpatialBlend;

    public float MinDistance = 1;
    public float MaxDistance = 5;
}
