using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] private Timer _timer;

    private TMP_Text _textField;
    private float _lastTimerValue;

    private void Awake()
    {
        _textField = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_lastTimerValue != _timer.Count)
        {
            _textField.text = FormatSecondsToFullDisplay();
        }
    }

    private string FormatSecondsToFullDisplay()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_timer.Count);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }
}
