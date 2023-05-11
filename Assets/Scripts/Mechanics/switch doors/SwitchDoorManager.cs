using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(1)]
public class SwitchDoorManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<SwitchDoor> switchDoors;


    private void Awake()
    {
        switchDoors = new List<SwitchDoor>();
    }

    void Start()
    {
        switchDoors.AddRange(GetComponentsInChildren<SwitchDoor>());
    }

    // Update is called once per frame

    public void FlickTheSwitch(bool val)
    {
        foreach (var door in switchDoors)
        {
            if (door.redBlue == val)
                door.OpenDoor();
            else
                door.CloseDoor();
        }
    }
    
}