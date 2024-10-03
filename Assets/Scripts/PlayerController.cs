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

    //Readonly player direction
    public OpeningSide FacingDirection { get; private set; }

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

    //Move event
    public event EventHandler<Tuple<OpeningSide, Side>> OnMove; //Facing-Dir, Arm side
    [SerializeField] private bool MapMove;

    private bool Turning;
    private bool Moving;

    private void Start()
    {
        //Start on Left side
        currentArm = Side.LEFT;

        TargetRotation = transform.localRotation.eulerAngles;

        CompassDirection(Vector3.Dot(transform.forward, Vector3.forward));
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
        //accept no input when moving
        if (Moving)
            return;

        Moving = true;

        if (MapMove)
        {
            Tuple<OpeningSide, Side> info = new Tuple<OpeningSide, Side>(FacingDirection, Side.RIGHT);

            OnMove?.Invoke(this, info);
        }
        else
        {
            Crawl(Side.RIGHT);
        }
    }
    private void OnLeftCrawl(InputAction.CallbackContext obj)
    {
        //accept no input when moving
        if (Moving)
            return;

        Moving = true;

        if (MapMove)
        {
            Tuple<OpeningSide, Side> info = new Tuple<OpeningSide, Side>(FacingDirection, Side.LEFT);

            OnMove?.Invoke(this, info);
        }
        else
        {
            Crawl(Side.LEFT);
        }
    }
    private void OnTurnLeft(InputAction.CallbackContext obj)
    {
        Turn(Side.LEFT);
    }
    private void OnTurnRight(InputAction.CallbackContext obj)
    {
        Turn(Side.RIGHT);
    }

    public bool Crawl( Side side)
    {    

        //Set the target position 
        //Mapmove = node movement
        //!Mapmove = free movement
        if (!MapMove)
            TargetPos = transform.position + (transform.forward).normalized;


        //Only move when input matches side
        switch (currentArm)
        {
            case Side.LEFT:
                if (side == Side.LEFT)
                {
                    OnLeft?.Invoke(this, EventArgs.Empty);
                    currentArm = Side.RIGHT;

                    return true;
                }

                break;
            case Side.RIGHT:
                if (side == Side.RIGHT)
                {
                    OnRight?.Invoke(this, EventArgs.Empty);
                    currentArm = Side.LEFT;

                    return true;
                }

                break;
            case Side.NONE:
                break;
            default:
                break;
        }

        return false;
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


        //Limit rotation values |0 > x > 270|
        if (Mathf.Abs(TargetRotation.y) == 360)
        {
            TargetRotation.y = 0;
        }

        //Set READONLY facing direction

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

                CompassDirection(Vector3.Dot(transform.forward, Vector3.forward));
            }
        }
    }
    //Determine direction, DOT
    private void CompassDirection(float dot)
    {
        dot = Mathf.Round(dot);

        if (Mathf.Approximately(dot, 0.0f))
        {
            if (transform.localRotation.eulerAngles.y == 90)
            {
                FacingDirection = OpeningSide.E;
            }
            else
            {
                FacingDirection = OpeningSide.W;
            }
        }
        else if (Mathf.Approximately(dot, 1.0f))
        {
            FacingDirection = OpeningSide.N;
        }
        else if (Mathf.Approximately(dot, -1.0f))
        {
            FacingDirection = OpeningSide.S;
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

    //For External classes
    public void SetTargetPos(Vector3 tar)
    {
        TargetPos = tar;
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
