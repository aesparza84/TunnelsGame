using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsAnimator : MonoBehaviour
{
    [Header("Player Controller Reference")]
    [SerializeField] private PlayerController _playerController;

    [Header("Limbs")]
    [SerializeReference] private Animator _armAnimator;

    [Header("Anim Names")]
    [SerializeField] private string Left;
    [SerializeField] private string Right;
    [SerializeField] private string Map;

    private int Hash_Left;
    private int Hash_Right;
    private int Hash_Map;

    private Side prevSide;
    private void Start()
    {
        Hash_Left = Animator.StringToHash(Left);
        Hash_Right = Animator.StringToHash(Right);
        Hash_Map = Animator.StringToHash(Map);

        _playerController.OnMove += OnMove;
        _playerController.OnMapCheck += OnMapOpen;
        _playerController.OnMapClose += OnMapClosed;
    }

    private void OnMapClosed(object sender, System.EventArgs e)
    {
        switch (prevSide)
        {
            case Side.LEFT:
                _armAnimator.Play(Hash_Left);

                break;
            case Side.RIGHT:
                _armAnimator.Play(Hash_Right);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }
    }

    private void OnMapOpen(object sender, System.EventArgs e)
    {
        _armAnimator.Play(Hash_Map);
    }

    private void OnMove(object sender, System.Tuple<OpeningSide, Side> e)
    {
        switch (e.Item2)
        {
            case Side.LEFT:
                _armAnimator.Play(Hash_Left);

                break;
            case Side.RIGHT:
                _armAnimator.Play(Hash_Right);

                break;
            case Side.NONE:
                break;
            default:
                break;
        }

        prevSide = e.Item2;
    }

    private void OnDisable()
    {
        _playerController.OnMove -= OnMove;
        _playerController.OnMapCheck -= OnMapOpen;
        _playerController.OnMapClose -= OnMapClosed;
    }
}
