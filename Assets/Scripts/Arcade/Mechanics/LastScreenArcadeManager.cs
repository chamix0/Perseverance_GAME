using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastScreenArcadeManager : MonoBehaviour
{
    // Start is called before the first frame update
    private JSONsaving _jsoNsaving;
    [SerializeField] private List<TMP_Text> _texts;
    [SerializeField] private Button exit, retry;
    private LoadScreen _loadScreen;

    void Start()
    {
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _loadScreen = FindObjectOfType<LoadScreen>();
        exit.onClick.AddListener(_loadScreen.LoadMenu);
        retry.onClick.AddListener(_loadScreen.LoadArcadeGame);
        ArcadeStats arcadeStats = _jsoNsaving._saveData.GetArcadeStats();
        _texts[0].text = arcadeStats.TotalPoints + " pts";
        _texts[1].text = arcadeStats.Rounds + "";
        _texts[2].text = arcadeStats.Level + "";
        _texts[3].text = arcadeStats.EnemiesKilled + "";
        _texts[4].text = arcadeStats.ZonesUnlocked;
        _texts[5].text = arcadeStats.UnlockedGears;
        _texts[6].text = arcadeStats.PowerEnabled;
        CursorManager.ShowCursor();
    }
}