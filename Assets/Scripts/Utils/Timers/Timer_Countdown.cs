using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer_Countdown : Timer
{
    [SerializeField] private float _startTime = 0f;

    public event Action OnCountdownEnded;

    protected override void Update()
    {
        if (_isRunning)
        {
            Count -= Time.deltaTime;
            //Debug.Log(Count);

            if (Count <= 0f)
            {
                PauseTimer();
                Count = 0f;
                OnCountdownEnded?.Invoke();
            }
        }
    }

    public override void ResetTimer()
    {
        Count = _startTime;
    }
}
