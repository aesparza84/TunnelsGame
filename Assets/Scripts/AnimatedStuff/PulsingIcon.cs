using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingIcon : MonoBehaviour
{
    [Header("Icon Blink")]
    [SerializeField] private float BlinkSpeed;

    //Icon material
    private MeshRenderer _render;

    //Recolor
    private Color tempColor;
    private void Start()
    {
        _render = GetComponent<MeshRenderer>();
        tempColor = _render.material.color;
    }

    private void Update()
    {
        if (_render != null)
        {
            tempColor.a =  Mathf.Sin(BlinkSpeed * Time.time);
            _render.material.color = tempColor;
        }
    }
}
