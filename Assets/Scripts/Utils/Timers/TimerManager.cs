using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private Timer_Stopwatch _gameTimer;
    [SerializeField] private Timer_Countdown _introTimer;

    public static event Action OnIntroTimerFinished;

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

    private void StartTimer(Timer timer)
    {
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }
    }
}