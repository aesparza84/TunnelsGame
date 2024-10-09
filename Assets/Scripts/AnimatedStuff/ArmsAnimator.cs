using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsAnimator : MonoBehaviour
{
    [Header("Player Controller Reference")]
    [SerializeField] private PlayerController _playerController;

    [Header("Limbs")]
    [SerializeField] private GameObject L_Arm;
    [SerializeField] private GameObject R_Arm;
    [SerializeField] private GameObject L_Leg;
    [SerializeField] private GameObject R_Leg;
    private void Start()
    {
        
    }

}
