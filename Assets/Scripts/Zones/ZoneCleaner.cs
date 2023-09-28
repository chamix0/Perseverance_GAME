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
        // LoadAllZones();
    }

    // private void LoadAllZones()
    // {
    //     _loadScreen.LoadFoundry();
    //     _loadScreen.LoadFreezer();
    //     _loadScreen.LoadWarehouse();
    //     _loadScreen.LoadGarden();
    //     _loadScreen.LoadResidential();
    // }

    public void EnableZone(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.LoadFoundry)
        {
            _loadScreen.LoadFoundry();
        }
        else if (playerAction is PlayerActions.LoadFreezer)
        {
            _loadScreen.LoadFreezer();
        }
        else if (playerAction is PlayerActions.LoadGarden)
        {
            _loadScreen.LoadGarden();
        }
        else if (playerAction is PlayerActions.LoadResidentialZone)
        {
            _loadScreen.LoadResidential();
        }
        else if (playerAction is PlayerActions.LoadWareHouse)
        {
            _loadScreen.LoadWarehouse();
        }
        // NotifyObservers(playerAction);
    }
}