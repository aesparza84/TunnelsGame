using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform _cameraTransform;

    [Header("Camera Lean Angles")]
    [SerializeField] private float LeftAngle;
    [SerializeField] private float RightAngle;
    [SerializeField] private float LeanSpeed;
    private float Z_TargetAngle;

    private Vector3 TargetEuler;

    private bool Lean;

    //Controller reference
    private PlayerController _playerController;

    private void Start()
    {
        if (_playerController == null)
            _playerController = GetComponent<PlayerController>();

        _playerController.OnRight += OnRightArm;
        _playerController.OnLeft += OnLeftArm;

        //Set initial lean side
        LeanCamera(_playerController.CurrentArm);   
    }

    private void OnLeftArm(object sender, System.EventArgs e)
    {
        LeanCamera(Side.LEFT);
    }
    private void OnRightArm(object sender, System.EventArgs e)
    {
        LeanCamera(Side.RIGHT);
    }

    private void LeanCamera(Side leanSide)
    {
        //If currently leaning, dont lean again
        if (Lean)
            return;

        //Set Target lean angle
        switch (leanSide)
        {
            case Side.LEFT:
                Z_TargetAngle = LeftAngle;

                break;
            case Side.RIGHT:
                Z_TargetAngle = RightAngle;

                break;
            case Side.NONE:
                break;
            default:
                break;
        }

        TargetEuler.z = Z_TargetAngle;

        //_cameraTransform.localRotation = Quaternion.Euler(0, 0, TargetAngle);

        Lean = true;
    }

    private void Update()
    {
        HandleLeaning();
    }

    private void HandleLeaning()
    {
        if (Lean)
        {
            _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation, Quaternion.Euler(TargetEuler), LeanSpeed);

            if (_cameraTransform.localRotation == Quaternion.Euler(TargetEuler))
            {
                _cameraTransform.localRotation = Quaternion.RotateTowards(_cameraTransform.localRotation, Quaternion.Euler(TargetEuler), 100);
                Lean = false;
            }
        }
    }

    private void OnDisable()
    {
        _playerController.OnRight -= OnRightArm;
        _playerController.OnLeft -= OnLeftArm;
    }
}
