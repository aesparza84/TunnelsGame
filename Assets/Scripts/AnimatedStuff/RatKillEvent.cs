using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatKillEvent : MonoBehaviour
{
    [Header("Death animation stuff")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string AttackName;
    private int Hash_KillAnim;
    

    public static event EventHandler KilledAnimFinished;
    private void Start()
    {
        LevelMessanger.GameLoopStopped += PlayerDeath;
        Hash_KillAnim = Animator.StringToHash(AttackName);
        _animator.enabled = false;
    }

    private void PlayerDeath(object sender, EventArgs e)
    {
        _animator.enabled = true;
        StartCoroutine(KillAnim());
    }
    private IEnumerator KillAnim()
    {
        _animator.Play(Hash_KillAnim);

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        KilledAnimFinished?.Invoke(this, EventArgs.Empty);

        yield return null;
    }

    private void OnDisable()
    {
        LevelMessanger.GameLoopStopped -= PlayerDeath;
    }
}
