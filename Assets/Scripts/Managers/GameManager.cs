using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputReader_UI _inputReader;

    // Game State
    private bool _isGamePaused = false;

    private void OnEnable()
    {
        _inputReader.PauseEvent += OnPausePressed;
    }

    private void OnDisable()
    {
        _inputReader.PauseEvent -= OnPausePressed;
    }

    private void OnPausePressed(bool startedPress)
    {
        if (!startedPress) return;

        TogglePause();
    }

    private void TogglePause()
    {
        _isGamePaused = !_isGamePaused;
        Time.timeScale = _isGamePaused ? 0f : 1f;
    }
}
