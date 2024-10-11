using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DooHickyScreenMode : MonoBehaviour
{
    [Header("Different Render Txtr.")]
    [SerializeField] private Material _Living;
    [SerializeField] private Material _Itmes;
    private MeshRenderer _render;

    private bool LivingMode;

    [SerializeField] private PlayerController _playerController;

    private void Start()
    {
        _render = GetComponent<MeshRenderer>();
        _render.material = _Living;
        LivingMode = true;

        if (_playerController != null)
        {
            _playerController.OnHickySwitch += OnSwitchmode;
        }
    }

    private void OnSwitchmode(object sender, System.EventArgs e)
    {
        if (LivingMode)
        {
            _render.material = _Itmes;
            LivingMode = false;
        }
        else
        {
            _render.material = _Living;
            LivingMode = true;
        }
    }

    //Unsubscribe
    private void OnDisable()
    {
        _playerController.OnHickySwitch -= OnSwitchmode;
    }
}
