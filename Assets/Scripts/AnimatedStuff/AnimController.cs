using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimController : MonoBehaviour
{
    [Header("Animator Refernce")]
    [SerializeField] private Animator _animator;

    [Header("Behavior reference")]
    [SerializeField] private EnemyBehavior _enemyBehavior;

    [Header("Anim string hash")]
    [SerializeField] private string Idlename;
    [SerializeField] private string RoamName;
    [SerializeField] private string ATKName;
    [SerializeField] private string InterruptLeftName;
    [SerializeField] private string InterruptRightName;

    [Header("IK_Point")]
    [SerializeField] private Transform IK_GrabPoint;
    private Transform CurrentLimbTransform;

    //Hashed References
    private int Hash_Idlename;
    private int Hash_RoamName;
    private int Hash_ATKName;
    private int Hash_InterruptLeftName;
    private int Hash_InterruptRightName;
    private void Start()
    {
        if (_enemyBehavior == null)
            _enemyBehavior = GetComponent<EnemyBehavior>();

        //Hash Anims to call later
        Hash_Idlename = Animator.StringToHash(Idlename);
        Hash_RoamName = Animator.StringToHash(RoamName);
        Hash_ATKName = Animator.StringToHash(ATKName);
        Hash_InterruptLeftName = Animator.StringToHash(InterruptLeftName);
        Hash_InterruptRightName = Animator.StringToHash(InterruptRightName);

        _enemyBehavior.OnATKInterupted += ATK_Interrupted;
        _enemyBehavior.OnAttack += ATK;
        _enemyBehavior.OnRoam += Roam;
        _enemyBehavior.OnIdle += Idle;
    }

    private void Idle(object sender, System.EventArgs e)
    {
        _animator.Play(Hash_Idlename);
    }

    private void Roam(object sender, System.EventArgs e)
    {
        ReleaseLimb();
        _animator.Play(Hash_RoamName);
    }

    private void ATK(object sender, System.EventArgs e)
    {
        _animator.Play(Hash_ATKName);

        Collider[] c = Physics.OverlapSphere(transform.position, 3, (1 << 10));

        if (c.Length > 0)
        {
            GrabLimb(c[0].transform);
        }
    }

    private void ATK_Interrupted(object sender, Side e)
    {
        switch (e)
        {
            case Side.LEFT:
                _animator.Play(InterruptLeftName);

                break;
            case Side.RIGHT:
                _animator.Play(InterruptRightName);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (CurrentLimbTransform != null)
        {
            CurrentLimbTransform.position = IK_GrabPoint.position;
        }
    }

    private void GrabLimb(Transform IK_Target)
    {
        CurrentLimbTransform = IK_Target;
    }
    
    private void ReleaseLimb()
    {
        CurrentLimbTransform = null;
    }
    private void OnDisable()
    {
        _enemyBehavior.OnATKInterupted += ATK_Interrupted;
        _enemyBehavior.OnAttack += ATK;
        _enemyBehavior.OnRoam += Roam;
        _enemyBehavior.OnIdle += Idle;
    }
}
