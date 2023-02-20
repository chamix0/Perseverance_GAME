using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaberinthManager : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private MinigameManager _minigameManager;
    private CameraChanger cameraChanger;
    private CameraController _cameraController;
    private PlayerAnimations _playerAnimations;
    private GameObject door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private GenericScreenUi _genericScreenUi;

    //variables
    private bool _minigameFinished, _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY;

    //values
    private const float OpenY = 5;

    //lists
    private List<TerminalLaberinth> _terminalLaberinthList;

    private void Awake()
    {
        _terminalLaberinthList = new List<TerminalLaberinth>();
    }

    void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        var parent = transform.parent;
        _terminalLaberinthList.AddRange(parent.GetComponentsInChildren<TerminalLaberinth>());
        cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        door = parent.Find("Door").gameObject;
        _closeY = door.transform.position.y;
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    private void Update()
    {
        if (_openDoor)
        {
            if (door.transform.position.y < OpenY)
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

    private int GetFinishedTerminals()
    {
        int sum = 0;
        foreach (var terminal in _terminalLaberinthList)
            sum = terminal.GetMinigameFinished() ? sum + 1 : sum;
        return sum;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_minigameFinished && !_inside)
        {
            _inside = true;
            var rotation = transform.rotation;
            _playerValues.snapRotationTo(rotation.y + 180);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _cameraController.RotateXCustom(rotation.y);
            _cameraController.RotateYCustom(0.5f);
            _cameraController.FreezeCamera();

            _playerValues.SetInputsEnabled(false);
            _playerValues.SetCanMove(false);
            _playerAnimations.SetSitAnim(true);
            cameraChanger.SetScreenCamera();
            //mostrar mensaje
            StartCoroutine(ShowMessageOpenDoor());
        }
    }

    private int GetMissingTerminals()
    {
        return _terminalLaberinthList.Count - GetFinishedTerminals();
    }

    IEnumerator ShowMessageOpenDoor()
    {
        string msg = "";
        print("miss " + GetFinishedTerminals());
        if (GetMissingTerminals() > 0)
        {
            msg = "Terminals left:" + GetMissingTerminals();
            _genericScreenUi.SetText(msg);
        }
        else
        {
            msg = "All terminals fixed";
        }

        _genericScreenUi.SetText(msg);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(5f);
        _genericScreenUi.FadeOutText();
        cameraChanger.SetOrbitCamera();
        yield return new WaitForSeconds(2f);
        _playerAnimations.SetSitAnim(false);
        yield return new WaitForSeconds(2.5f);
        _playerValues.SetCanMove(true);
        _playerValues.SetGear(0);
        yield return new WaitForSeconds(3f);
        _playerValues.StopMovement();
        _cameraController.UnFreezeCamera();
        _playerValues.SetInputsEnabled(true);
        _inside = false;
        yield return new WaitForSeconds(2f);
        if (GetMissingTerminals() == 0)
        {
            //open door
            OpenDoor();
            _minigameFinished = true;
        }
    }
}