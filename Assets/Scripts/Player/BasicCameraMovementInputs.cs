using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class BasicCameraMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private PlayerLights _playerLights;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
        _playerLights = FindObjectOfType<PlayerLights>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (Input.GetKeyDown(KeyCode.W))
        {
            _playerValues.ResetRigidBodyConstraints();
            _playerValues.RiseGear();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _playerValues.ResetRigidBodyConstraints();
            _playerValues.DecreaseGear();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_playerValues.lightsOn)
                _playerValues.TurnOffLights();
            else
                _playerValues.TurnOnLights();
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        if (move.face == FACES.R)
        {
            if (move.direction == 1)
                _playerValues.RiseGear();
            else
                _playerValues.DecreaseGear();
        }
        //turn camera in y axis
        else if (move.face == FACES.L)
        {
            if (move.direction == 1)
                _cameraController.RotateYClockwise();
            else
                _cameraController.RotateYCounterClockwise();
        }
        else if (move.face == FACES.U)
        {
            if (move.direction == 1)
                _cameraController.RotateXClockwise();
            else
                _cameraController.RotateXCounterClockwise();
        }
        else if (move.face == FACES.B)
        {
            if (move.direction == 1)
                _playerValues.TurnOffLights();
            else
                _playerValues.TurnOnLights();
        }
    }
}