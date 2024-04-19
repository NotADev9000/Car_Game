using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer_Stopwatch : Timer
{
    protected override void Update()
    {
        if (_isRunning) Count += Time.deltaTime;
    }

    public override void ResetTimer()
    {
        Count = 0f;
    }
}
