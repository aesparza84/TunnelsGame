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

    [Header("Health")]
    [SerializeField] private HealthComponent _healthComponent;

    //Input references
    private InputAction LeftArm;
    private InputAction RightArm;
    private InputAction TurnLeft;
    private InputAction TurnRight;
    private InputAction Punch;
    private InputAction Kick;
    private InputAction CheckMap;
    private InputAction DooHickyMode;

    private Side currentArm;
    public Side CurrentArm { get { return currentArm; } }

    //Readonly player direction
    public OpeningSide FacingDirection { get; private set; }

    [Header("Movement Fields")]
    [SerializeField] private float RegularMoveSpeed;
    [SerializeField] private float MapMoveSpeed;
    private float CurrentMoveSpeed;
    private Vector3 TargetPos;

    [Header("Flashlight")]
    [SerializeField] private Light _flashLight;

    //Camera info, distance percent
    [SerializeField] private Transform _camTransform;

    //Player turning
    [SerializeField] private float TurnTime;
    private Vector3 TargetRotation;
    private Point CurrentMapPoint;

    [Header("Noie Detect Values")]
    [SerializeField] private float CrawlSoundRadius;
    [SerializeField] private float MaxSoundRadius;
    [SerializeField] private float SoundCoolDownRate;
    private float CurrentSoundRadius;

    [Header("Vulnerable Attacked cooldown")]
    [SerializeField] private float VictimTimer;
    private float VictimCooldown;

    [Header("Retaliate Cooldown")]
    [SerializeField] private float AttackCoolDown;
    private float currentAttackCooldown;

    //Retaliate Weapons
    private Weapon _weaponHand;
    private Weapon _weaponFoot;

    private bool CanAttack { get { return currentAttackCooldown >= AttackCoolDown; } }
    private bool AttackingFromFront;

    //Arm Crawl Events
    public event EventHandler OnLeft;
    public event EventHandler OnRight;

    //Attack events & fields
    public event EventHandler<Vector3> OnAttacked;
    public event EventHandler OnReleased;
    public event EventHandler OnVulRelease;
    public event EventHandler<Side> OnVulRetaliate;
    public event EventHandler OnMapCheck;
    public event EventHandler OnMapClose;
    public event EventHandler<Side> OnPlayerMoved;
    private int currentEncounterTime;

    //Misc. events
    public event EventHandler OnHickySwitch;
    public static event EventHandler OnRetalHit;
    public static event EventHandler OnAttackLowHealth;
    public static event EventHandler OnAttackStatic;
    public static event EventHandler OnAttackRelease;
    public static event EventHandler OnDeath;

    //Ear Collider Array
    private Collider[] EarColliders;
    private const int EnemyLayer = (1 << 7);

    //Move event
    public event EventHandler<Tuple<OpeningSide, Side>> OnMove; //Facing-Dir, Arm side
    [SerializeField] private bool MapMove;



    //Map using bool
    private bool CheckingMap;
    private bool CanMap {  get { return !Attacked && !Moving && !Turning; } }

    //Visible Status
    private bool Visible;

    private bool Turning;
    private bool Moving;
    private const float RegInputCoolDown = 0.25f;
    private const float MapInputCoolDown = 0.7f;
    private float SetInputCoolDown;
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

        currentCoolDown = SetInputCoolDown;

        _healthComponent.OnPlayerDeath += PlayerDeath;
        LevelMessanger.LevelExitCompleted += EndLevel;
        LevelMessanger.LevelStart += LevelReady;

        PauseMenu.OnPause += OnPause;
        PauseMenu.OnResume += OnResum;
        //Start Disabled
        _activeState = ActiveState.DISABLED;
    }

    private void OnResum(object sender, EventArgs e)
    {
        EnableAllControls();
    }

    private void OnPause(object sender, EventArgs e)
    {
        DisableAllControls();
    }

    private void PlayerDeath(object sender, EventArgs e)
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        DisableAllControls();
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

        //Reset filter if on
        OnAttackRelease?.Invoke(this, EventArgs.Empty);

        CompassDirection(Vector3.Dot(transform.forward, Vector3.forward));
        currentCoolDown = SetInputCoolDown;
        EnableAllControls();
        _activeState = ActiveState.ACTIVE;
        CurrentMoveSpeed = RegularMoveSpeed;

        VictimCooldown = VictimTimer;
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

        Punch = _mappedInputs.Crawl.Punch;
        Punch.Disable();
        Punch.started += OnPunch;

        Kick = _mappedInputs.Crawl.Kick;
        Kick.Disable();
        Kick.started += OnKick;

        CheckMap = _mappedInputs.Crawl.CheckMap;
        CheckMap.Enable();
        CheckMap.started += OnMapOpen;
        CheckMap.canceled += OnCloseMap;

        DooHickyMode = _mappedInputs.Crawl.DooHickySwitch;
        DooHickyMode.Enable();
        DooHickyMode.started += OnDooHickySwitch;
    }

    private void DisableAllControls()
    {
        LeftArm.Disable();
        RightArm.Disable();
        TurnLeft.Disable();
        RightArm.Disable();
        CheckMap.Disable();
        DooHickyMode.Disable();
    }
    private void EnableAllControls()
    {
        if (Attacked)
        {
            Punch.Enable();
            Kick.Enable();
            return;
        }

        LeftArm.Enable();
        RightArm.Enable();
        TurnLeft.Enable();
        RightArm.Enable();
        CheckMap.Enable();
        DooHickyMode.Enable();
    }
    private void OnDooHickySwitch(InputAction.CallbackContext obj)
    {
        OnHickySwitch?.Invoke(this, EventArgs.Empty);
    }
    private void OnMapOpen(InputAction.CallbackContext obj)
    {
        if (CanMap)
        {
            OpenMap();
        }
    }
    private void OnCloseMap(InputAction.CallbackContext obj)
    {
        CloseMap();
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

    #region Action Map Inputs
    private void OnTurnLeft(InputAction.CallbackContext obj)
    {
        Turn(Side.LEFT);
    }
    private void OnTurnRight(InputAction.CallbackContext obj)
    {
        Turn(Side.RIGHT);
    }

    private void OnPunch(InputAction.CallbackContext obj)
    {
        if (CanAttack)
        {
            if (_weaponHand != null)
            {
                if (!_weaponHand.Broken)
                {
                    Retaliate(true, ref _weaponHand);
                }
            }
        }
    }

    private void OnKick(InputAction.CallbackContext obj)
    {
        if (CanAttack)
        {
            if (_weaponFoot != null)
            {
                if (!_weaponFoot.Broken)
                {
                    Retaliate(false, ref _weaponFoot);
                }
            }
        }
    }
    #endregion
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
                    OnPlayerMoved?.Invoke(this, currentArm);

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
                    OnPlayerMoved?.Invoke(this, currentArm);

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
            //Attack Cooldown
            if (!CanAttack)
            {
                currentAttackCooldown += Time.deltaTime;
            }
        }
        else
        {
            if (currentCoolDown < SetInputCoolDown)
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

            if (VictimCooldown < VictimTimer)
            {
                VictimCooldown += Time.deltaTime;
            }
        }
    }
    
    private void MakeNoise(float noiseValue)
    {
        CurrentSoundRadius += noiseValue;
        CurrentSoundRadius = Mathf.Clamp(CurrentSoundRadius, 0, MaxSoundRadius);
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
                transform.position = Vector3.MoveTowards(transform.position, TargetPos, CurrentMoveSpeed * Time.deltaTime);
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
        return currentCoolDown >= SetInputCoolDown;
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
        Kick.started -= OnKick;
        Punch.started -= OnPunch;
        CheckMap.started -= OnMapOpen;
        DooHickyMode.started -= OnDooHickySwitch;

        _healthComponent.OnPlayerDeath -= PlayerDeath;
        LevelMessanger.LevelExitCompleted -= EndLevel;
        LevelMessanger.LevelStart -= LevelReady;

        PauseMenu.OnPause -= OnPause;
        PauseMenu.OnResume -= OnResum;
    }

    public void SetWeapon(Weapon w)
    {
        if (w.HandSide) //If the weapon is meant for hand
        {
            if (_weaponHand != null)
            {
                //Make trash noise
            }

            //Replace hand weapon
            _weaponHand = w;
        }
        else
        {
            if (_weaponFoot != null)
            {
                //Make trash noise
            }

            //Replace hand weapon
            _weaponFoot = w;
        }

    }

    private void OpenMap()
    {
        if (CanMap)
        {
            OnMapCheck?.Invoke(this, EventArgs.Empty);
            CurrentMoveSpeed = MapMoveSpeed;
            SetInputCoolDown = MapInputCoolDown;
            _flashLight.enabled = false;

            CheckingMap = true;
        }
    }
    private void CloseMap()
    {
        if (CheckingMap)
        {
            OnMapClose?.Invoke(this, EventArgs.Empty);
            CurrentMoveSpeed = RegularMoveSpeed;
            SetInputCoolDown = RegInputCoolDown;
            _flashLight.enabled = true;
            CheckingMap = false;
        }
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

    public void Attack(Vector3 p, int encounterSeconds)
    {
        CloseMap();
        OnAttacked?.Invoke(this, p);
        OnAttackStatic?.Invoke(this, EventArgs.Empty);

        if ( (_healthComponent.Health / _healthComponent.MaxHealth) <= 0.3f)
        {
            OnAttackLowHealth?.Invoke(this, EventArgs.Empty);
        }

        Vector3 dir = (p - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);
        currentAttackCooldown = AttackCoolDown; //Reset attack timer

        //Set to front by default, change if needed by DOT
        AttackingFromFront = true;

        if (dot < -0.2f)
        {
            AttackingFromFront = false;
        }

        Punch.Enable();
        Kick.Enable();

        DisableAllControls();
        Attacked = true;

        StartCoroutine(AttackCoroutine(_healthComponent, encounterSeconds));
    }
    private IEnumerator AttackCoroutine(IHealth hel, int EncounterTime)
    {
        currentEncounterTime = 0;
        hel.TakeDamage(1);


        while (currentEncounterTime < EncounterTime)
        {
            yield return new WaitForSeconds(1);
            currentEncounterTime++;
            Debug.Log(currentEncounterTime + " Second(s) have passed");

            hel.TakeDamage(1);

            yield return null;
        }

        Release();
        yield return null;
    }

    public bool CanAttackThis()
    {
        return (VictimCooldown >= VictimTimer) && (!Attacked);
    }


    public Vector3 GetLookPoint()
    {
        return _camTransform.position;
    }

    public void Retaliate(bool inFront, ref Weapon w)
    {
        //If the side matches the enemy side, Successful attack
        if (inFront == AttackingFromFront)
        {
            //Reduce time 
            w.UseWeapon();
            currentEncounterTime += w.Stagger;

            //Get direction rat is at
            Side s = Side.LEFT;


            if (!AttackingFromFront)
            {
                s = Side.LEFT;
            }

            OnRetalHit?.Invoke(this, EventArgs.Empty); //Camera Event
            OnVulRetaliate?.Invoke(this, s);
            currentAttackCooldown = 0.0f;
        }
    }
    
    public void Release()
    {

        Punch.Disable();
        Kick.Disable();

        OnAttackRelease?.Invoke(this, EventArgs.Empty);
        OnReleased?.Invoke(this, EventArgs.Empty);
        OnVulRelease?.Invoke(this, EventArgs.Empty);
        Attacked = false;
        VictimCooldown = 0.0f;
        EnableAllControls();
    }
}
