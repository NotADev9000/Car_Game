using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MenuManager
{
    [Header("Input Settings")]
    [SerializeField] private InputReader_UI _inputReader;

    [Header("UI Settings")]
    [SerializeField] private GameObject _pauseMenuContainer;

    // Pause State
    private bool _isGamePaused = false;

    // Events
    public static event Action OnRestartPressed;

    private void OnEnable()
    {
        _inputReader.PauseEvent += OnPauseInputPressed;
        GameManager.OnGameEnded += TurnOffPause;
    }

    private void OnDisable()
    {
        _inputReader.PauseEvent -= OnPauseInputPressed;
        GameManager.OnGameEnded -= TurnOffPause;
    }
     
    protected override void Start()
    {
        base.Start();
        _pauseMenuContainer.SetActive(false);
    }

    //--------------------
    #region Pause State

    private void OnPauseInputPressed(bool startedPress)
    {
        if (!startedPress) return;

        TogglePause();
    }

    private void TogglePause()
    {
        ChangePauseState(!_isGamePaused);
        Time.timeScale = _isGamePaused ? 0f : 1f;
        ChangePauseUI(_isGamePaused);
        ChangeOverlayUI(_isGamePaused);
    }

    private void TurnOffPause()
    {
        ChangePauseState(false); 
        ChangePauseUI(false);
    }

    private void ChangePauseState(bool state)
    {
        _isGamePaused = state;
    }


    #endregion
    //--------------------

    //--------------------
    #region UI

    private void ChangePauseUI(bool isGamePaused)
    {
        if (_pauseMenuContainer != null)
        {
            _pauseMenuContainer.SetActive(isGamePaused);
        }
    }

    public void OnResumeButtonPressedUI()
    {
        TogglePause();
    }

    public void OnRestartButtonPressedUI()
    {
        OnRestartPressed?.Invoke();
        TogglePause();
    }

    #endregion
    //--------------------
}
