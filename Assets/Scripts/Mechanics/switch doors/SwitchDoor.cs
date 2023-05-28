using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public bool redBlue;
    private GameObject door;

    //variables
    private bool _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY, _openY;

    void Start()
    {
        door = transform.Find("Door").gameObject;
        door = transform.Find("Door").gameObject;
        _closeY = door.transform.position.y;
        _openY = _closeY + 8;
    }

    private void Update()
    {
        if (_openDoor)
        {
            if (door.transform.position.y < _openY)
                door.transform.position += new Vector3(0, 0.2f, 0);
            else
                _openDoor = false;
        }

        if (_closeDoor)
        {
            if (door.transform.position.y > _closeY)
                door.transform.position -= new Vector3(0, 0.2f, 0);
            else
                _closeDoor = false;
        }
    }

    // Update is called once per frame
    public void OpenDoor()
    {
        _openDoor = true;
        _closeDoor = false;
    }

    public void CloseDoor()
    {
        _openDoor = false;
        _closeDoor = true;
    }
}