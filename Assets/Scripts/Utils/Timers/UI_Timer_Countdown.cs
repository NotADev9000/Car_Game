using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Timer_Countdown : UI_Timer
{
    [SerializeField] private string zeroText;

    private Timer_Countdown Timer { get => (Timer_Countdown) _timer; }

    protected override void OnEnable()
    {
        base.OnEnable();
        Timer.OnCountdownStarted += ResetText;
        Timer.OnCountdownEnded += EndCountdown;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Timer.OnCountdownStarted -= ResetText;
        Timer.OnCountdownEnded -= EndCountdown;
    }

    private void ResetText()
    {
        _textField.text = GetFormattedTimeFromSeconds(_timer.Count);
        FadeText(1f, 0f);
    }

    private void EndCountdown()
    {
        DisplayZeroText();
        FadeText(0f, 1f);
    }

    private void DisplayZeroText()
    {
        _textField.text = zeroText;
    }

    private void FadeText(float alpha, float time)
    {
        _textField.CrossFadeAlpha(alpha, time, false);
    }
}
