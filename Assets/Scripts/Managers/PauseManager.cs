using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputReader_UI _inputReader;

    [Header("UI Settings")]
    [SerializeField] private GameObject pauseMenuContainer;

    // Pause State
    private bool _isGamePaused = false;

    // Events
    public static event Action<bool> OnPauseChange;
    public static event Action OnRestartPressed;

    private void OnEnable()
    {
        _inputReader.PauseEvent += OnPauseInputPressed;
    }

    private void OnDisable()
    {
        _inputReader.PauseEvent -= OnPauseInputPressed;
    }

    private void Start()
    {
        pauseMenuContainer.SetActive(false);
    }

    //--------------------
    #region Pause State

    private void OnPauseInputPressed(bool startedPress)
    {
        if (!startedPress) return;

        ChangePause();
    }

    private void ChangePause()
    {
        TogglePauseState();

        OnPauseChange?.Invoke(_isGamePaused);
        TogglePauseUI(_isGamePaused);
        Time.timeScale = _isGamePaused ? 0f : 1f;
    }

    private void TogglePauseState()
    {
        _isGamePaused = !_isGamePaused;
    }

    #endregion
    //--------------------

    //--------------------
    #region UI

    private void TogglePauseUI(bool isGamePaused)
    {
        if (pauseMenuContainer != null)
        {
            pauseMenuContainer.SetActive(isGamePaused);
        }
    }

    public void OnResumeButtonPressedUI()
    {
        ChangePause();
    }

    public void OnRestartButtonPressedUI()
    {
        OnRestartPressed?.Invoke();
        ChangePause();
    }

    #endregion
    //--------------------
}
