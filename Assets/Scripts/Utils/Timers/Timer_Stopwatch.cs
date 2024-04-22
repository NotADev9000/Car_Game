using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer_Stopwatch : Timer
{
    protected override void Update()
    {
        if (IsRunning)
        {
            Count += Time.deltaTime;
            InvokeTimerChange();
        }
    }

    public override void ResetTimer()
    {
        Count = 0f;
        InvokeTimerChange();
    }
}
