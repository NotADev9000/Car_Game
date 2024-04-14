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
    }

    private void OnDisable()
    {
        Checkpoint.OnCheckpointTriggered -= OnCheckpointTriggered;
    }

    private void Awake()
    {
        ResetCurrentCheckpoint();
    }

    private void OnCheckpointTriggered(Vector3 newRespawnPosition)
    {
        CurrentCheckpoint = newRespawnPosition;
    }

    private void ResetCurrentCheckpoint()
    {
        CurrentCheckpoint = _defaultCheckpoint;
    }
}
