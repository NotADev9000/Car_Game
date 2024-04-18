using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Events
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;

    private void OnEnable()
    {
        EndZone.OnEndZoneTriggered += EndGame;
    }

    private void OnDisable()
    {
        EndZone.OnEndZoneTriggered -= EndGame;
    }

    private void Start()
    {
        OnGameStarted?.Invoke();
    }

    private void EndGame()
    {
        OnGameEnded?.Invoke();
    }
}
