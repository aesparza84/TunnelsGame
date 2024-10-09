using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState { LURKING, HUNTING, ATTACKING, LINGERING}
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
    [SerializeField] private float ChaseSpeed;

    [Header("Linger Time")]
    [SerializeField] private float LingerTime;
    private float currentLingerTime;

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
                if (_pathFinder._activeState == ActiveState.ACTIVE)
                {
                    if (!_pathFinder.Traverse && !_pathFinder.Calculating)
                    {
                        Debug.Log("Making new path");
                        _pathFinder.SetNewDestination(_pathFinder.GetRandomPoint());
                    }
                }
                

                break;
            case EnemyState.HUNTING:
                

                break;
            case EnemyState.ATTACKING:
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
    }
    public void Hear(Point heardPoint)
    {
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
                _pathFinder.SetSpeed(ChaseSpeed);
                
                break;

            case EnemyState.LINGERING:
                currentLingerTime = 0.0f;

                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        _pathFinder.OnReachedPoint -= ReachedDestination;
        LevelMessanger.LevelFinished -= OnLevelFinished;
        LevelMessanger.LevelStart -= OnLevelStart;
    }
}
