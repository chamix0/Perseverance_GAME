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
    private CameraController _cameraController;
    private CameraChanger _cameraChanger;
    private List<MachineGun> machineGuns;

    private Stopwatch shootTimer;
    [SerializeField] private int shootCooldown = 250;
    [SerializeField] private float yAngle;

    private void Awake()
    {
        machineGuns = new List<MachineGun>();
        shootTimer = new Stopwatch();
        shootTimer.Start();
    }

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
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
            bool changeCam = false;
            foreach (var machineGun in machineGuns)
            {
                changeCam = machineGun.HideMachinegun();
            }

            if (changeCam)
            {
                _cameraChanger.SetOrbitCamera();
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
                    // if (machineGuns[0].GetAim()) _cameraController.Shake();
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
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                    _cameraChanger.SetOrbitCamera();
                }
                else
                {
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
                    _cameraChanger.SetFirstPersonCamera();
                }
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (_cameraChanger.activeCamera is not ActiveCamera.FirstPerson)
                    _cameraChanger.SetFirstPersonCamera();
                else
                    _cameraChanger.SetOrbitCamera();
            }
        }
    }

    public void PerformAction(Move move)
    {
        //set camera height
        if (_cameraChanger.activeCamera is ActiveCamera.Orbit)
            _cameraController.RotateVerticalCustom(yAngle);
        _playerValues.CheckIfStuck();

        //accelerate deccelerate
        if (move.face == FACES.R)
        {
            if (_cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            {
                if (move.direction == 1)
                    _cameraController.RotateVerticalUp();
                else
                    _cameraController.RotateVerticalDown();
            }
            else
            {
                if (move.direction == 1)
                {
                    if (_playerValues.GetGear() < 3)
                        _playerValues.RiseGear();
                }
                else
                {
                    _playerValues.DecreaseGear();
                }
            }
        }
        //turn camera in y axis
        else if (move.face == FACES.L)
        {
            if (move.direction == 1)
            {
                foreach (var machineGun in machineGuns)
                {
                    machineGun.Aim();
                }
            }
            else
            {
                if (shootTimer.Elapsed.TotalMilliseconds > shootCooldown)
                {
                    shootTimer.Restart();
                    foreach (var machineGun in machineGuns)
                    {
                        shootCooldown = machineGun.Shoot();
                        machineGun.StopAim();
                    }
                }
            }
        }
        else if (move.face == FACES.U)
        {
            if (move.direction == 1)
                _cameraController.RotateClockwise();
            else
                _cameraController.RotateCounterClockwise();
        }
        else if (move.face == FACES.B)
        {
            if (move.direction == 1)
            {
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else
                    _cameraChanger.SetOrbitCamera();
            }
            else
            {
                if (!_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
                else
                    _cameraChanger.SetFirstPersonCamera();
            }
        }
        else if (move.face == FACES.D)
        {
            if (move.direction == 1)
                foreach (var machineGun in machineGuns)
                    machineGun.NextShootingMode();
            else
                foreach (var machineGun in machineGuns)
                    machineGun.PrevShootingMode();
        }
    }
}