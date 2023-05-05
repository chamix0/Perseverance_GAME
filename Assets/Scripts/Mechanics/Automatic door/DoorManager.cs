using System;
using System.Diagnostics;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    //components
    private GameObject door;

    //variables
    public bool _inside;
    private bool _openDoor, _closeDoor;
    [NonSerialized] public bool opened;
    private float _closeY, _openY;

    private Stopwatch closeTimer;

    private float closeTime = 10f;
    //values


    void Start()
    {
        door = transform.Find("Door").gameObject;
        opened = false;
        _closeY = door.transform.position.y;
        _openY = _closeY + 8;
        closeTimer = new Stopwatch();
        closeTimer.Start();
    }

    private void Update()
    {
        if (_openDoor)
        {
            if (door.transform.position.y < _openY)
                door.transform.position += new Vector3(0, 0.1f, 0);
            else
                _openDoor = false;
        }

        if (_closeDoor)
        {
            if (door.transform.position.y > _closeY)
                door.transform.position -= new Vector3(0, 0.1f, 0);
            else
                _closeDoor = false;
        }

        if (!_inside && opened && closeTimer.Elapsed.TotalSeconds > closeTime)
        {
            closeTimer.Stop();
            CloseDoor();
        }

        if (_inside)
        {
            closeTimer.Restart();
        }
    }

    public void OpenDoor()
    {
        closeTimer.Restart();
        _openDoor = true;
        _closeDoor = false;
        opened = true;
    }

    public void CloseDoor()
    {
        closeTimer.Restart();
        _openDoor = false;
        _closeDoor = true;
        opened = false;
    }
}