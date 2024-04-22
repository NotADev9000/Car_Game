using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private Timer_Stopwatch _gameTimer;
    [SerializeField] private Timer_Countdown _introTimer;

    public static event Action OnIntroTimerFinished;
    public static event Action<float> EmitGameTimerCount;

    private void OnEnable()
    {
        GameManager.OnIntroStarted += OnIntroStart;
        GameManager.OnGameStarted += OnGameStart;
        GameManager.OnGameEnded += OnGameEnd;
        GameManager.OnGameReset += OnGameReset;
        _introTimer.OnCountdownEnded += OnIntroCountdownFinished;
    }

    private void OnDisable()
    {
        GameManager.OnIntroStarted -= OnIntroStart;
        GameManager.OnGameStarted -= OnGameStart;
        GameManager.OnGameEnded -= OnGameEnd;
        GameManager.OnGameReset -= OnGameReset;
        _introTimer.OnCountdownEnded -= OnIntroCountdownFinished;
    }

    //--------------------
    #region Game State Events

    private void OnIntroStart()
    {
        StartTimer(_introTimer);
    }

    private void OnGameStart()
    {
        StartTimer(_gameTimer);
    }

    private void OnGameEnd()
    {
        _gameTimer.PauseTimer();
        TrySaveHighscore();
        EmitGameTimerCount?.Invoke(_gameTimer.Count);
    }

    private void OnGameReset()
    {
        _gameTimer.ResetTimer();
        _introTimer.ResetTimer();
    }

    private void OnIntroCountdownFinished()
    {
        OnIntroTimerFinished?.Invoke();
    }

    #endregion
    //--------------------

    //--------------------
    #region Timer Management

    private void StartTimer(Timer timer)
    {
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }
    }

    #endregion
    //--------------------

    //--------------------
    #region Highscore

    private void TrySaveHighscore()
    {
        if (IsHighscore()) SaveHighscore();
    }

    private bool IsHighscore()
    {
        float highscore = PlayerPrefs.GetFloat("Highscore");
        return _gameTimer.Count < highscore || highscore == 0;
    }

    private void SaveHighscore()
    {
        PlayerPrefs.SetFloat("Highscore", _gameTimer.Count);
    }

    #endregion
    //--------------------

}