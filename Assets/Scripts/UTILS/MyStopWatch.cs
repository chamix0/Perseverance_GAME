using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class MyStopWatch : MonoBehaviour
{
    private float elapsed = 0;

    private bool stoped;

    // Update is called once per frame
    void Update()
    {
        if (!stoped)
            elapsed += Time.deltaTime;
    }

    public void Stop()
    {
        stoped = true;
    }

    public void StartStopwatch()
    {
        stoped = false;
    }

    public void ResetStopwatch()
    {
        elapsed = 0;
    }

    public void Restart()
    {
        ResetStopwatch();
        StartStopwatch();
    }

    public bool IsRunning()
    {
        return !stoped;
    }

    public int GetElapsedMiliseconds()
    {
        return (int)elapsed * 1000;
    }

    public float GetElapsedSeconds()
    {
        return elapsed;
    }
}