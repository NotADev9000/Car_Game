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
        Void.OnVoidTriggered += TeleportPlayerToLastCheckpoint;
        PauseManager.OnRestartPressed += TeleportPlayerToLastCheckpoint;
    }

    private void OnDisable()
    {
        Void.OnVoidTriggered -= TeleportPlayerToLastCheckpoint;
        PauseManager.OnRestartPressed -= TeleportPlayerToLastCheckpoint;
    }

    private void TeleportPlayerToLastCheckpoint()
    {
        Player.ResetAllMovement();
        ResetPlayerTransform();
    }

    private void ResetPlayerTransform()
    {
        Player.transform.rotation = Quaternion.identity;
        Player.transform.position = CheckpointManager.CurrentCheckpoint;
    }
}
