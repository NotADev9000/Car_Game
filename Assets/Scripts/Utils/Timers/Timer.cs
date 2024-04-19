using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float Count { get; private set; }
    private bool _isRunning;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (_isRunning)
        {
            Count += Time.deltaTime;
        }
    }

    public void ResetTimer()
    {
        Count = 0f;
        _isRunning = false;
    }

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
        ResetTimer();
    }
}
