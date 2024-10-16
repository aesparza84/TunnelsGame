using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    //Impulse Source
    private CinemachineImpulseSource _impulseSource;

    [Header("Shake SO")]
    [SerializeField] private CameraShakeData _shakeRetaliate;
    [SerializeField] private CameraShakeData _shakeEntrance;

    [Header("Player Camera")]
    [SerializeField] private CinemachineCamera _camera;
    private CinemachineImpulseListener _impulseListener;

    private void Start()
    {
        //Get Impulse source component
        _impulseSource = GetComponent<CinemachineImpulseSource>();

        //Get Impulse listener component
        _impulseListener = _camera.GetComponentInChildren<CinemachineImpulseListener>();

        PlayerController.OnRetalHit += OnRetaliate;
        CameraTransitioner.OnEntranceStarted += OnEntrance;
        LevelMessanger.GameLoopStopped += OnRetaliate;
    }

    private void OnEntrance(object sender, System.EventArgs e)
    {
        //Entrance Shake
        OneShotShake(_shakeEntrance);
    }

    private void OnRetaliate(object sender, System.EventArgs e)
    {
        //Small Shake
        OneShotShake(_shakeRetaliate);
    }

    private void OneShotShake(CameraShakeData s)
    {
        //Apply Shake values
        _impulseSource.DefaultVelocity = s._defaultVelocity;
        _impulseSource.ImpulseDefinition.ImpulseDuration = s._impulseDuration;

        //Apply Listner Reaction values
        _impulseListener.ReactionSettings.AmplitudeGain = s._listenerAmplitude;
        _impulseListener.ReactionSettings.FrequencyGain = s._listenerFrequency;
        _impulseListener.ReactionSettings.Duration = s._listenerDuration;
    
        //Send the impulse through
        _impulseSource.GenerateImpulse(s._impulseForce);
    }

    private void OnDisable()
    {
        PlayerController.OnRetalHit -= OnRetaliate;
        CameraTransitioner.OnEntranceStarted -= OnEntrance;
        LevelMessanger.GameLoopStopped -= OnRetaliate;
    }
}
