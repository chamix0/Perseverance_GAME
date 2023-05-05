using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    enum RaceStates
    {
        Start,
        End,
        Exit
    }


    private RaceManager raceManager;
    [SerializeField] private RaceStates raceState = RaceStates.Start;

    private void Start()
    {
        raceManager = transform.parent.GetComponent<RaceManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (raceState)
            {
                case RaceStates.Start:
                    raceManager.StartRace();
                    break;
                case RaceStates.Exit:
                    raceManager.ExitRace();
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (raceState == RaceStates.End)
            {
                raceManager.EndRace();
            }
        }
    }
}