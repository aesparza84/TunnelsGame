using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DooHickyScreenMode : MonoBehaviour
{
    [Header("Different Render Txtr.")]
    [SerializeField] private Material _Living;
    [SerializeField] private Material _Itmes;
    private MeshRenderer _render;

    [Header("Light")]
    [SerializeField] private Light _screenLight;

    [Header("Colors")]
    [SerializeField] private Color _livingColor;
    [SerializeField] private Color _itemColor;


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
            _screenLight.color = _itemColor;
            LivingMode = false;
        }
        else
        {
            _render.material = _Living;
            _screenLight.color = _livingColor;
            LivingMode = true;
        }
    }

    //Unsubscribe
    private void OnDisable()
    {
        _playerController.OnHickySwitch -= OnSwitchmode;
    }
}
