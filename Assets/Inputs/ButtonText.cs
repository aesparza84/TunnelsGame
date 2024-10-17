using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Button Text")]
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _buttonText.color = _selectedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _buttonText.color = _defaultColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _buttonText.color = _selectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _buttonText.color = _defaultColor;
    }
}
