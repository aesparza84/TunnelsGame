using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowFPSCamera : MonoBehaviour
{
    [Header("Player Cam")]
    [SerializeField] private Transform _trackingTarget;

    [Range(0f, 1f)]
    [SerializeField] private float MaxFrameWaitTime = 0.7f;
    private float _frameWaitTime;
    private float _currentWait;

    [Header("Independent shake values")]
    [SerializeField] private float FPS_ShakeAmplitude;
    [SerializeField] private float FPS_ShakeSpeed;


    [Header("Low FPS Camera")]
    [SerializeField] private Camera _lowFPSCAmera;

    private void Update()
    {
        _lowFPSCAmera.transform.rotation = _trackingTarget.rotation;

        if (_currentWait < _frameWaitTime)
        {
            _currentWait += Time.deltaTime;

            Vector3 v = _lowFPSCAmera.transform.localPosition;
            v.x = (FPS_ShakeAmplitude * Mathf.Sin(FPS_ShakeSpeed * Time.time));
            v.y = (FPS_ShakeAmplitude * Mathf.Cos(FPS_ShakeSpeed * Time.time));
            _lowFPSCAmera.transform.localPosition = v;
        }
        else
        {
            _lowFPSCAmera.Render();
            StaggerWait();
        }
    }

    private void StaggerWait()
    {
        _frameWaitTime = UnityEngine.Random.Range(0, MaxFrameWaitTime);
        _currentWait = 0.0f;
    }
}
