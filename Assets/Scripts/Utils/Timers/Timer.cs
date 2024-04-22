using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public float Count { get; protected set; }
    public bool IsRunning { get; protected set; }

    public event Action OnTimerChanged;

    private void Awake()
    {
        StopTimer();
    }

    protected abstract void Update();

    public abstract void ResetTimer();

    public virtual void StartTimer()
    {
        IsRunning = true;
    }

    public void PauseTimer()
    {
        IsRunning = false;
    }

    public void StopTimer()
    {
        PauseTimer();
        ResetTimer();
    }

    protected void InvokeTimerChange()
    {
        OnTimerChanged?.Invoke();
    }
}
