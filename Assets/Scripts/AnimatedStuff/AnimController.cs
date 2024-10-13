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
    [SerializeField] private string RageName;

    [Header("IK_Point")]
    [SerializeField] private Transform IK_GrabPoint;
    private Transform CurrentLimbTransform;

    //Hashed References
    private int Hash_Idlename;
    private int Hash_RoamName;
    private int Hash_ATKName;
    private int Hash_InterruptLeftName;
    private int Hash_InterruptRightName;
    private int Hash_Rage;
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
        _enemyBehavior.OnRoamFromAttack += Roam;
        _enemyBehavior.OnIdle += Idle;
    }

    
    private void Idle(object sender, System.EventArgs e)
    {
        _animator.Play(Hash_Idlename);
    }

    private void Roam(object sender, System.EventArgs e)
    {
        StopAllCoroutines();
        ReleaseLimb();
        _animator.CrossFade(Hash_RoamName, 0.3f);
    }
    private void Retreat(object sender, System.EventArgs e)
    {
        ReleaseLimb();
        //StartCoroutine(ChainAnimation(Hash_Rage, Hash_RoamName, 0.4f));
        _animator.CrossFade(Hash_RoamName, 0.3f);
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
                StartCoroutine(ChainAnimation(Hash_InterruptLeftName, Hash_ATKName, 1));

                break;
            case Side.RIGHT:
                _animator.Play(InterruptRightName);
                StartCoroutine(ChainAnimation(Hash_InterruptRightName, Hash_ATKName, 1));

                break;
            case Side.NONE:
                break;
            default:
                break;
        }

    }

    private IEnumerator ChainAnimation(int from, int to, float normalizedCut)
    {
        _animator.CrossFade(from, 0.1f);

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < normalizedCut)
        {
            yield return null;
        }

        _animator.CrossFade(to, 0.1f);

        yield return null;
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
        _enemyBehavior.OnATKInterupted -= ATK_Interrupted;
        _enemyBehavior.OnAttack -= ATK;
        _enemyBehavior.OnRoam -= Roam;
        _enemyBehavior.OnRoamFromAttack -= Retreat;
        _enemyBehavior.OnIdle -= Idle;
    }
}
