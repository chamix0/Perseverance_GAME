using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public bool redBlue;
    private DoorManager doorManager;

    void Start()
    {
        doorManager = GetComponent<DoorManager>();
    }

    // Update is called once per frame
    public void OpenDoor()
    {
        doorManager.OpenDoor();
    }

    public void CloseDoor()
    {
        doorManager.CloseDoor();
    }
}