using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState { LURKING, HUNTING, ATTACKING, RETREATING, LINGERING}
public enum ActiveState { ACTIVE, DISABLED}

[RequireComponent(typeof(PathFinder))]
public class EnemyBehavior : MonoBehaviour, IEars, ICompActivate
{
    //Pathfinder componenet
    private PathFinder _pathFinder;

    [SerializeField] private Vector2 newPos;

    [Header("Move State Speeds")]
    [SerializeField] private float LurkSpeed;
    [SerializeField] private float HuntSpeed;

    [Header("Linger Time")]
    [SerializeField] private float LingerTime;
    private float currentLingerTime;

    [Header("Attack Cooldown")]
    [SerializeField] private float AttackCooldown;
    private float currentAttackTime;

    private Vector3 AttackLookDirection;
    private Quaternion PrevRotation;

    private const int WallMask = (1<<6);

    //Current Enemy state
    //LURKING - Visiting random nodes
    //HUNTING - Heard Player, moving to sound position
    //ATTACKING - (Doing attack player action)
    private EnemyState _enemyState;

    //Active state of enemy behavior entirly
    private ActiveState _activeState;

    private void Awake()
    {
        LevelMessanger.LevelFinished += OnLevelFinished;
        LevelMessanger.LevelStart += OnLevelStart;

    }
    private void Start()
    {
        _pathFinder = GetComponent<PathFinder>();
        _pathFinder.OnReachedPoint += ReachedDestination;

        EnterState(EnemyState.LURKING);

        
        //Start as Disabled
        _activeState = ActiveState.DISABLED;
    }    

    private void OnLevelStart(object sender, System.EventArgs e)
    {
        //Disable enemy behavior
        Invoke("ActivateComponent", 5);
    }

    private void OnLevelFinished(object sender, System.EventArgs e)
    {
        //Disable enemy behavior
        DisableComponent();
        CancelInvoke();
    }

    public void ActivateComponent()
    {
        _pathFinder.ActivateComponent();
        _activeState = ActiveState.ACTIVE;
        EnterState(EnemyState.LURKING);
    }
    public void DisableComponent()
    {
        _pathFinder.DisableComponent();
        _activeState = ActiveState.DISABLED;
    }
    private void ReachedDestination(object sender, System.EventArgs e)
    {
        EnterState(EnemyState.LINGERING);
    }

    private void Update()
    {
        HandleStates();
    }

    private void HandleStates()
    {
        switch (_enemyState)
        {
            case EnemyState.LURKING:
                MoveToRandomPoint();

                break;
            case EnemyState.HUNTING:
                //Nothing over time

                break;
            case EnemyState.ATTACKING:
                //Nothing over time

                break;

            case EnemyState.RETREATING:
                MoveToRandomPoint();


                break;
            case EnemyState.LINGERING:
                if (currentLingerTime < LingerTime)
                {
                    currentLingerTime += Time.deltaTime;
                }
                else
                {
                    EnterState(EnemyState.LURKING);
                }

                break;
            default:
                break;
        }

        if (currentAttackTime < AttackCooldown)
        {
            currentAttackTime += Time.deltaTime;
        }
    }

    private void MoveToRandomPoint()
    {
        if (_pathFinder._activeState == ActiveState.ACTIVE)
        {
            if (!_pathFinder.Traverse && !_pathFinder.Calculating)
            {
                Debug.Log("Making new path");
                _pathFinder.SetNewDestination(_pathFinder.GetRandomPoint());
            }
        }
    }

    public void Hear(Point heardPoint)
    {
        if (_enemyState == EnemyState.RETREATING)
        {
            return;
        }

        //Debug.Log("heard you!!");
        if (_activeState == ActiveState.ACTIVE)
        {
            if (_enemyState != EnemyState.HUNTING)
            {
                EnterState(EnemyState.HUNTING);
            }

            if (_pathFinder._activeState == ActiveState.ACTIVE)
            {
                if (!_pathFinder.Calculating)
                {
                    _pathFinder.SetNewDestination(heardPoint);
                }
            }
        }             
    }
    private void EnterState(EnemyState _state)
    {
        _enemyState = _state;

        switch (_state)
        {
            case EnemyState.LURKING:
                _pathFinder.SetSpeed(LurkSpeed);
                
                break;
            case EnemyState.HUNTING:
                _pathFinder.SetSpeed(HuntSpeed);
                
                break;
            case EnemyState.ATTACKING:
                _pathFinder.StopTraverse();
                Vector3 dir = Vector3.RotateTowards(transform.forward, AttackLookDirection, 0.2f, 0.0f);
                dir.x = 0;
                dir.z = 0;
                transform.localRotation = Quaternion.LookRotation(dir);
                break;

            case EnemyState.RETREATING:
                _pathFinder.SetSpeed(HuntSpeed);


                break;
            case EnemyState.LINGERING:
                currentLingerTime = 0.0f;

                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentAttackTime >= AttackCooldown)
        {
            AttackIfPossible(other);
        }
    }

    private void AttackIfPossible(Collider other)
    {
        Vector3 dirtTo = other.transform.position - transform.position;
        if (Physics.Raycast(transform.position, dirtTo, dirtTo.magnitude, WallMask))
        {
            return;
        }

        if (other.TryGetComponent<IHideable>(out IHideable h))
        {
            if (h.IsVisible())
            {
                if (other.TryGetComponent<IVulnerable>(out IVulnerable vul))
                {
                    if (_enemyState != EnemyState.ATTACKING)
                    {
                        
                        Debug.Log("Attacking player");
                        EnterState(EnemyState.ATTACKING);
                        vul.OnVulRelease += OnRelease;
                        vul.Attack(transform.position);

                        PrevRotation = transform.rotation;
                        SetLookRotation(vul.GetLookPoint());

                        currentAttackTime = 0.0f;
                    }
                }
            }
        }
    }

    private void SetLookRotation(Vector3 e)
    {
        AttackLookDirection = (e - transform.position).normalized;
    }
    private void OnRelease(object sender, Vector3 e)
    {
        if (sender is IVulnerable vul)
        {
            vul.OnVulRelease -= OnRelease;
        }

        Debug.Log("Released player");

        transform.rotation = PrevRotation;

        EnterState(EnemyState.RETREATING);
    }

    private void OnDisable()
    {
        _pathFinder.OnReachedPoint -= ReachedDestination;
        LevelMessanger.LevelFinished -= OnLevelFinished;
        LevelMessanger.LevelStart -= OnLevelStart;
    }
}
