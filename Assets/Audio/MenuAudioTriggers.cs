using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuAudioTriggers : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public static event EventHandler OnNewOption;
    public static event EventHandler OnConfirm;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnNewOption?.Invoke(this, EventArgs.Empty);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnConfirm?.Invoke(this, EventArgs.Empty);
    }
}
