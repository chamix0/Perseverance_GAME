using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    [SerializeField] private float volume;
    [SerializeField] private bool muted;
    [SerializeField] private float tiltSens;

    // [SerializeField] private Level[] _levels;
    [SerializeField] private bool[] zonesEnabled;
    [SerializeField] private long[] zonestime;
    [SerializeField] private string[] zonesPB;
    private const int MAX_ZONES = 5;

    public GameData()
    {
        volume = 1;
        tiltSens = 75;
        muted = false;
        zonesEnabled = new bool[MAX_ZONES];
        zonestime = new long[MAX_ZONES];
        zonesPB = new string[MAX_ZONES];
        intializeLevels();
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

        for (int i = 1; i < 10; i++)
        {
            zonesEnabled[i] = false;
            zonestime[i] = -1;
            zonesPB[i] = "--:--.--";
        }
    }

    public void setTime(int level, long time, string timeString)
    {
        if (zonestime[level] < 0)
        {
            zonestime[level] = time;
            zonesPB[level] = timeString;
        }
        else
        {
            if (zonestime[level] > time)
            {
                zonestime[level] = time;
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

    public override string ToString()
    {
        return $"{volume}{tiltSens}";
    }
}