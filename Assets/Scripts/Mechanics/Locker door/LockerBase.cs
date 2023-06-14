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
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private CameraChanger cameraChanger;
   [SerializeField] private DoorManager doorManager;

    //minigame
    private LockerManager _lockerManager;

    //variables
    private bool _minigameFinished, _inside;


    //values
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
        doorManager.OpenDoor();
    }

    public void ExitBase()
    {
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(true, 2.5f);
        minigamePlaying = false;
    }
}