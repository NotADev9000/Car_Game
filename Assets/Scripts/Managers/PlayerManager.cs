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
        VoidContact.OnVoidTriggered += OnVoidTriggered;
    }

    private void OnDisable()
    {
        VoidContact.OnVoidTriggered -= OnVoidTriggered;
    }

    private void OnVoidTriggered()
    {
        TeleportPlayerToLastCheckpoint();
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
