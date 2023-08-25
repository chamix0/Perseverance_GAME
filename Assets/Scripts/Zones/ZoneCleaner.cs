using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneCleaner : Subject
{
    // Start is called before the first frame update
    private LoadScreen _loadScreen;

    void Start()
    {
        _loadScreen = FindObjectOfType<LoadScreen>();
        LoadAllZones();
    }
    
    private void LoadAllZones()
    {
        _loadScreen.LoadFreezer();
    }

    public void EnableZone(PlayerActions playerAction)
    {
        NotifyObservers(playerAction);
    }
}