using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    private PlayerControls _mappedInputs;
    private InputAction toggleScreen;
    private InputAction backPage;
    private bool Activated;


    [Header("UI Groups")]
    [SerializeField] private GameObject TopUILayer;
    [SerializeField] private GameObject SettingsUIGroup;

    public static event EventHandler OnMainGameEnter;

    public UnityEvent OnTvSwithced;

    private void Start()
    {
        Activated = false;
        TopUILayer.SetActive(false);
    }

    private void OnEnable()
    {
        if (_mappedInputs == null)
            _mappedInputs = new PlayerControls();

        toggleScreen = _mappedInputs.MainMenu.StartMenu;
        toggleScreen.Enable();
        toggleScreen.started += OnScreenActivate;

        backPage = _mappedInputs.MainMenu.BackPage;
        backPage.Enable();
        backPage.started += GoBack;
    }

    private void GoBack(InputAction.CallbackContext obj)
    {
        if (SettingsUIGroup.activeInHierarchy)
        {
            LeaveSettings();
        }
    }

    public void CallTransitionEvent()
    {
        OnMainGameEnter?.Invoke(this, EventArgs.Empty);
    }

    public void EnterSettings()
    {
        SettingsUIGroup.SetActive(true);
        TopUILayer.SetActive(false);
    }

    public void LeaveSettings()
    {
        SettingsUIGroup.SetActive(false);
        TopUILayer.SetActive(true);
    }

    private void OnDisable()
    {
        toggleScreen.Disable();
        toggleScreen.started -= OnScreenActivate;

        backPage.Disable();
        backPage.started -= GoBack;
    }

    private void OnScreenActivate(InputAction.CallbackContext obj)
    {
        if (!Activated)
        {
            OnTvSwithced?.Invoke();
            TopUILayer.SetActive(true);
            Activated = true;
            toggleScreen.Disable();
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
