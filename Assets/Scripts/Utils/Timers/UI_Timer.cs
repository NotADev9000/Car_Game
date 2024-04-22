using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TimerUtils;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] protected Timer _timer;
    [SerializeField] protected TimerFormat _timerFormat = TimerFormat.S;

    protected TMP_Text _textField;
    private float _lastTimerValue;

    protected virtual void OnEnable()
    {
        _timer.OnTimerChanged += UpdateTimerText;
    }

    protected virtual void OnDisable()
    {
        _timer.OnTimerChanged -= UpdateTimerText;
    }

    private void Awake()
    {
        _textField = GetComponent<TMP_Text>();
    }

    protected virtual void UpdateTimerText()
    {
        if (_lastTimerValue != _timer.Count)
        {
            _lastTimerValue = _timer.Count;
            _textField.text = GetFormattedTimeFromSeconds(_timerFormat, _timer.Count);
        }
    }
}
