using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MyUtils 
{
    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)

    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }

    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
            result += 360f;

        return result;
    }
    public static string GetTimeString(Stopwatch stopwatch)
    {
        TimeSpan ts = stopwatch.Elapsed;
        return String.Format(String.Format("{0:00}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10));
    }
}
