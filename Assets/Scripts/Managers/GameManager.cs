using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Events
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action OnGameReset;

    // State
    public static bool IsInputAllowed { get; private set; } = true;

    private void OnEnable()
    {
        EndZone.OnEndZoneTriggered += EndGame;
        FinishManager.OnRestartPressed += ResetGame;
    }

    private void OnDisable()
    {
        EndZone.OnEndZoneTriggered -= EndGame;
        FinishManager.OnRestartPressed -= ResetGame;
    }

    private void Start()
    {
        OnGameStarted?.Invoke();
    }

    private void EndGame()
    {
        IsInputAllowed = false;
        OnGameEnded?.Invoke();
    }

    private void ResetGame()
    {
        OnGameReset?.Invoke();
        IsInputAllowed = true;
        OnGameStarted?.Invoke();
    }
}
