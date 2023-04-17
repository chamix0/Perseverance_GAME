using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class MachinegunMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private OrbitCameraController _cameraController;
    private CameraChanger _cameraChanger;
    private List<MachineGun> machineGuns;

    private Stopwatch shootTimer;
    [SerializeField] private int shootCooldown = 250;

    private void Awake()
    {
        machineGuns = new List<MachineGun>();
        shootTimer = new Stopwatch();
        shootTimer.Start();
    }

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<OrbitCameraController>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        machineGuns.AddRange(FindObjectsOfType<MachineGun>());
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.ShootMovement)
        {
            foreach (var machineGun in machineGuns)
            {
                machineGun.ShowMachineGun();
            }
        }
        else
        {
            foreach (var machineGun in machineGuns)
            {
                machineGun.HideMachinegun();
            }
        }

        if (_playerValues.GetCurrentInput() == CurrentInput.ShootMovement && _playerValues.GetInputsEnabled())
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

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (shootTimer.Elapsed.TotalMilliseconds > shootCooldown)
                {
                    shootTimer.Restart();
                    foreach (var machineGun in machineGuns)
                    {
                        shootCooldown = machineGun.Shoot();
                    }
                }
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                foreach (var machineGun in machineGuns)
                {
                    machineGun.Aim();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                foreach (var machineGun in machineGuns)
                {
                    machineGun.StopAim();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (var machineGun in machineGuns)
                {
                    machineGun.NextShootingMode();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                foreach (var machineGun in machineGuns)
                {
                    machineGun.PrevShootingMode();
                }
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (_playerValues.GetLights())
                {
                    _playerValues.TurnOffLights();
                    _cameraChanger.SetFirstPersonCamera();
                }
                else
                {
                    _playerValues.TurnOnLights();
                    _cameraChanger.SetOrbitCamera();
                }
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