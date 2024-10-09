using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Side { LEFT, RIGHT, NONE };
public enum VisibleStatus { VISIBLE, HIDDEN};
public class PlayerController : MonoBehaviour, IHideable, ICompActivate, IVulnerable
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
    [SerializeField] private Transform _camTransform;

    //Player turning
    [SerializeField] private float TurnTime;
    private Vector3 TargetRotation;
    private Point CurrentMapPoint;

    [Header("Noie Detect Values")]
    [SerializeField] private float CrawlSoundRadius;
    [SerializeField] private float SoundCoolDownRate;
    private float CurrentSoundRadius;


    //Arm Crawl Events
    public event EventHandler OnLeft;
    public event EventHandler OnRight;

    //Attack event
    public event EventHandler<Vector3> OnAttacked;
    public event EventHandler OnReleased;
    public event EventHandler<Vector3> OnVulRelease;

    //Ear Collider Array
    private Collider[] EarColliders;
    private const int EnemyLayer = (1 << 7);

    //Move event
    public event EventHandler<Tuple<OpeningSide, Side>> OnMove; //Facing-Dir, Arm side
    [SerializeField] private bool MapMove;

    //Visible Status
    private bool Visible;

    private bool Turning;
    private bool Moving;
    private const float InputCoolDown = 0.25f;
    private float currentCoolDown;

    //Active state of Player Controller
    private ActiveState _activeState;

    private bool Attacked;

    private void Start()
    {
        //Start on Left side
        currentArm = Side.LEFT;
        Visible = true;

        TargetRotation = transform.localRotation.eulerAngles;

        CompassDirection(Vector3.Dot(transform.forward, Vector3.forward));

        currentCoolDown = InputCoolDown;

        LevelMessanger.LevelFinished += EndLevel;
        LevelMessanger.LevelStart += LevelReady;

        //Start Disabled
        _activeState = ActiveState.DISABLED;
    }

    private void LevelReady(object sender, EventArgs e)
    {
        ActivateComponent();
    }

    private void EndLevel(object sender, EventArgs e)
    {
        DisableAllControls();
    }

    public void ActivateComponent()
    {
        currentArm = Side.LEFT;
        Visible = true;

        TargetRotation = transform.localRotation.eulerAngles;

        CompassDirection(Vector3.Dot(transform.forward, Vector3.forward));
        currentCoolDown = InputCoolDown;
        EnableAllControls();
        _activeState = ActiveState.ACTIVE;
    }

    public void DisableComponent()
    {
        DisableAllControls();
        _activeState = ActiveState.DISABLED;
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

    private void DisableAllControls()
    {
        LeftArm.Disable();
        RightArm.Disable();
        TurnLeft.Disable();
        RightArm.Disable();
    }
    private void EnableAllControls()
    {
        LeftArm.Enable();
        RightArm.Enable();
        TurnLeft.Enable();
        RightArm.Enable();
    }

    private void OnRightCrawl(InputAction.CallbackContext obj)
    {
        if (!InputsChilled())
            return;

        if (MapMove)
        {
            Tuple<OpeningSide, Side> info = new Tuple<OpeningSide, Side>(FacingDirection, Side.RIGHT);

            OnMove?.Invoke(this, info);
        }
        else
        {
            Crawl(Side.RIGHT);
        }

        currentCoolDown = 0.0f;
    }
    private void OnLeftCrawl(InputAction.CallbackContext obj)
    {
        if (!InputsChilled())
            return;

        if (MapMove)
        {
            Tuple<OpeningSide, Side> info = new Tuple<OpeningSide, Side>(FacingDirection, Side.LEFT);

            OnMove?.Invoke(this, info);
        }
        else
        {
            Crawl(Side.LEFT);
        }

        currentCoolDown = 0.0f;
    }
    private void OnTurnLeft(InputAction.CallbackContext obj)
    {
        Turn(Side.LEFT);
    }
    private void OnTurnRight(InputAction.CallbackContext obj)
    {
        Turn(Side.RIGHT);
    }

    public bool Crawl(Side side, Point newPoint)
    {
        if (Moving || Turning)
            return false;

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

                    Moving = true;
                    CurrentMapPoint = newPoint;
                    MakeNoise(CrawlSoundRadius);
                    return true;
                }

                break;
            case Side.RIGHT:
                if (side == Side.RIGHT)
                {
                    OnRight?.Invoke(this, EventArgs.Empty);
                    currentArm = Side.LEFT;

                    Moving = true;
                    CurrentMapPoint = newPoint;
                    MakeNoise(CrawlSoundRadius);
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
    public bool Crawl(Side side)
    {
        if (Moving || Turning)
            return false;

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

                    Moving = true;
                    MakeNoise(CrawlSoundRadius);
                    return true;
                }

                break;
            case Side.RIGHT:
                if (side == Side.RIGHT)
                {
                    OnRight?.Invoke(this, EventArgs.Empty);
                    currentArm = Side.LEFT;

                    Moving = true;
                    MakeNoise(CrawlSoundRadius);
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
        if (Attacked)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Release();
            }
        }
        else
        {
            if (currentCoolDown < InputCoolDown)
            {
                currentCoolDown += Time.deltaTime;
            }

            if (CurrentSoundRadius > 0.0f)
            {
                AlertEars();
                CurrentSoundRadius -= (SoundCoolDownRate * Time.deltaTime);
            }


            if (_activeState == ActiveState.ACTIVE)
            {
                HandleRotation();
                HandleMovement();
            }
        }            
    }
    
    private void MakeNoise(float noiseValue)
    {
        CurrentSoundRadius += noiseValue;
    }

    private void AlertEars()
    {
        EarColliders = Physics.OverlapSphere(transform.position, CurrentSoundRadius, EnemyLayer);

        if (EarColliders.Length > 0)
        {
            for (int i = 0; i < EarColliders.Length; i++)
            {
                if (EarColliders[i].transform.parent.TryGetComponent<IEars>(out IEars e))
                {
                    e.Hear(CurrentMapPoint);
                }
            }
        }
    }

    private void HandleRotation()
    {
        if (Turning)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                          Quaternion.Euler(TargetRotation),
                                                          TurnTime * Time.deltaTime);

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
            else
            {
                Moving = false;

            }          
        }
    }

    //To prevent move spam
    private bool InputsChilled()
    {
        return currentCoolDown >= InputCoolDown;
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

        LevelMessanger.LevelFinished -= EndLevel;
        LevelMessanger.LevelStart -= LevelReady;
    }

    public void Hide()
    {
        Visible = false;
    }

    public void Reveal()
    {
        Visible = true;

    }

    public bool IsVisible()
    {
        return Visible;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CurrentSoundRadius);
    }

    public void Attack(Vector3 p)
    {
        OnAttacked?.Invoke(this, p);
        DisableAllControls();
        Attacked = true;
    }

    public Vector3 GetLookPoint()
    {
        return _camTransform.position;
    }
    public void Release()
    {
        OnReleased?.Invoke(this, EventArgs.Empty);
        OnVulRelease?.Invoke(this, _camTransform.position);
        EnableAllControls();
        Attacked = false;
    }
}
