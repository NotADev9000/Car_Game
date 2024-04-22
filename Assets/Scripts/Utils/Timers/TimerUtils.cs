using System;
using UnityEngine;

public static class TimerUtils
{
    public enum TimerFormat
    {
        S,
        MM_SS_MS
    }

    public static string GetFormattedTimeFromSeconds(TimerFormat timerFormat, float seconds)
    {
        switch (timerFormat)
        {
            case TimerFormat.S:
                return FormatSecondsToSecondsDisplay(seconds);
            case TimerFormat.MM_SS_MS:
                return FormatSecondsToFullDisplay(seconds);
            default:
                return seconds.ToString();
        }
    }

    public static string FormatSecondsToFullDisplay(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }

    public static string FormatSecondsToSecondsDisplay(float seconds)
    {
        return Mathf.CeilToInt(seconds).ToString();
    }
}
