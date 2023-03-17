using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private TimeZoneManager _timeZoneManager;

    // Start is called before the first frame update
    void Start()
    {
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _playerValues = FindObjectOfType<PlayerValues>();
        _timeZoneManager = transform.parent.GetComponent<TimeZoneManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerValues.snapRotationTo(_snapPos.transform.eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _playerValues.Sit();
            _cameraChanger.SetScreenCamera();
            //do stuff
            
            _timeZoneManager.EndRun();
        }
    }
}