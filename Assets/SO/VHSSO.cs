using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="VHS Settings")]
public class VHSSO : ScriptableObject
{
    [Header("Weight")]
    [Range(0,1)]
    public float WeightVal;

    [Header("Bleed")]
    [Range(0, 10)]
    public float BleedVal;

    [Header("Rocking")]
    [Range(0, 0.1f)]
    public float RockingVal;

    [Header("Tape")]
    [Range(0, 2)]
    public float TapeVal;

    [Header("Noise")]
    [Range(0, 1)]
    public float NoiseVal;

    [Header("Flicker")]
    [Range(0, 2)]
    public float FlickeringVal;

    [Header("Glitching")]

    public Color GlitchColor;
}
