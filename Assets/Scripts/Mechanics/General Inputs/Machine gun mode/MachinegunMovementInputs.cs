using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class MachinegunMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private CameraChanger _cameraChanger;
    private List<MachineGun> machineGuns;
    private GuiManager guiManager;

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
        guiManager = FindObjectOfType<GuiManager>();
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
                machineGun.ResetShootingMode();
                changeCam = machineGun.HideMachinegun();
            }

            if (changeCam)
            {
                _cameraChanger.SetOrbitCamera();
            }
        }

        if (_playerValues.GetCurrentInput() == CurrentInput.ShootMovement && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                guiManager.SetTutorial(
                    "WS - Increase/Descrease gear    RCLICK - Aim    LCLICK - Shoot      EQ - Change shooting mode       D - Lights      " +
                    "V - First person mode");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_playerValues.GetGear() < 3)
                    _playerValues.RiseGear();
                _playerValues.CheckIfStuck(true);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _playerValues.DecreaseGear();
                _playerValues.CheckIfStuck(true);
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

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_playerValues.GetLights())
                {
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                }
                else
                {
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
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
        _playerValues.CheckIfStuck(true);
        if (_playerValues.GetInputsEnabled())
        {
            if (_cameraChanger.activeCamera is ActiveCamera.FirstPerson)
                guiManager.SetTutorial(
                    "R - Camera vertical axis   U - Camera horizonatal axis    L - Aim and load     L' - Shoot     B' - Lights      B2 - Third person mode       F - Change Shooting mode");
            else
                guiManager.SetTutorial(
                    "R - Increase/Decrease Gear    U - Camera horizonatal axis    L - Aim and load     L' - Shoot     B' - Lights      B'2 - First person mode       F - Change Shooting mode");

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
            else if (move.face == FACES.B)
            {
                if (move.direction == 1)
                {
                    if (_cameraChanger.activeCamera is ActiveCamera.FirstPerson)
                    {
                        _cameraChanger.SetOrbitCamera();
                    }
                    else if (_playerValues.GetLights())
                        _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                }
                else
                {
                    if (!_playerValues.GetLights())
                        _playerValues.NotifyAction(PlayerActions.TurnOnLights);
                    else if (_cameraChanger.activeCamera is not ActiveCamera.FirstPerson)
                        _cameraChanger.SetFirstPersonCamera();
                }
            }
            else if (move.face == FACES.F)
            {
                if (move.direction == 1)
                    foreach (var machineGun in machineGuns)
                        machineGun.NextShootingMode();
                else
                    foreach (var machineGun in machineGuns)
                        machineGun.PrevShootingMode();
            }
        }

        if (move.face == FACES.U)
        {
            if (move.direction == 1)
                _cameraController.RotateClockwise();
            else
                _cameraController.RotateCounterClockwise();
        }
    }

    public void AddObservers(IObserver observer)
    {
        machineGuns[0].AddObserver(observer);
    }
}