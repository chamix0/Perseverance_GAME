using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private string runName;
    [SerializeField] private float volume;
    [SerializeField] private bool muted;
    [SerializeField] private float tiltSens;
    [SerializeField] private bool GameStarted;
    [SerializeField] private int eddoModel;

    [SerializeField] private bool[] zonesEnabled;
    [SerializeField] private double[] zonestime;
    [SerializeField] private string lastTimePlayed;
    [SerializeField] private int totalTimePlayed;
    [SerializeField] private string[] zonesPB;

    private const int MAX_ZONES = 5;

    public GameData(int model, string name)
    {
        volume = 1;
        runName = name;
        tiltSens = 75;
        eddoModel = model;
        totalTimePlayed = 0;
        lastTimePlayed = DateTime.Now.ToString();
        muted = false;
        GameStarted = true;
        zonesEnabled = new bool[MAX_ZONES];
        zonestime = new Double[MAX_ZONES];
        zonesPB = new string[MAX_ZONES];
        intializeLevels();
    }

    public GameData()
    {
        volume = 1;
        runName = "------";
        tiltSens = 75;
        eddoModel = 1;
        muted = false;
        GameStarted = false;
        zonesEnabled = new bool[MAX_ZONES];
        zonestime = new Double[MAX_ZONES];
        zonesPB = new string[MAX_ZONES];
        intializeLevels();
    }

    public int GetEddoModel()
    {
        return eddoModel;
    }

    public bool getMuted()
    {
        return muted;
    }

    public void setMuted(bool mute)
    {
        muted = mute;
    }

    private void intializeLevels()
    {
        zonesEnabled[0] = true;
        zonestime[0] = -1;
        zonesPB[0] = "--:--.--";

        for (int i = 1; i < MAX_ZONES; i++)
        {
            zonesEnabled[i] = false;
            zonestime[i] = -1;
            zonesPB[i] = "--:--.--";
        }
    }

    public void setTime(int level, double timeMiliseconds, string timeString)
    {
        if (zonestime[level] < 0)
        {
            zonestime[level] = timeMiliseconds;
            zonesPB[level] = timeString;
        }
        else
        {
            if (zonestime[level] > timeMiliseconds)
            {
                zonestime[level] = timeMiliseconds;
                zonesPB[level] = timeString;
            }
        }
    }

    public void updateLevel(int level, long time, string timeS, int coins)
    {
        setTime(level, time, timeS);
    }

    public void enableNextLevel(int levelIndex)
    {
        zonesEnabled[levelIndex + 1] = true;
    }

    public bool checkEnabled(int index)
    {
        return zonesEnabled[index];
    }

    public string getPBTime(int index)
    {
        return zonesPB[index];
    }


    public void setVolume(float vol)
    {
        volume = vol;
    }

    public float getVolume()
    {
        return volume;
    }

    public bool GetGameStarted()
    {
        return GameStarted;
    }

    public void StartGame()
    {
        GameStarted = true;
    }

    public string getTotalTime()
    {
        int hours = totalTimePlayed / 60;
        int minutes = totalTimePlayed % 60;
        return hours + "H " + minutes + "m";
    }

    public string GetName()
    {
        return runName;
    }

    public string GetLastTimePlayed()
    {
        return lastTimePlayed;
    }

    public override string ToString()
    {
        return $"{volume}{tiltSens}";
    }
}