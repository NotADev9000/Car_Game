using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public float Count { get; protected set; }
    protected bool _isRunning;

    private void Awake()
    {
        StopTimer();
    }

    protected abstract void Update();

    public abstract void ResetTimer();

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void StopTimer()
    {
        PauseTimer();
        ResetTimer();
    }
}
