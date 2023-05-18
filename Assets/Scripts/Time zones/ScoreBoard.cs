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
    [SerializeField] private TimeZoneManager _zoneManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        scoreboard = GetComponentInChildren<TMP_Text>();
        scoreboard.text = "PB " + _playerValues.gameData.getPBTime(_zoneManager.zone);
    }
}