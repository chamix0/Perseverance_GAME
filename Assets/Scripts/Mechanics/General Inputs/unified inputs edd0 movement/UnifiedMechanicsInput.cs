using System.Collections.Generic;
using Mechanics.General_Inputs;
using Mechanics.General_Inputs.Machine_gun_mode;
using Player.Observer_pattern;
using UnityEngine;
using UTILS;

public class UnifiedMechanicsInput : MonoBehaviour, IObserver
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
    private Distraction _distraction;

    //run mode
    private Stamina _stamina;
    [SerializeField] private ParticleSystem turboParticles;

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
        _stamina = FindObjectOfType<Stamina>();
        _distraction = FindObjectOfType<Distraction>();
        machineGuns.AddRange(FindObjectsOfType<MachineGun>());
        _playerValues.AddObserver(this);
        //show tutorial
        guiManager.ShowTutorial();
        UpdateTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentInput currentInput = _playerValues.GetCurrentInput();
        //show machine guns
        if (currentInput == CurrentInput.ShootMovement)
            ShowMachineGuns();
        else
            HideMachineGuns();

        //show distraction
        if (currentInput == CurrentInput.StealthMovement)
        {
            if (!_distraction.GetIsVisible())
                _distraction.SetVisible(true);
        }

        //show stamina
        if (currentInput is CurrentInput.RaceMovement)
        {
            if (!_stamina.beingShown)
                _stamina.ShowStamina();
        }

        if (currentInput is CurrentInput.ShootMovement or CurrentInput.RaceMovement or CurrentInput.Movement
                or CurrentInput.StealthMovement &&
            _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            //common actions
            if (_newInputs.GearUp())
            {
                if (currentInput is CurrentInput.RaceMovement)
                {
                    _playerValues.RiseGear();
                    if (_playerValues.GetGear() == 4)
                        turboParticles.Play();
                    else
                        turboParticles.Stop();
                }
                else
                {
                    if (_playerValues.GetGear() < 3)
                        _playerValues.RiseGear();
                }

                _playerValues.CheckIfStuck(true);
            }

            if (_newInputs.GearDown())
            {
                _playerValues.DecreaseGear();
                turboParticles.Stop();
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
                    _cameraController.SlowCamera(3);
                    foreach (var machineGun in machineGuns)
                        machineGun.Aim();
                }

                if (_newInputs.AimRelease())
                {
                    _cameraController.NormalCameraSpeed();
                    foreach (var machineGun in machineGuns)
                        machineGun.StopAim();
                }

                if (_newInputs.ChangeWeapon())
                {
                    foreach (var machineGun in machineGuns)
                        machineGun.NextShootingMode();
                }
            }

            //stealth actions
            if (currentInput is CurrentInput.StealthMovement)
            {
                if (_newInputs.GrenadeDistraction())
                    _distraction.ThrowDistraction();
            }
            else
            {
                //hide distraction
                if (_distraction.GetIsVisible())
                    _distraction.SetVisible(false);
            }

            //run movement
            if (currentInput is not CurrentInput.RaceMovement)
            {
                if (_stamina.beingShown)
                    _stamina.HideStamina();
                if (turboParticles.isPlaying)
                    turboParticles.Stop();
                if (_playerValues.GetGear() == 4)
                    _playerValues.DecreaseGear();
            }
        }
    }

    public void PerformAction(Move move)
    {
        CurrentInput currentInput = _playerValues.GetCurrentInput();

        if (_playerValues.GetInputsEnabled())
        {
            //set cube as input device
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            _playerValues.CheckIfStuck(true);
            //common actions
            //gears
            if (move.face == FACES.R)
            {
                if (move.direction == 1)
                {
                    if (currentInput is CurrentInput.RaceMovement)
                    {
                        _playerValues.RiseGear();
                    }
                    else
                    {
                        if (_playerValues.GetGear() < 3)
                            _playerValues.RiseGear();
                    }

                    if (_playerValues.GetGear() == 4)
                        turboParticles.Play();
                    else
                        turboParticles.Stop();
                }
                else
                    _playerValues.DecreaseGear();
            }
            //lights
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

            //machine gun actions
            if (currentInput is CurrentInput.ShootMovement)
            {
                //shoot and aim
                if (move.face == FACES.F)
                {
                    if (move.direction == -1)
                    {
                        foreach (var machineGun in machineGuns)
                        {
                            machineGun.Aim();
                            machineGun.StopAutomaticShooting();
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
                //change shooting mode
                else if (move.face == FACES.D)
                {
                    if (move.direction == -1)
                        foreach (var machineGun in machineGuns)
                            machineGun.NextShootingMode();
                }
            }

            //stealth actions
            if (currentInput is CurrentInput.StealthMovement)
            {
                if (move.face == FACES.M)

                    _distraction.ThrowDistraction();
            }
        }

        //camera movement
        if (move.face is FACES.U)
        {
            if (move.direction == 1)
                _cameraController.RotateClockwise();
            else
                _cameraController.RotateCounterClockwise();
        }
        //vertical camera movement
        else if (move.face == FACES.L)
        {
            if (move.direction == 1)
                _cameraController.RotateVerticalDown();
            else
                _cameraController.RotateVerticalUp();
        }
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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode && _playerValues.GetCurrentInput() is CurrentInput.Movement
                or CurrentInput.RaceMovement or CurrentInput.StealthMovement or CurrentInput.ShootMovement)
        {
            UpdateTutorial();
        }
    }

    private void UpdateTutorial()
    {
        CurrentInput currentInput = _playerValues.GetCurrentInput();
        guiManager.ShowTutorial();
        if (currentInput is CurrentInput.Movement or CurrentInput.RaceMovement)
        {
            guiManager.SetTutorial(
                _newInputs.GearUpText() + "- Gear Up |" + _newInputs.GearDownText() + "- Gear Down |" +
                _newInputs.CamMovementText() + "- Camera |" + _newInputs.LightsText() + "- Lights |" +
                _newInputs.PauseText() + "- Pause");
        }
        else if (currentInput is CurrentInput.ShootMovement)
        {
            guiManager.SetTutorial(
                _newInputs.GearUpText() + "- Gear Up |" + _newInputs.GearDownText() + "- Gear Down |" +
                _newInputs.CamMovementText() + "- Camera |" + _newInputs.LightsText() + "- Lights |" +
                _newInputs.AimText() + "- Aim |" + _newInputs.ShootText() + "- Shoot |" + _newInputs.ArmorWheelText() +
                "- Change Shooting Mode");
        }
        else if (currentInput is CurrentInput.StealthMovement)
        {
            guiManager.SetTutorial(
                _newInputs.GearUpText() + "- Gear Up |" + _newInputs.GearDownText() + "- Gear Down |" +
                _newInputs.CamMovementText() + "- Camera |" + _newInputs.LightsText() + "- Lights |" +
                _newInputs.GrenadeDistractionText() + "- Distraction");
        }
    }
}