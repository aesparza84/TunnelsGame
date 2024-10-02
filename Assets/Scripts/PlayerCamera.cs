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
    [SerializeField] private float LeanTime;
    private float currentLeanTime;
    private float TargetAngle;

    //Controller reference
    private PlayerController _playerController;

    private void Start()
    {
        if (_playerController == null)
            _playerController = GetComponent<PlayerController>();

        _playerController.OnRight += OnRightArm;
        _playerController.OnLeft += OnLeftArm;
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
        currentLeanTime = 0.0f;

        switch (leanSide)
        {
            case Side.LEFT:
                TargetAngle = LeftAngle;

                break;
            case Side.RIGHT:
                TargetAngle = RightAngle;

                break;
            case Side.NONE:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        //if (currentLeanTime < LeanTime)
        //{
        //    _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation,
        //                                                Quaternion.Euler(0, TargetAngle, 0),
        //                                                LeanTime * Time.deltaTime);
        //}
    }
    private void OnDisable()
    {
        _playerController.OnRight -= OnRightArm;
        _playerController.OnLeft -= OnLeftArm;
    }
}
