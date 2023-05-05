using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private PlayerValues playerValues;
    private Stopwatch timer;
    [SerializeField] private int timeLimit = 60;
    [SerializeField] private Transform respawnPos;
    bool ended, exited = true;

    private void Awake()
    {
        timer = new Stopwatch();
    }

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.Elapsed.TotalSeconds > timeLimit)
        {
            if (!ended)
            {
                ended = true;
                timer.Stop();
                EndRace();
            }
        }
    }

    public string GetRemainingTime()
    {
        double aux = timeLimit * 1000 - timer.Elapsed.TotalMilliseconds;
        return MyUtils.GetCountdownTimeString(aux);
    }

    public void StartRace()
    {
        //countdown or something
        if (exited)
        {
            timer.Restart();
            exited = false;
        }
    }

    public void ExitRace()
    {
        exited = true;
        timer.Stop();
        timer.Reset();
    }

    public void EndRace()
    {
        if (timer.Elapsed.TotalSeconds > timeLimit)
        {
            StartOver();
        }
        else
        {
            //congratulations or something
            print("congratulations");
        }

        exited = true;
        ended = false;
        timer.Reset();
    }

    private void StartOver()
    {
        playerValues.Die(respawnPos.position);
    }
}