using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputReader_UI _inputReader;

    [Header("UI Settings")]
    [SerializeField] private GameObject _pauseMenuContainer;
    [SerializeField] [Tooltip("UI Elements that should be hidden in WebGL builds")] private GameObject[] _NonWebGlUIElements;

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
        // hide pause menu on start
        _pauseMenuContainer.SetActive(false);
        HideElementsFromWebGL();
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
        if (_pauseMenuContainer != null)
        {
            _pauseMenuContainer.SetActive(isGamePaused);
        }
    }

    private void HideElementsFromWebGL()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            for (int i = 0; i < _NonWebGlUIElements.Length; i++)
            {
                _NonWebGlUIElements[i].SetActive(false);
            }
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

    public void OnQuitButtonPressedUI()
    {
        Application.Quit();
    }

    #endregion
    //--------------------
}
