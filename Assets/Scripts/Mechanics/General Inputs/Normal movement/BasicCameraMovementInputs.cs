using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class BasicCameraMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private OrbitCameraController _cameraController;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<OrbitCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.Movement && _playerValues.GetInputsEnabled())
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_playerValues.GetGear() < 3)
                    _playerValues.RiseGear();

                _playerValues.CheckIfStuck();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _playerValues.DecreaseGear();
                _playerValues.CheckIfStuck();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (_playerValues.GetLights())
                    _playerValues.TurnOffLights();
                else
                    _playerValues.TurnOnLights();
            }
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        if (move.face == FACES.R)
        {
            _playerValues.CheckIfStuck();

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