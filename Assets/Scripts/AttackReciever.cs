using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackReciever : MonoBehaviour
{
    [Header("Low FPS Render Image")]
    [SerializeField] private GameObject _imageObject;

    void Start()
    {
        PlayerController.OnAttackLowHealth += Attack;
        PlayerController.OnAttackRelease += Release;

        _imageObject.SetActive(false);
    }

    private void Release(object sender, System.EventArgs e)
    {
        _imageObject.SetActive(false);
    }

    private void Attack(object sender, System.EventArgs e)
    {
        _imageObject.SetActive(true);
    }

    private void OnDisable()
    {
        PlayerController.OnAttackLowHealth -= Attack;
        PlayerController.OnAttackRelease -= Release;
    }
}
