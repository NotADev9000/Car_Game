using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishManager : MenuManager
{
    [Header("UI Settings")]
    [SerializeField] private GameObject _finishMenuContainer;

    // Finish State
    private bool _isGameFinished = false;

    // Events
    public static event Action OnRestartPressed;

    private void OnEnable()
    {
        GameManager.OnGameEnded += TurnOnFinished;
        GameManager.OnGameReset += TurnOffFinished;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnded -= TurnOnFinished;
        GameManager.OnGameReset -= TurnOffFinished;
    }

    protected override void Start()
    {
        base.Start();
        _finishMenuContainer.SetActive(false);
    }

    //--------------------
    #region Finish State

    private void TurnOnFinished()
    {
        ChangeFinishState(true);
        Time.timeScale = 0f;
        ChangeFinishUI(true);
        ChangeOverlayUI(true);
    }

    private void TurnOffFinished()
    {
        ChangeFinishState(false);
        Time.timeScale = 1f;
        ChangeFinishUI(false);
        ChangeOverlayUI(false);
    }

    private void ChangeFinishState(bool state)
    {
        _isGameFinished = state;
    }

    #endregion
    //--------------------

    //--------------------
    #region UI

    private void ChangeFinishUI(bool isGameFinished)
    {
        if (_finishMenuContainer != null)
        {
            _finishMenuContainer.SetActive(isGameFinished);
        }
    }

    public void OnRestartButtonPressedUI()
    {
        OnRestartPressed?.Invoke();
    }

    #endregion
    //--------------------
}
