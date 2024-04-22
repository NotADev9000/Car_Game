using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TimerUtils;

public class FinishManager : MenuManager
{
    [Header("UI Settings")]
    [SerializeField] private GameObject _finishMenuContainer;

    [Header("UI Text Field Settings")]
    [SerializeField] private TMP_Text _scoreField;
    [SerializeField] private TMP_Text _highscoreField;
    [SerializeField] private string _scoreText = "Your Time: ";
    [SerializeField] private string _highscoreText = "Best Time: ";
    [SerializeField] private TimerFormat _scoreFormat = TimerFormat.MM_SS_MS;
    [SerializeField] private TimerFormat _highscoreFormat = TimerFormat.MM_SS_MS;

    // Events
    public static event Action OnRestartPressed;

    private void OnEnable()
    {
        TimerManager.EmitGameTimerCount += UpdateFields;
        GameManager.OnGameEnded += OnGameEnd;
        GameManager.OnGameReset += TurnOffFinished;
    }

    private void OnDisable()
    {
        TimerManager.EmitGameTimerCount -= UpdateFields;
        GameManager.OnGameEnded -= OnGameEnd;
        GameManager.OnGameReset -= TurnOffFinished;
    }

    protected override void Start()
    {
        base.Start();
        _finishMenuContainer.SetActive(false);
    }

    private void OnGameEnd()
    {
        TurnOnFinished();
    }

    //--------------------
    #region Finish State

    private void TurnOnFinished()
    {
        Time.timeScale = 0f;
        ChangeFinishUI(true);
        ChangeOverlayUI(true);
    }

    private void TurnOffFinished()
    {
        Time.timeScale = 1f;
        ChangeFinishUI(false);
        ChangeOverlayUI(false);
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

    private void UpdateFields(float gameTime)
    {
        _scoreField.SetText(_scoreText + GetFormattedTimeFromSeconds(_scoreFormat, gameTime));
        _highscoreField.SetText(_highscoreText + GetFormattedTimeFromSeconds(_highscoreFormat, PlayerPrefs.GetFloat("Highscore")));
    }

    public void OnRestartButtonPressedUI()
    {
        OnRestartPressed?.Invoke();
    }

    #endregion
    //--------------------
}
