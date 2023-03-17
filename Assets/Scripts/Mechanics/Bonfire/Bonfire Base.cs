using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireBase : MonoBehaviour
{
  //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private CameraController _cameraController;
    private GameObject _door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;

    //minigame
    private LockerManager _lockerManager;

    //variables
    private bool _minigameFinished, _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY;


    //values
    private const float OpenY = 5;
    private List<int> _code;

    //lists


    private void Awake()
    {
        _code = new List<int>();
    }

    void Start()
    {
        _lockerManager = FindObjectOfType<LockerManager>();
        _cameraController = FindObjectOfType<CameraController>();
        var parent = transform.parent;
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _closeY = _door.transform.position.y;
        _playerValues = FindObjectOfType<PlayerValues>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_minigameFinished && !_inside)
        {
            _inside = true;
            _playerValues.snapRotationTo(_snapPos.transform.eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _cameraController.RotateXCustom(MyUtils.Clamp0360(-_snapPos.transform.eulerAngles.y));
            _cameraController.RotateYCustom(0.5f);
            _cameraController.FreezeCamera();
            _playerValues.Sit();
            _cameraChanger.SetScreenCamera();
            //Empezar minijuego
            _lockerManager.StartMinigame();
        }
    }


    public void ExitBase()
    {
        StartCoroutine(ExitBaseCoroutine());
    }

    IEnumerator ExitBaseCoroutine()
    {
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(false, 2.5f);
        yield return new WaitForSeconds(2.5f);
        _playerValues.SetGear(0);
        yield return new WaitForSeconds(3f);
        _playerValues.StopMovement();
        _cameraController.UnFreezeCamera();
        _playerValues.SetInputsEnabled(true);
        _inside = false;
        yield return new WaitForSeconds(2f);
    }
}
