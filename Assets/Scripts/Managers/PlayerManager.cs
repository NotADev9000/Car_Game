using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private VehicleController _player;
    public VehicleController Player { get => _player; }

    private void OnEnable()
    {
        Void.OnVoidTriggered += RestartAtLastCheckpoint;
        PauseManager.OnRestartPressed += RestartAtLastCheckpoint;
        GameManager.OnGameReset += RestartAtLastCheckpoint;
    }

    private void OnDisable()
    {
        Void.OnVoidTriggered -= RestartAtLastCheckpoint;
        PauseManager.OnRestartPressed -= RestartAtLastCheckpoint;
        GameManager.OnGameReset -= RestartAtLastCheckpoint;
    }

    private void RestartAtLastCheckpoint()
    {
        Player.ResetAllMovement();
        TeleportPlayerToLastCheckpoint();
    }

    private void TeleportPlayerToLastCheckpoint()
    {
        Player.transform.position = CheckpointManager.CurrentCheckpoint;
    }
}
