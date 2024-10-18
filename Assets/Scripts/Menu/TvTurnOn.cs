using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TvTurnOn : MonoBehaviour
{
    [SerializeField] private Material TvOnMaterial;
    [SerializeField] private Sound TvOnSound;

    [Header("Light")]
    [SerializeField] private Light _spotLight;
    [SerializeField] private float StartRange;
    [SerializeField] private float OnRange;
    private AudioSource _audioSource;

    private void Start()
    {
        _spotLight.range = StartRange;
        _audioSource = GetComponent<AudioSource>();
    }
    public void SwitchOn()
    {
        GetComponent<MeshRenderer>().material = TvOnMaterial;

        _spotLight.range = OnRange;

        _audioSource.clip = TvOnSound.soundClip;
        _audioSource.spatialBlend = TvOnSound.SpatialBlend;
        _audioSource.pitch = TvOnSound.Pitch;
        _audioSource.volume = TvOnSound.Volume;
        _audioSource.Play();
    }
}
