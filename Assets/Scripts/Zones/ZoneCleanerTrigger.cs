using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCleanerTrigger : MonoBehaviour
{
    private ZoneCleaner zoneCleaner;
    [SerializeField] private int zoneIndex;

    void Start()
    {
        zoneCleaner = FindObjectOfType<ZoneCleaner>();
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zoneCleaner.DisableZonesNotUsed(zoneIndex);
        }
    }
}