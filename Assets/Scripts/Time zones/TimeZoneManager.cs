using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeZoneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Stopwatch _stopwatch;
    private JSONsaving _jsoNsaving;
    private PlayerValues _playerValues;
    private GameData _gameData;
    public int zone;

    void Start()
    {
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _stopwatch = new Stopwatch();
        _playerValues = FindObjectOfType<PlayerValues>();
        _gameData = _playerValues.gameData;
    }

    public void StartRun()
    {
        _stopwatch.Start();
    }

    public void EndRun()
    {
        _stopwatch.Stop();
        _gameData.setTime(zone, _stopwatch.Elapsed.TotalMilliseconds, MyUtils.GetTimeString(_stopwatch));
        _jsoNsaving.SaveTheData();
        SceneManager.LoadScene(1);
    }
}