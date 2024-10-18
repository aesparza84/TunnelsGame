using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Game Sounds")]
    [SerializeField] private Sound _RatGrowlLoop;
    [SerializeField] private Sound _RatAggroSound;
    [SerializeField] private Sound _RatDeAggroSound;
    [SerializeField] private Sound[] _RatAttackSounds;
    [SerializeField] private Sound _RockSound;
    [SerializeField] private Sound Deathsound;
    [SerializeField] private Sound RetaliateSound;
    [SerializeField] private GameObject Deathobj;

    //This audio suorce to send out
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        EnemyBehavior.OnAudioRequest += EnemyAudioRequest;
        EnemyBehavior.OnHeardPlayer += EnemyAggroAudio;
        EnemyBehavior.OnDeAggro += DeAggroAudio;
        LevelMessanger.GameLoopStopped += DeahAudio;
        PlayerController.OnRetalHit += RetaliateSOund;
    }

    private void RetaliateSOund(object sender, EventArgs e)
    {
        SetAndPlayAudio(_audioSource, RetaliateSound, true);
    }

    private void DeahAudio(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(Deathsound.soundClip, Deathobj.transform.position, 0.5f);
    }

    private void DeAggroAudio(object sender, Vector3 e)
    {
        AudioSource.PlayClipAtPoint(_RatDeAggroSound.soundClip, e, _RatDeAggroSound.Volume);

    }

    private void EnemyAggroAudio(object sender, Vector3 e)
    {
        AudioSource.PlayClipAtPoint(_RatAggroSound.soundClip, e, _RatAggroSound.Volume);
    }

    private void EnemyAudioRequest(object sender, Tuple<AudioSource, byte> e)
    {

        if (e.Item2 == 0)
        {
            SetAndPlayAudio(e.Item1, _RatGrowlLoop, false);

        }
        else if (e.Item2 == 1)
        {
            int choice = UnityEngine.Random.Range(0, _RatAttackSounds.Length);

            SetAndPlayAudio(e.Item1, _RatAttackSounds[choice], false);
        }
    }

    private void SetAndPlayAudio(AudioSource aud, Sound s, bool oneSHot)
    {
        aud.clip = s.soundClip;
        aud.priority = s.Priority;
        aud.volume = s.Volume;
        aud.pitch = s.Pitch;
        aud.panStereo = s.StereoPan;
        aud.spatialBlend = s.SpatialBlend;
        aud.minDistance = s.MinDistance;
        aud.maxDistance = s.MaxDistance;

        if (oneSHot)
        {
            aud.PlayOneShot(s.soundClip, s.Volume);
        }
        else
        {
            aud.Play();
        }
    }

    private void OnDisable()
    {
        EnemyBehavior.OnAudioRequest -= EnemyAudioRequest;
        EnemyBehavior.OnHeardPlayer -= EnemyAggroAudio;
        EnemyBehavior.OnDeAggro -= DeAggroAudio;
        LevelMessanger.GameLoopStopped -= DeahAudio;
        PlayerController.OnRetalHit -= RetaliateSOund;
    }
}
