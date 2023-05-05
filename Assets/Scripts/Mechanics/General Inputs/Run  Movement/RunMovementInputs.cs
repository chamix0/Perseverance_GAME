using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class RunMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    [SerializeField] private ParticleSystem turboParticles;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.RaceMovement && _playerValues.GetInputsEnabled())
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _playerValues.RiseGear();
                if (_playerValues.GetGear() == 4)
                    turboParticles.Play();
                else
                    turboParticles.Stop();
                _playerValues.CheckIfStuck();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _playerValues.DecreaseGear();
                turboParticles.Stop();
                _playerValues.CheckIfStuck();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
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
            
            if (_playerValues.GetGear() == 4)
                turboParticles.Play();
            else
                turboParticles.Stop();
        }
        //turn camera in y axis
        else if (move.face == FACES.L)
        {
            if (move.direction == 1)
                _cameraController.RotateVerticalDown();
            else
                _cameraController.RotateVerticalUp();
        }
        else if (move.face == FACES.U)
        {
            if (move.direction == 1) _cameraController.RotateClockwise();
            else _cameraController.RotateCounterClockwise();
        }
        else if (move.face == FACES.B)
        {
            if (move.direction == 1)
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else if (!_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
        }
    }
}