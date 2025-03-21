using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Menu UI Objects")]
    [SerializeField] private GameObject MenuButtonsObj;
    [SerializeField] private GameObject DefaultOption;

    //Input actions
    private PlayerControls _mappedInputs;

    //Menu inputs
    private InputAction _pause;
    private bool IsPaused;
    private bool DeathMode;

    public static event EventHandler OnPause;
    public static event EventHandler OnResume;
    public static event EventHandler OnLeaveGame;

    private void Start()
    {
        LevelMessanger.PlayerReset += GameReset;
        LevelMessanger.LevelStart += AllowMenu;
        LevelMessanger.LevelFinished += DisableMenu;
        RatKillEvent.KilledAnimFinished += DeathMenu;
    }

    private void DeathMenu(object sender, EventArgs e)
    {
        DeathMode = true;
        _pause.Disable();
        MenuButtonsObj.SetActive(true);
        EventSystem.current.SetSelectedGameObject(DefaultOption);
    }

    private void AllowMenu(object sender, System.EventArgs e)
    {
        _pause.Enable();
    }

    private void OnEnable()
    {
        if (_mappedInputs == null)
        {
            _mappedInputs = new PlayerControls();
        }

        _pause = _mappedInputs.Menu.PauseAndPlay;
        _pause.Enable();
        _pause.started += OnPauseToggle;
    }

    private void GameReset(object sender, System.EventArgs e)
    {
        if (DeathMode)
        {
            DeathMode = false;
        }
        Time.timeScale = 1.0f;
    }
    private void DisableMenu(object sender, System.EventArgs e)
    {
        _pause.Disable();
    }
    private void OnPauseToggle(InputAction.CallbackContext obj)
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 0.0f;
        MenuButtonsObj.SetActive(true);
        EventSystem.current.SetSelectedGameObject(DefaultOption);
    }

    public void ResumeGame()
    {
        if (DeathMode)
        {
            return;
        }

        OnResume?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 1.0f;
        MenuButtonsObj.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void RestartGame()
    {
        //Reload this scene
        SceneManager.LoadScene(1);
    }

    public void ExitToMainMenu()
    {
        //Load Main Menu Scene
        OnLeaveGame?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 1;
    }

    private void OnDisable()
    {
        _pause.Disable();
        _pause.started -= OnPauseToggle;

        LevelMessanger.LevelStart -= GameReset;
        LevelMessanger.LevelFinished -= DisableMenu;
        RatKillEvent.KilledAnimFinished -= AllowMenu;
        RatKillEvent.KilledAnimFinished -= DeathMenu;
    }
}
