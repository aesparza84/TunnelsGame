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

    //Cinemachine Impulse Component
    private CinemachineImpulseSource _cineImpulse;

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
        StartCoroutine(Entrance(PlayerUpperCam, PlayerMainCam));
    }

    private void OnExit(object sender, System.EventArgs e)
    {
        StartCoroutine(Exit());

    }

    private void Start()
    {
        _cineImpulse = GetComponent<CinemachineImpulseSource>();

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
        yield return new WaitForSeconds(1);
        _cineImpulse.GenerateImpulse(1);
        SwitchToCamera(to);
        OnEntranceTransitionFinished?.Invoke(this, EventArgs.Empty);
        yield return null;
    }
    private IEnumerator Exit()
    {
        SwitchToCamera(PlayerLowerCam);
        yield return new WaitForSeconds(2);
        OnExitTransitionFinished?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

    private void OnDisable()
    {
        LevelMessanger.PlayerReset -= OnEntrance;
        LevelMessanger.LevelFinished -= OnExit;
    }
}
