﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For sprites & canvases
/// </summary>
public class Script_GraphicsManager : MonoBehaviour
{
    static private float fadeFastTime = 0.25f;
    static private float fadeMedTime = 0.75f;
    static private float fadeSlowTime = 1.25f;

    public static float GetFadeTime(FadeSpeeds fadeSpeed)
    {
        float fadeTime;
        switch (fadeSpeed)
        {
            case FadeSpeeds.Fast:
                fadeTime = fadeFastTime;
                break;
            case FadeSpeeds.Med:
                fadeTime = fadeMedTime;
                break;
            case FadeSpeeds.Slow:
                fadeTime = fadeSlowTime;
                break;
            default:
                fadeTime = 0f;
                break;
        }
        return fadeTime;
    }
}
