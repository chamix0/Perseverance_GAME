using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum RaceStates
{
    Start,
    End,
    Exit
}

public class StartRaceTrigger : MonoBehaviour
{
    [SerializeField] private RaceManager raceManager;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            raceManager.StartRace();
    }
}