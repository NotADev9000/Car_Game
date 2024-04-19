using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private Vector3 _defaultCheckpoint = Vector3.zero;

    public static Vector3 CurrentCheckpoint { get; private set; }

    private void OnEnable()
    {
        Checkpoint.OnCheckpointTriggered += OnCheckpointTriggered;
        GameManager.OnGameEnded += ResetCurrentCheckpoint;
    }

    private void OnDisable()
    {
        Checkpoint.OnCheckpointTriggered -= OnCheckpointTriggered;
        GameManager.OnGameEnded -= ResetCurrentCheckpoint;
    }

    private void Awake()
    {
        ResetCurrentCheckpoint();
    }

    private void ResetCurrentCheckpoint()
    {
        CurrentCheckpoint = _defaultCheckpoint;
    }

    private void OnCheckpointTriggered(Vector3 newRespawnPosition)
    {
        CurrentCheckpoint = newRespawnPosition;
    }
}
