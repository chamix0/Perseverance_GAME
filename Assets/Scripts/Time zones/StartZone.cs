using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    private TimeZoneManager _timeZoneManager;
    [SerializeField] private DoorManager loadedLevelDoor;
    private ZoneCleaner _zoneCleaner;
    private bool actionPerformed;
    private LoadScreen _loadScreen;

    private void Start()
    {
        _timeZoneManager = transform.parent.GetComponent<TimeZoneManager>();
        _zoneCleaner = FindObjectOfType<ZoneCleaner>();
        _loadScreen = FindObjectOfType<LoadScreen>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !actionPerformed)
        {
            actionPerformed = true;
            _timeZoneManager.StartRun();
            StartCoroutine(LoadZoneCoroutine());
        }
    }

    IEnumerator LoadZoneCoroutine()
    {
        _zoneCleaner.EnableZone(IndexToPlayerAction(_timeZoneManager.zone));
        yield return new WaitUntil(_loadScreen.Loaded);
        loadedLevelDoor.OpenDoor();
    }

    private PlayerActions IndexToPlayerAction(int index)
    {
        if (index == 0)
            return PlayerActions.LoadFoundry;
        if (index == 1)
            return PlayerActions.LoadFreezer;
        if (index == 2)
            return PlayerActions.LoadWareHouse;
        if (index == 3)
            return PlayerActions.LoadGarden;
        if (index == 4)
            return PlayerActions.LoadResidentialZone;
        return PlayerActions.LoadFoundry;
    }
}