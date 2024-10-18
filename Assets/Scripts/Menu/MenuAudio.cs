using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    //singleton audio events
    private static MenuAudio _instance;

    //Menu sfx audioSource
    private AudioSource _audioSource;

    [SerializeField] private Sound SelectSound;
    [SerializeField] private Sound ConfirmSound;


    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);

        MenuAudioTriggers.OnNewOption += NewOption;
        MenuAudioTriggers.OnConfirm += Confirm;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.spatialBlend = SelectSound.SpatialBlend;
        _audioSource.priority = 1;
    }

    private void Confirm(object sender, System.EventArgs e)
    {
        PlayConfirmSound();
    }

    private void NewOption(object sender, System.EventArgs e)
    {
        PlaySelectSound();
    }

    private void PlaySelectSound()
    {
        _audioSource.clip = SelectSound.soundClip;
        _audioSource.volume = SelectSound.Volume;
        _audioSource.pitch = SelectSound.Pitch;
        _audioSource.Play();
    }

    private void PlayConfirmSound()
    {
        _audioSource.clip = ConfirmSound.soundClip;
        _audioSource.volume = ConfirmSound.Volume;
        _audioSource.pitch = ConfirmSound.Pitch;
        _audioSource.Play();
    }

    private void OnDisable()
    {
        MenuAudioTriggers.OnNewOption -= NewOption;
        MenuAudioTriggers.OnConfirm -= Confirm;
    }
}
