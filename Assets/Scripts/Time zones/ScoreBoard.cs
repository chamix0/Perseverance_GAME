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

        UpdateText();
    }

    public void UpdateText()
    {
        if (_playerValues.gameData.checkEnabled(_zoneManager.zone) &&
            (int)_playerValues.gameData.GetZoneTime(_zoneManager.zone) == -1)
        {
            scoreboard.text = "ACCESS GRANTED";
        }
        else if (_playerValues.gameData.checkEnabled(_zoneManager.zone) &&
                 (int)_playerValues.gameData.GetZoneTime(_zoneManager.zone) != -1)
        {
            scoreboard.text = "PB " + _playerValues.gameData.getPBTime(_zoneManager.zone);
        }
        else if (!_playerValues.gameData.checkEnabled(_zoneManager.zone))
        {
            scoreboard.text = "ACCESS DENIED";
        }
    }
}