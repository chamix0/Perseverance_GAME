using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class PlayerLives : MonoBehaviour
{
    // Start is called before the first frame update
    public float cooldown = 5;
    private Stopwatch timer;
    private PlayerValues playerValues;

    private void Awake()
    {
        timer = new Stopwatch();
        timer.Start();
        playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerValues.GetPaused() && timer.IsRunning)
            timer.Stop();
        else if (!playerValues.GetPaused() && !timer.IsRunning)
            timer.Start();

        if (timer.Elapsed.TotalSeconds > cooldown)
        {
            timer.Restart();
            playerValues.AddLive();
            playerValues.NotifyCameraLives();
        }
    }

    public void Damage()
    {
        timer.Restart();
    }
}