using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private Timer _gameTimer;
    [SerializeField] private Timer _introTimer;

    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStart;
        GameManager.OnGameEnded += OnGameEnd;
        GameManager.OnGameReset += OnGameReset;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStart;
        GameManager.OnGameEnded -= OnGameEnd;
        GameManager.OnGameReset -= OnGameReset;
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

    private void StartTimer(Timer timer)
    {
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }
    }
}