using System;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private string runName;
    [SerializeField] private float masterVolume, vfxVolume, musicVolume, uiVolume;
    [SerializeField] private float tiltSens;
    [SerializeField] private bool GameStarted;
    [SerializeField] private int eddoModel;

    [SerializeField] private bool[] zonesEnabled;
    [SerializeField] private double[] zonestime;
    [SerializeField] private string lastTimePlayed;
    [SerializeField] private int totalTimePlayed;
    [SerializeField] private int maxShootingRangeScore;
    [SerializeField] private string[] zonesPB;
    [SerializeField] private bool newGame;
    private const int MAX_ZONES = 5;

    public GameData(int model, string name)
    {
        masterVolume = 0.5f;
        vfxVolume = 0.5f;
        musicVolume = 0.5f;
        uiVolume = 0.5f;
        runName = name;
        tiltSens = 75;
        eddoModel = model;
        totalTimePlayed = 0;
        maxShootingRangeScore = 0;
        lastTimePlayed = DateTime.Now.ToString();
        GameStarted = true;
        newGame = true;
        zonesEnabled = new bool[MAX_ZONES];
        zonestime = new Double[MAX_ZONES];
        zonesPB = new string[MAX_ZONES];
        intializeLevels();
    }

    public GameData()
    {
        masterVolume = 0.5f;
        vfxVolume = 0.5f;
        musicVolume = 0.5f;
        uiVolume = 0.5f;
        runName = "------";
        tiltSens = 75;
        eddoModel = 1;
        maxShootingRangeScore = 0;
        GameStarted = false;
        newGame = true;
        zonesEnabled = new bool[MAX_ZONES];
        zonestime = new Double[MAX_ZONES];
        zonesPB = new string[MAX_ZONES];
        intializeLevels();
    }

    public int GetEddoModel()
    {
        return eddoModel;
    }

    #region Sound

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
    }

    public void SetVfxVolume(float value)
    {
        vfxVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public void SetUiVolume(float value)
    {
        uiVolume = value;
    }

    public float GetUiVolume()
    {
        return uiVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetVfxVolume()
    {
        return vfxVolume;
    }

    #endregion

    public bool GetIsNewGame()
    {
        return newGame;
    }

    public void SetNewGame()
    {
        newGame = false;
    }


    public int GetMaxShootingScore()
    {
        return maxShootingRangeScore;
    }

    public void SetMaxShootingScore(int score)
    {
        maxShootingRangeScore = Mathf.Max(score, maxShootingRangeScore);
    }

    private void intializeLevels()
    {
        // zonesEnabled[0] = true;
        // zonestime[0] = -1;
        // zonesPB[0] = "--:--.--";

        for (int i = 0; i < MAX_ZONES; i++)
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

        enableNextLevel(level);
    }

    public double GetZoneTime(int zone)
    {
        return zonestime[zone];
    }

    public void enableNextLevel(int levelIndex)
    {
        zonesEnabled[(levelIndex + 1) % zonesEnabled.Length] = true;
    }

    public void enableLevel(int levelIndex)
    {
        zonesEnabled[levelIndex] = true;
    }

    public bool checkEnabled(int index)
    {
        return zonesEnabled[index];
    }

    public string getPBTime(int index)
    {
        return zonesPB[index];
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
}