using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FootStepAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Sound[] _CrawlSounds;

    [SerializeField] private FootStep _FootStep;
    private int choice;
    private void Start()
    {
        _FootStep.OnFootStep += PlayFootStep;

        ConfigureSource(_CrawlSounds[0]);
    }

    private void PlayFootStep(object sender, EventArgs e)
    {
        choice = UnityEngine.Random.Range(0, _CrawlSounds.Length);
        _audioSource.clip = _CrawlSounds[choice].soundClip;
        _audioSource.Play();
    }
    private void ConfigureSource(Sound s)
    {
        _audioSource.clip = s.soundClip;
        _audioSource.priority = s.Priority;
        _audioSource.volume = s.Volume;
        _audioSource.pitch = s.Pitch;
        _audioSource.panStereo = s.StereoPan;
        _audioSource.spatialBlend = s.SpatialBlend;
        _audioSource.Play();
    }

    private void OnDisable()
    {
        _FootStep.OnFootStep -= PlayFootStep;
    }
}
