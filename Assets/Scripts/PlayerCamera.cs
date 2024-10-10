using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform _cameraTransform;

    [Header("Camera Lean Angles")]
    [SerializeField] private float LeftAngle;
    [SerializeField] private float RightAngle;
    [SerializeField] private float LeanSpeed;
    [SerializeField] private float BackLeanSpeed;
    private float Z_TargetAngle;

    [Header("Camera Noise Values")]
    [SerializeField] private float AttackAmplitude;
    [SerializeField] private float AttackFrequency;
    [SerializeField] private float CrawlAmplitude;
    [SerializeField] private float CrawlFrequency;

    [Header("Camera Attack Noise Values")]
    [SerializeField] private float RetaliateForce;
    private float RetalTimer;

    //Cinemachine Components
    private CinemachineBasicMultiChannelPerlin _cameraNoise;
    private CinemachineImpulseListener _impulseListener;
    private CinemachineImpulseSource _impulseSource;

    //Camera rotations
    private Vector3 TargetForwardEuler;
    private Vector3 TargetBackEuler;
    private Vector3 BackLeftEuler;
    private Vector3 BackRightEuler;
    private Quaternion OverrideRotation;
    private float Forward_Z;

    private bool Lean;
    private bool BackLean;
    private bool OverrideLook;
    private Side PrevArm;

    //Controller reference
    private PlayerController _playerController;

    //CameraSpeicifc control
    private PlayerControls _camControls;
    private InputAction CameraTurnaround_Left;
    private InputAction CameraTurnaround_Right;

    private void Start()
    {
        if (_playerController == null)
            _playerController = GetComponent<PlayerController>();


        _cameraNoise = _camera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        _impulseListener = _camera.GetComponentInChildren<CinemachineImpulseListener>();
        _impulseSource = transform.GetComponentInChildren<CinemachineImpulseSource>();

        _playerController.OnRight += OnRightArm;
        _playerController.OnLeft += OnLeftArm;
        _playerController.OnAttacked += OnPlayerAttacked;
        _playerController.OnReleased += OnPlayerReleased;
        _playerController.OnRetaliate += OnPlayerRetaliate;


        //Set initial lean side
        LeanCamera(Side.RIGHT);

        BackRightEuler = new Vector3(0, 147.73f, 352.0f);
        BackLeftEuler = new Vector3(4.71f, 215.96f, 353.53f);

        Forward_Z = _cameraTransform.localPosition.z;
        CameraNoise_Normal();

    }

    private void OnEnable()
    {
        if (_camControls == null)
        {
            _camControls = new PlayerControls();
        }

        CameraTurnaround_Left = _camControls.Crawl.CamBehindLeft;
        CameraTurnaround_Left.Enable();
        CameraTurnaround_Left.performed += OnCameraBackLeft;
        CameraTurnaround_Left.canceled += OnCameraBackLeft_Stopped;

        CameraTurnaround_Right = _camControls.Crawl.CamBehindRight;
        CameraTurnaround_Right.Enable();
        CameraTurnaround_Right.performed += OnCameraBackRight;
        CameraTurnaround_Right.canceled += OnCameraBackRight_Stopped;
    }

    #region Back Leaning controls
    private void OnCameraBackLeft(InputAction.CallbackContext obj)
    {
        BackLean = true;
        TargetBackEuler = BackLeftEuler;
    }
    private void OnCameraBackLeft_Stopped(InputAction.CallbackContext obj)
    {
        BackLean = false;
    }

    private void OnCameraBackRight(InputAction.CallbackContext obj)
    {
        BackLean = true;
        TargetBackEuler = BackRightEuler;
    }
    private void OnCameraBackRight_Stopped(InputAction.CallbackContext obj)
    {
        BackLean = false;
    }
    #endregion

    #region Attack Events
    private void OnPlayerReleased(object sender, System.EventArgs e)
    {
        CameraNoise_Normal();

        OverrideLook = false;
        LeanCamera(PrevArm);
    }

    private void OnPlayerAttacked(object sender, Vector3 e)
    {
        OverrideLook = true;

        CameraNoise_Attack();

        Vector3 dir = (e - _cameraTransform.position).normalized;
        

        OverrideRotation = Quaternion.LookRotation(dir, Vector3.up);
        Vector3 v = OverrideRotation.eulerAngles;
        v.z = TargetForwardEuler.z;
        OverrideRotation = Quaternion.Euler(v);
    }
    private void OnPlayerRetaliate(object sender, System.EventArgs e)
    {
        //_impulseListener
        _impulseSource.GenerateImpulseWithForce(RetaliateForce);
    }
    #endregion

    private void CameraNoise_Attack()
    {
        _cameraNoise.AmplitudeGain = AttackAmplitude;
        _cameraNoise.FrequencyGain = AttackFrequency;
    }
    private void CameraNoise_Normal()
    {
        _cameraNoise.AmplitudeGain = CrawlAmplitude;
        _cameraNoise.FrequencyGain = CrawlFrequency;
    }
    private void Impulse_Retaliate()
    {
        _impulseListener.ReactionSettings.AmplitudeGain = _cameraNoise.AmplitudeGain;
        _impulseListener.ReactionSettings.FrequencyGain = _cameraNoise.FrequencyGain;
        _impulseListener.ReactionSettings.Duration = RetalTimer;
    }
    private void Impulse_Reset()
    {
        _impulseListener.ReactionSettings.AmplitudeGain = 0;
        _impulseListener.ReactionSettings.FrequencyGain = 0;
        _impulseListener.ReactionSettings.Duration = 0;
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

                PrevArm = leanSide;
                break;
            case Side.RIGHT:
                Z_TargetAngle = RightAngle;

                PrevArm = leanSide;
                break;
            case Side.NONE:
                break;
            default:
                break;
        }

        TargetForwardEuler.z = Z_TargetAngle;

        Lean = true;
    }

    private void Update()
    {
        if (OverrideLook)
        {
            HandleAttackCamera();
        }
        else
        {
            HandleLeaning();
        }
    }

    private void HandleLeaning()
    {        
        if (BackLean)
        {
            SetBackLean(TargetBackEuler);
        }
        else if (Lean)
        {
            //Reset camera to forward Pos if coming from beacklean
            if (_cameraTransform.localPosition.z != Forward_Z)
            {
                _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, 
                                                              new Vector3(_cameraTransform.localPosition.x,
                                                              _cameraTransform.localPosition.y,
                                                              Forward_Z),
                                                              0.8f);
            }

            _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation, Quaternion.Euler(TargetForwardEuler), LeanSpeed);

            if (_cameraTransform.localRotation == Quaternion.Euler(TargetForwardEuler))
            {
                _cameraTransform.localRotation = Quaternion.RotateTowards(_cameraTransform.localRotation, Quaternion.Euler(TargetForwardEuler), 100);
                Lean = false;
            }
        }
    }
    private void HandleAttackCamera()
    {
        _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, OverrideRotation, LeanSpeed);
    }

    private void SetBackLean(Vector3 euler)
    {
        _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation, Quaternion.Euler(euler), BackLeanSpeed);

        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, new Vector3(_cameraTransform.localPosition.x,
                                                                                                  _cameraTransform.localPosition.y,
                                                                                                  0),
                                                                                                  0.8f);

        if (_cameraTransform.localRotation == Quaternion.Euler(euler))
        {
            _cameraTransform.localRotation = Quaternion.RotateTowards(_cameraTransform.localRotation, Quaternion.Euler(euler), 100);
        }

        Lean = true;
    }

    private void OnDisable()
    {
        CameraTurnaround_Left.Disable();
        CameraTurnaround_Left.started -= OnCameraBackLeft;

        CameraTurnaround_Right.Disable();
        CameraTurnaround_Right.started -= OnCameraBackRight;

        _playerController.OnRight -= OnRightArm;
        _playerController.OnLeft -= OnLeftArm;
        _playerController.OnAttacked -= OnPlayerAttacked;
        _playerController.OnReleased -= OnPlayerReleased;
        _playerController.OnRetaliate -= OnPlayerRetaliate;
    }
}
