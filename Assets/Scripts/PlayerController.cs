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
    public Side CurrentArm { get { return currentArm; } }


    //Player movement
    [SerializeField] private float MoveSpeed;
    private Vector3 TargetPos;

    //Camera info, distance percent
    private Vector3 startPos;
    private float startDistance;
    private float percentMoved;

    //Player turning
    [SerializeField] private float TurnTime;
    private Vector3 TargetRotation;


    //Arm Crawl Events
    public event EventHandler OnLeft;
    public event EventHandler OnRight;

    private bool Turning;
    private bool Moving;

    private void Start()
    {
        //Start on Left side
        currentArm = Side.LEFT;

        TargetRotation = transform.localRotation.eulerAngles;
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
    }
    private void OnLeftCrawl(InputAction.CallbackContext obj)
    {
        Crawl(Side.LEFT);
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
        //If currently moving or turning, dont move
        if (Moving || Turning)
            return;

        //Set the target position
        TargetPos = transform.position + (transform.forward).normalized;

        //Info for camera.
        startPos = transform.position;
        startDistance = Vector3.Distance(transform.position, TargetPos);

        //Only move when input matches side
        switch (currentArm)
        {
            case Side.LEFT:
                if (side == Side.LEFT)
                {
                    OnLeft?.Invoke(this, EventArgs.Empty);

                    Moving = true;
                    currentArm = Side.RIGHT;
                }

                break;
            case Side.RIGHT:
                if (side == Side.RIGHT)
                {
                    OnRight?.Invoke(this, EventArgs.Empty);

                    Moving = true;
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
        //If currently moving or turning, dont move
        if (Moving || Turning)
            return;
        
        //Set new target rotation
        switch (newTurn)
        {
            case Side.LEFT:
                TargetRotation += new Vector3(0, -90.0f, 0);

                break;
            case Side.RIGHT:
                TargetRotation += new Vector3(0, 90.0f, 0);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }

        if (Mathf.Abs(TargetRotation.y) == 360)
        {
            TargetRotation.y = 0;
        }

        Turning = true; 
    }

    private void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        if (Turning)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                          Quaternion.Euler(TargetRotation),
                                                          TurnTime);

            if (transform.rotation == Quaternion.Euler(TargetRotation))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                          Quaternion.Euler(TargetRotation),
                                                          100);
                Turning = false;
            }
        }
    }

    private void HandleMovement()
    {
        if (Moving)
        {
            if (transform.position != TargetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetPos, MoveSpeed);
            }

            if (transform.position == TargetPos)
            {
                Moving = false;
            }            
        }
    }

    private void OnDisable()
    {
        //Disable controls
        LeftArm.Disable();
        RightArm.Disable();
        TurnLeft.Disable();
        TurnRight.Disable();

        //Unsubscirbe events
        LeftArm.started -= OnLeftCrawl;
        RightArm.started -= OnRightCrawl;
        TurnLeft.started -= OnTurnLeft;
        TurnRight.started -= OnTurnRight;
    }
}
