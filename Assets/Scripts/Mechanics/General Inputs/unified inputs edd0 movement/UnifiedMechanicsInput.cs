using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using Mechanics.General_Inputs.Machine_gun_mode;
using Player.Observer_pattern;
using UnityEngine;

public class UnifiedMechanicsInput : MonoBehaviour
{
    //common components
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private CameraChanger _cameraChanger;
    private GuiManager guiManager;
    private PlayerNewInputs _newInputs;

    //shooting mode
    private List<MachineGun> machineGuns;
    private MyStopWatch shootTimer;
    [SerializeField] private int shootCooldown = 250;

    //stealth mode

    //run mode

    private void Awake()
    {
        machineGuns = new List<MachineGun>();
        shootTimer = gameObject.AddComponent<MyStopWatch>();
        shootTimer.StartStopwatch();
    }

    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        machineGuns.AddRange(FindObjectsOfType<MachineGun>());
    }

    // Update is called once per frame
    void Update()
    {
        //show machine guns
        if (_playerValues.GetCurrentInput() == CurrentInput.ShootMovement)
            ShowMachineGuns();
        else
            HideMachineGuns();

        CurrentInput currentInput = _playerValues.GetCurrentInput();
        if (currentInput is CurrentInput.ShootMovement or CurrentInput.Movement &&
            _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            //common actions
            if (_newInputs.GearUp())
            {
                if (_playerValues.GetGear() < 3)
                    _playerValues.RiseGear();
                _playerValues.CheckIfStuck(true);
            }

            if (_newInputs.GearDown())
            {
                _playerValues.DecreaseGear();
                _playerValues.CheckIfStuck(true);
            }

            if (_newInputs.Lights())
            {
                _playerValues.NotifyAction(_playerValues.GetLights()
                    ? PlayerActions.TurnOffLights
                    : PlayerActions.TurnOnLights);
            }

            //shooting actions
            if (currentInput is CurrentInput.ShootMovement)
            {
                if (_newInputs.Shoot())
                {
                    if (shootTimer.GetElapsedMiliseconds() > shootCooldown)
                    {
                        shootTimer.Restart();
                        foreach (var machineGun in machineGuns)
                            shootCooldown = machineGun.Shoot();
                    }
                }

                if (_newInputs.ShootReleased())
                {
                    foreach (var machineGun in machineGuns)
                        machineGun.StopAutomaticShooting();
                }

                if (_newInputs.Aim())
                {
                    foreach (var machineGun in machineGuns)
                        machineGun.Aim();
                }

                if (_newInputs.AimRelease())
                {
                    foreach (var machineGun in machineGuns)
                        machineGun.StopAim();
                }

                if (_newInputs.ChangeWeapon())
                {
                    foreach (var machineGun in machineGuns)
                        machineGun.NextShootingMode();
                }
            }
        }
    }

    public void PerformAction(Move move)
    {
        //set camera height
        _playerValues.CheckIfStuck(true);
        if (_playerValues.GetInputsEnabled())
        {
            //common actions
            //gears
            if (move.face == FACES.R)
            {
                if (move.direction == 1)
                    if (_playerValues.GetGear() < 3)
                        _playerValues.RiseGear();
                    else
                        _playerValues.DecreaseGear();
            }
            //vertical camera movement
            else if (move.face == FACES.L)
            {
                if (move.direction == 1)
                    _cameraController.RotateVerticalDown();
                else
                    _cameraController.RotateVerticalUp();
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
                    if (shootTimer.GetElapsedMiliseconds() > shootCooldown)
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
                    if (_playerValues.GetLights())
                        _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                }
                else
                {
                    if (!_playerValues.GetLights())
                        _playerValues.NotifyAction(PlayerActions.TurnOnLights);
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

        if (move.face is FACES.U)
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


    #region private  methods

    private void HideMachineGuns()
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

    private void ShowMachineGuns()
    {
        foreach (var machineGun in machineGuns)
            machineGun.ShowMachineGun();
    }

    #endregion
}