using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class SwitchDoorManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<SwitchDoor> switchDoors;
    private List<SwitchInterruptor> switchInterruptors;
    public bool redBlue;


    private void Awake()
    {
        switchDoors = new List<SwitchDoor>();
        switchInterruptors = new List<SwitchInterruptor>();
    }

    void Start()
    {
        switchDoors.AddRange(GetComponentsInChildren<SwitchDoor>());
        switchInterruptors.AddRange(GetComponentsInChildren<SwitchInterruptor>());
        OpeninitDoors();
    }

    // Update is called once per frame
    private void OpeninitDoors()
    {
        foreach (var door in switchDoors)
        {
            if (door.redBlue == redBlue)
                door.OpenDoor();
        }

        foreach (var interruptor in switchInterruptors)
        {
            interruptor.SetColor();
        }
    }

    public void FlickTheSwitch()
    {
        redBlue = !redBlue;
        foreach (var interruptor in switchInterruptors)
        {
            interruptor.SetColor();
        }

        foreach (var door in switchDoors)
        {
            if (door.redBlue == redBlue)
                door.OpenDoor();
            else
                door.CloseDoor();
        }
    }
}