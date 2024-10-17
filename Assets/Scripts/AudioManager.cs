using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Game Sounds")]
    [SerializeField] private Sound _RatGrowlLoop;
    [SerializeField] private Sound _RatAttackSound;


    private void Start()
    {
        EnemyBehavior.OnAudioRequest += EnemyAudioRequest;
    }    
    private void EnemyAudioRequest(object sender, Tuple<AudioSource, byte> e)
    {

        if (e.Item2 == 0)
        {
            SetAndPlayAudio(e.Item1, _RatGrowlLoop);

        }
        else if (e.Item2 == 1)
        {
            SetAndPlayAudio(e.Item1, _RatAttackSound);
        }
    }

    private void SetAndPlayAudio(AudioSource aud, Sound s)
    {
        aud.clip = s.soundClip;
        aud.priority = s.Priority;
        aud.volume = s.Volume;
        aud.pitch = s.Pitch;
        aud.panStereo = s.StereoPan;
        aud.spatialBlend = s.SpatialBlend;
        aud.minDistance = s.MinDistance;
        aud.maxDistance = s.MaxDistance;
        aud.Play();
    }

    private void OnDisable()
    {
        EnemyBehavior.OnAudioRequest -= EnemyAudioRequest;
    }
}
