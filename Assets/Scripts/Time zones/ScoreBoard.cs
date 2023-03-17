using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[DefaultExecutionOrder(4)]
public class ScoreBoard : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private TMP_Text scoreboard;
    private TimeZoneManager _zoneManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _zoneManager = transform.parent.GetComponent<TimeZoneManager>();
        scoreboard = GetComponent<TMP_Text>();
        scoreboard.text = "PB " + _playerValues.gameData.getPBTime(_zoneManager.zone);
    }
}