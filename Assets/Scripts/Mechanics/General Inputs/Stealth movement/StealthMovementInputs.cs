using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using TreeEditor;
using UnityEngine;

public class StealthMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private OrbitCameraController _cameraController;
    private Distraction _distraction;


    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<OrbitCameraController>();
        _distraction = FindObjectOfType<Distraction>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.StealthMovement)
        {
            if (!_distraction.GetIsVisible())
                _distraction.SetVisible(true);
        }
        
        if (_playerValues.GetCurrentInput() == CurrentInput.StealthMovement && _playerValues.GetInputsEnabled())
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                _distraction.ThrowDistraction();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _distraction.RecuperateDistraction();
            }
        }
        else
        {
            if (_distraction.GetIsVisible())
                _distraction.SetVisible(false);
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        if (move.face == FACES.R)
        {
            _playerValues.CheckIfStuck();
            if (move.direction == 1)
            {
                if (_playerValues.GetGear() < 3)
                    _playerValues.RiseGear();
            }
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
        else if (move.face == FACES.M)
        {
            if (move.direction == 1)
                _distraction.RecuperateDistraction();
            else
                _distraction.ThrowDistraction();
        }
    }
}