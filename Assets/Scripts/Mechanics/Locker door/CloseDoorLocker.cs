using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorLocker : MonoBehaviour
{
    //components
    private LockerBase lockerBase;

    void Start()
    {
        lockerBase = transform.parent.GetComponentInChildren<LockerBase>();
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            lockerBase.CloseDoor();
    }
    
}