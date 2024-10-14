using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTransitioner : MonoBehaviour
{
    [Header("Camera Referenes")]
    [SerializeField] private CinemachineCamera PlayerMainCam;
    [SerializeField] private CinemachineCamera PlayerLowerCam;
    [SerializeField] private CinemachineCamera PlayerUpperCam;
    private CinemachineCamera _currentCamera;

    [Header("Entrance Force")]
    [SerializeField] private float EntranceImpulseForce = 0.2f;

    //Cinemachine Impulse Component
    //private CinemachineImpulseSource _cineImpulse;

    //For Screen Fader
    public static event EventHandler OnEntranceStarted;
    
    public event EventHandler OnEntranceTransitionFinished;
    public event EventHandler OnExitTransitionFinished;

    private void Awake()
    {
        //Activate Entrance Camera to blend into Play-cam
        LevelMessanger.PlayerReset += OnEntrance;

        //Activate Exit cam to pan donw and fade to black
        LevelMessanger.LevelFinished += OnExit;
    }


    private void OnEntrance(object sender, System.EventArgs e)
    {
        StartCoroutine(Entrance(PlayerUpperCam, PlayerLowerCam));
    }

    private void OnExit(object sender, System.EventArgs e)
    {
        StartCoroutine(Exit());
    }

    private void Start()
    {
        //_cineImpulse = GetComponent<CinemachineImpulseSource>();

        SwitchToCamera(PlayerMainCam);
    }
    private void SwitchToCamera(CinemachineCamera _cam)
    {
        if (_currentCamera != null) 
        { 
            _currentCamera.Priority = 0; 
        }

        _currentCamera = _cam;
        _currentCamera.Priority = 10;
    }

    private IEnumerator Entrance(CinemachineCamera from, CinemachineCamera to)
    {
        SwitchToCamera(from);

        yield return new WaitForSeconds(0.8f);

        SwitchToCamera(to);
        OnEntranceStarted?.Invoke(this, EventArgs.Empty); //Static

        yield return new WaitForSeconds(1);

        SwitchToCamera(PlayerMainCam);

        OnEntranceTransitionFinished?.Invoke(this, EventArgs.Empty);
        yield return null;
    }
    private IEnumerator Exit()
    {
        SwitchToCamera(PlayerLowerCam);
        yield return new WaitForSeconds(1);
        OnExitTransitionFinished?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

    private void OnDisable()
    {
        LevelMessanger.PlayerReset -= OnEntrance;
        LevelMessanger.LevelFinished -= OnExit;
    }
}
