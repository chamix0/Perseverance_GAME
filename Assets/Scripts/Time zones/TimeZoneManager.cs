using System.Diagnostics;
using UnityEngine;

public class TimeZoneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Stopwatch _stopwatch;
    private JSONsaving _jsoNsaving;
    private PlayerValues _playerValues;
    private GameData _gameData;
    public int zone;
    [SerializeField] private GameObject trigger;
    private LoadScreen loadScreen;

    void Start()
    {
        loadScreen = FindObjectOfType<LoadScreen>();
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _stopwatch = new Stopwatch();
        _playerValues = FindObjectOfType<PlayerValues>();
        _gameData = _playerValues.gameData;

        if (!_playerValues.gameData.checkEnabled(zone))
            trigger.SetActive(false);
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
        if (zone == 4)
            loadScreen.LoadCredits();
        else
            loadScreen.LoadLevels();
    }
}