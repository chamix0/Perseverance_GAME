using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LockerBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private OrbitCameraController _cameraController;
    private GameObject _door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private CameraChanger cameraChanger;

    //minigame
    private LockerManager _lockerManager;

    //variables
    private bool _minigameFinished, _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY;


    //values
    private const float OpenY = 5;
    private List<int> _code;
    private bool minigamePlaying;

    //lists
    [SerializeField] private List<TMP_Text> codeNumbersText;


    private void Awake()
    {
        _code = new List<int>();
    }

    void Start()
    {
        _lockerManager = FindObjectOfType<LockerManager>();
        _cameraController = FindObjectOfType<OrbitCameraController>();
        cameraChanger = FindObjectOfType<CameraChanger>();
        StartPhase();
        var parent = transform.parent;
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _door = parent.Find("Door").gameObject;
        _closeY = _door.transform.position.y;
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    public bool CheckGetDigit(int index, int value)
    {
        return _code[index] == value;
    }

    private void StartPhase()
    {
        _minigameFinished = false;
        SetCode();
    }

    private void SetCode()
    {
        _code.Clear();
        for (int i = 0; i < 4; i++)
        {
            int val = Random.Range(0, 10);
            _code.Add(val);
            codeNumbersText[i].text = val + "";
        }
    }

    private void Update()
    {
        if (_openDoor)
        {
            if (_door.transform.position.y < OpenY)
                _door.transform.position += new Vector3(0, 0.1f, 0);
            else
                _openDoor = false;
        }

        if (_closeDoor)
        {
            if (_door.transform.position.y > _closeY)
                _door.transform.position -= new Vector3(0, 0.1f, 0);
            else
                _closeDoor = false;
        }
    }


    private void OpenDoor()
    {
        _openDoor = true;
        _closeDoor = false;
    }

    public void CloseDoor()
    {
        _openDoor = false;
        _closeDoor = true;
    }


    IEnumerator ChangeCameraCoroutine()
    {
        yield return new WaitForSeconds(3f);
        _lockerManager.StartMinigame();
        cameraChanger.SetScreenCamera();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_minigameFinished && !_inside)
        {
            _inside = true;
            _playerValues.snapRotationTo(_snapPos.transform.eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _playerValues.Sit();
            //Empezar minijuego
            _lockerManager.SetLockerBase(this);
            StartCoroutine(ChangeCameraCoroutine());
            minigamePlaying = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !minigamePlaying&& _inside)
        {
            _inside = false;
        }
    }

    public void EndLocker()
    {
        _minigameFinished = true;
        OpenDoor();
    }

    public void ExitBase()
    {
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(true, 2.5f);
        minigamePlaying = false;
    }
}