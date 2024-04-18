using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _count;
    private bool _isRunning;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (_isRunning)
        {
            _count += Time.deltaTime;

            // Convert timerValue to TimeSpan
            TimeSpan timeSpan = TimeSpan.FromSeconds(_count);

            // Format the TimeSpan to display in mm:ss:msms format
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

            // Display the formatted time
            Debug.Log(formattedTime);
        }
    }

    public void ResetTimer()
    {
        _count = 0f;
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
