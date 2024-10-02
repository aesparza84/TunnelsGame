using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Side { LEFT, RIGHT, NONE };
public class PlayerController : MonoBehaviour
{
    //Mapped Inputs
    private PlayerControls _mappedInputs;

    //Input references
    private InputAction LeftArm;
    private InputAction RightArm;
    private InputAction TurnLeft;
    private InputAction TurnRight;

    private Side currentArm;

    [SerializeField] private float TurnTime;
    private float currentTurnTime;
    private Vector3 TargetRotation;

    //Arm Crawl Events
    public event EventHandler OnLeft;
    public event EventHandler OnRight;

    private void Start()
    {
        currentArm = Side.LEFT;
        currentTurnTime = 0.0f;
        TargetRotation = Vector3.zero;
    }
    private void OnEnable()
    {
        if (_mappedInputs == null)
            _mappedInputs = new PlayerControls();

        LeftArm = _mappedInputs.Crawl.LeftArm;
        LeftArm.Enable();
        LeftArm.started += OnLeftCrawl;

        RightArm = _mappedInputs.Crawl.RightArm;    
        RightArm.Enable();
        RightArm.started += OnRightCrawl;

        TurnLeft = _mappedInputs.Crawl.CamLeft;
        TurnLeft.Enable();
        TurnLeft.started += OnTurnLeft;

        TurnRight = _mappedInputs.Crawl.CamRight;
        TurnRight.Enable();
        TurnRight.started += OnTurnRight;

    }

    private void OnRightCrawl(InputAction.CallbackContext obj)
    {
        Crawl(Side.RIGHT);
        OnRight?.Invoke(this, EventArgs.Empty);
    }
    private void OnLeftCrawl(InputAction.CallbackContext obj)
    {
        Crawl(Side.LEFT);
        OnLeft?.Invoke(this, EventArgs.Empty);
    }
    private void OnTurnLeft(InputAction.CallbackContext obj)
    {
        Turn(Side.LEFT);
    }
    private void OnTurnRight(InputAction.CallbackContext obj)
    {
        Turn(Side.RIGHT);
    }

    private void Crawl(Side side)
    {
        switch (currentArm)
        {
            case Side.LEFT:
                if (side == Side.LEFT)
                {
                    transform.localPosition += transform.forward;

                    currentArm = Side.RIGHT;
                }

                break;
            case Side.RIGHT:
                if (side == Side.RIGHT)
                {
                    transform.localPosition += transform.forward;

                    currentArm = Side.LEFT;  
                }

                break;
            case Side.NONE:
                break;
            default:
                break;
        }
    }

    private void Turn(Side newTurn)
    {
        //Reset Turn timer
        currentTurnTime = 0.0f;
        
        //Set new target rotation
        switch (newTurn)
        {
            case Side.LEFT:
                TargetRotation = transform.localEulerAngles + new Vector3(0, -90, 0);

                break;
            case Side.RIGHT:
                TargetRotation = transform.localEulerAngles + new Vector3(0, 90, 0);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (currentTurnTime < TurnTime)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                          Quaternion.Euler(TargetRotation),
                                                          TurnTime * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        LeftArm.Disable();
        RightArm.Disable();
        TurnLeft.Disable();
        TurnRight.Disable();

        LeftArm.started -= OnLeftCrawl;
        RightArm.started -= OnRightCrawl;
        TurnLeft.started -= OnTurnLeft;
        TurnRight.started -= OnTurnRight;

    }
}
