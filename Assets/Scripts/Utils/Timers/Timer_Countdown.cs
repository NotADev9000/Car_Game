using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer_Countdown : Timer
{
    [SerializeField] private float _startTime = 0f;

    public event Action OnCountdownStarted;
    public event Action OnCountdownEnded;

    protected override void Update()
    {
        if (IsRunning)
        {
            Count -= Time.deltaTime;

            if (Count <= 0f)
            {
                Count = 0f;
                OnCountdownEnded?.Invoke();
                PauseTimer();
            }
            else
            {
                InvokeTimerChange();
            }
        }
    }

    public override void StartTimer()
    {
        OnCountdownStarted?.Invoke();
        base.StartTimer();
    }

    public override void ResetTimer()
    {
        Count = _startTime;
    }
}
