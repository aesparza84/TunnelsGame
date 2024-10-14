using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LimbsAnimator : MonoBehaviour
{
    [Header("Player Controller Reference")]
    [SerializeField] private PlayerController _playerController;

    [Header("Animators")]
    [SerializeReference] private Animator _armAnimator;
    [SerializeReference] private Animator _legAnimator;

    [Header("Limb Rigs")]
    [SerializeField] private Rig L_ArmRig;
    [SerializeField] private Rig R_ArmRig;
    [SerializeField] private Rig L_LegRig;
    [SerializeField] private Rig R_legRig;

    [Header("Arm Anim Names")]
    [SerializeField] private string Idle;
    [SerializeField] private string Left;
    [SerializeField] private string Right;
    [SerializeField] private string Map;

    [Header("Leg Anim Names")]
    [SerializeField] private string NoRigName;
    [SerializeField] private string LeftCrawlName;
    [SerializeField] private string RightCrawlName;

    [Header("Cross fade time")]
    [SerializeField] private float _crossFade;

    //Arm Hashes
    private int Hash_Idle;
    private int Hash_Left;
    private int Hash_Right;
    private int Hash_Map;

    //Leg Hashes
    private int Hash_LegNoRig;
    private int Hash_LegLeftCrawl;
    private int Hash_LegRightCrawl;

    private bool Mapping;
    private void Start()
    {
        //Arms
        Hash_Idle = Animator.StringToHash(Idle);
        Hash_Left = Animator.StringToHash(Left);
        Hash_Right = Animator.StringToHash(Right);
        Hash_Map = Animator.StringToHash(Map);

        //Legs
        Hash_LegNoRig = Animator.StringToHash(NoRigName);
        Hash_LegLeftCrawl = Animator.StringToHash(LeftCrawlName);
        Hash_LegRightCrawl = Animator.StringToHash(RightCrawlName);

        _playerController.OnPlayerMoved += OnMove;
        _playerController.OnMapCheck += OnMapOpen;
        _playerController.OnMapClose += OnReturnToCrawlArm;
        _playerController.OnAttacked += OnAtk;
        _playerController.OnReleased += OnReturnToCrawlArm;
        LevelMessanger.PlayerReset += PosReset;

        //BackToCrawl();

        L_ArmRig.weight = 1;
        R_ArmRig.weight = 1;
        L_LegRig.weight = 1;
        R_legRig.weight = 1;

        BackToCrawl();
    }

    private void PosReset(object sender, System.EventArgs e)
    {
        _armAnimator.Play(Hash_Left);
        _legAnimator.Play(Hash_LegRightCrawl, 0);
    }

    private void OnAtk(object sender, Vector3 e)
    {        
        _armAnimator.CrossFade(Hash_Idle, 0.5f);
        _legAnimator.CrossFade(Hash_LegNoRig, 0.5f, 0);
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
                _legAnimator.CrossFade(Hash_LegLeftCrawl, _crossFade);

                break;
            case Side.RIGHT:
                _armAnimator.CrossFade(Hash_Right, _crossFade);
                _legAnimator.CrossFade(Hash_LegRightCrawl, _crossFade);

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
                _legAnimator.CrossFade(Hash_LegLeftCrawl, 0.5f, 0);

                break;
            case Side.RIGHT:
                _armAnimator.CrossFade(Hash_Right, 0.5f);
                _legAnimator.CrossFade(Hash_LegRightCrawl, 0.5f, 0);

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
