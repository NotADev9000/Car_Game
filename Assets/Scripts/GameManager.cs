using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Events
    public static event Action OnIntroStarted;
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action OnGameReset;

    private void OnEnable()
    {
        TimerManager.OnIntroTimerFinished += StartGame;
        EndZone.OnEndZoneTriggered += EndGame;
        FinishManager.OnRestartPressed += ResetGame;
    }

    private void OnDisable()
    {
        TimerManager.OnIntroTimerFinished -= StartGame;
        EndZone.OnEndZoneTriggered -= EndGame;
        FinishManager.OnRestartPressed -= ResetGame;
    }

    private void Start()
    {
        OnIntroStarted?.Invoke();
    }

    private void StartGame()
    {
        OnGameStarted?.Invoke();
    }

    private void EndGame()
    {
        OnGameEnded?.Invoke();
    }

    private void ResetGame()
    {
        OnGameReset?.Invoke();
        OnIntroStarted?.Invoke();
    }
}
