using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ArmsAnimator : MonoBehaviour
{
    [Header("Player Controller Reference")]
    [SerializeField] private PlayerController _playerController;

    [Header("Animator")]
    [SerializeReference] private Animator _armAnimator;

    [Header("Limb Rigs")]
    [SerializeField] private Rig L_ArmRig;
    [SerializeField] private Rig R_ArmRig;

    [Header("Anim Names")]
    [SerializeField] private string Idle;
    [SerializeField] private string Left;
    [SerializeField] private string Right;
    [SerializeField] private string Map;

    [SerializeField] private float _crossFade;

    private int Hash_Idle;
    private int Hash_Left;
    private int Hash_Right;
    private int Hash_Map;

    private bool Mapping;
    private void Start()
    {
        Hash_Idle = Animator.StringToHash(Idle);
        Hash_Left = Animator.StringToHash(Left);
        Hash_Right = Animator.StringToHash(Right);
        Hash_Map = Animator.StringToHash(Map);

        _playerController.OnPlayerMoved += OnMove;
        _playerController.OnMapCheck += OnMapOpen;
        _playerController.OnMapClose += OnReturnToCrawlArm;
        _playerController.OnAttacked += OnAtk;
        _playerController.OnReleased += OnReturnToCrawlArm;
        LevelMessanger.PlayerReset += PosReset;

        //BackToCrawl();

        L_ArmRig.weight = 1;
        R_ArmRig.weight = 1;

        BackToCrawl();
    }

    private void PosReset(object sender, System.EventArgs e)
    {
        _armAnimator.Play(Hash_Left);
    }

    private void OnAtk(object sender, Vector3 e)
    {        
        _armAnimator.CrossFade(Hash_Idle, 0.5f);
    }

    private void OnReturnToCrawlArm(object sender, System.EventArgs e)
    {
        Mapping = false;
        BackToCrawl();
    }

    private void OnMapOpen(object sender, System.EventArgs e)
    {
        Mapping = true;
        _armAnimator.CrossFade(Hash_Map, _crossFade);
    }

    private void OnMove(object sender, Side e)
    {
        if (Mapping)
            return;

        switch (e)
        {
            case Side.LEFT:
                _armAnimator.CrossFade(Hash_Left, _crossFade);

                break;
            case Side.RIGHT:
                _armAnimator.CrossFade(Hash_Right, _crossFade);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }

    }

    private void BackToCrawl()
    {
        switch (_playerController.CurrentArm)
        {
            case Side.LEFT:
                _armAnimator.CrossFade(Hash_Left, 0.5f);

                break;
            case Side.RIGHT:
                _armAnimator.CrossFade(Hash_Right, 0.5f);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        _playerController.OnPlayerMoved -= OnMove;
        _playerController.OnMapCheck -= OnMapOpen;
        _playerController.OnMapClose -= OnReturnToCrawlArm;
        _playerController.OnAttacked -= OnAtk;
        _playerController.OnReleased -= OnReturnToCrawlArm;
        LevelMessanger.PlayerReset -= PosReset;
    }
}
