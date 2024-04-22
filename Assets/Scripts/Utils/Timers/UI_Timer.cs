using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Timer : MonoBehaviour
{
    public enum TimerFormat
    {
        S,
        MM_SS_MS
    }

    [SerializeField] protected Timer _timer;
    [SerializeField] private TimerFormat _timerFormat = TimerFormat.S;

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
            _textField.text = GetFormattedTimeFromSeconds(_timer.Count);
        }
    }

    protected string GetFormattedTimeFromSeconds(float seconds)
    {
        switch (_timerFormat)
        {
            case TimerFormat.S:
                return FormatSecondsToSecondsDisplay(seconds);
            case TimerFormat.MM_SS_MS:
                return FormatSecondsToFullDisplay(seconds);
            default:
                return seconds.ToString();
        }
    }

    private string FormatSecondsToFullDisplay(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }

    private string FormatSecondsToSecondsDisplay(float seconds)
    {
        return Mathf.CeilToInt(seconds).ToString();
    }
}
