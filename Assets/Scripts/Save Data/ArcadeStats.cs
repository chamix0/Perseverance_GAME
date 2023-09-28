using System;
using UnityEngine;


[Serializable]
public class ArcadeStats
{
    [SerializeField] private string _totalPoints;
    [SerializeField] private int _rounds;
    [SerializeField] private int _level;
    [SerializeField] private int _enemiesKilled;
    [SerializeField] private string _zonesUnlocked;
    [SerializeField] private string _unlockedGears;
    [SerializeField] private string _powerEnabled;

    public ArcadeStats(string points, int rounds, int level, int enemiesKilled, string zonesUnlocked, string unlockedGears,
        string powerEnabled)
    {
        _totalPoints = points;
        this._rounds = rounds;
        this._level = level;
        this._enemiesKilled = enemiesKilled;
        this._zonesUnlocked = zonesUnlocked;
        this._unlockedGears = unlockedGears;
        this._powerEnabled = powerEnabled;
    }

    public string TotalPoints => _totalPoints;

    public int Rounds => _rounds;

    public int Level => _level;

    public int EnemiesKilled => _enemiesKilled;

    public string ZonesUnlocked => _zonesUnlocked;

    public string UnlockedGears => _unlockedGears;

    public string PowerEnabled => _powerEnabled;
}