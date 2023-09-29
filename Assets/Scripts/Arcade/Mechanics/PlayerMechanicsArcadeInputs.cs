using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using Mechanics.General_Inputs.Machine_gun_mode;
using Player.Observer_pattern;
using UnityEngine;

public class PlayerMechanicsArcadeInputs : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    private PlayerNewInputs _playerNewInputs;
    private PlayerValues _playerValues;
    private PlayerMechanicsArcadeManager _manager;
    private ArcadePlayerData _arcadePlayerData;
    private CameraChanger _cameraChanger;
    private CameraController _cameraController;
    private GuiManager guiManager;

    //run mode
    private Stamina stamina;

    [SerializeField] private ParticleSystem turboParticles;

    //shoot stuff
    private Stopwatch shootTimer;

    private void Awake()
    {
        shootTimer = new Stopwatch();
        shootTimer.Start();
    }

    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        guiManager.AddObserver(this);
        _playerNewInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _arcadePlayerData = FindObjectOfType<ArcadePlayerData>();
        _cameraController = FindObjectOfType<CameraController>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        stamina = FindObjectOfType<Stamina>();
        _manager = FindObjectOfType<PlayerMechanicsArcadeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Show stamina
        if (_arcadePlayerData.unlockedGear == 4)
        {
            if (!stamina.beingShown)
                stamina.ShowStamina();
            if (_playerValues.GetGear() < 4 && turboParticles.isPlaying)
                turboParticles.Stop();
        }

        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() is CurrentInput.ArcadeMechanics)
            _manager.ShowMachineGuns();
        else
            _manager.HideMachineGuns();

        if (_playerValues.GetCurrentInput() == CurrentInput.ArcadeMechanics && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                guiManager.SetTutorial(
                    "WS - Increase/Descrease gear    RCLICK - Aim    LCLICK - Shoot");
            }

            if (!_arcadePlayerData.isArmorWheelDisplayed)
            {
                //gear up
                if (_playerNewInputs.GearUp())
                {
                    int gear = _playerValues.GetGear();
                    if (gear < _arcadePlayerData.unlockedGear)
                        _playerValues.RiseGear();

                    if (_playerValues.GetGear() == 4)
                        turboParticles.Play();
                    else
                        turboParticles.Stop();

                    _playerValues.CheckIfStuck(true);
                }

                //gear down
                if (_playerNewInputs.GearDown())
                {
                    _playerValues.DecreaseGear();
                    turboParticles.Stop();
                    _playerValues.CheckIfStuck(true);
                }

                //shoot
                if (_playerNewInputs.Shoot())
                {
                    if (shootTimer.Elapsed.TotalMilliseconds > _arcadePlayerData.GetShootingCooldown())
                    {
                        shootTimer.Restart();
                        _manager.Shoot();
                    }
                }

                //stop  shooting
                if (_playerNewInputs.ShootReleased())
                {
                    _manager.StopAutomaticShooting();
                }

                //aim
                if (_playerNewInputs.Aim())
                {
                    _manager.Aim();
                }

                //stop aim
                if (_playerNewInputs.AimRelease())
                {
                    _manager.StopAim();
                }

                //grenade
                if (_playerNewInputs.GrenadeDistraction())
                {
                    _manager.ThrowGrenade();
                }

                //open armor wheel
                if (_playerNewInputs.ShowArmorWheel())
                {
                    guiManager.ShowArmorWheel();
                    _manager.StopAutomaticShooting();
                }
            }
            else if (_playerNewInputs.HideArmorWheel())
            {
                guiManager.HideArmorWheel();
                _manager.StopAutomaticShooting();
                _manager.StopAim();
            }

            if (_playerNewInputs.Lights())
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
        _playerNewInputs.SetCubeAsDevice();

        if (_playerValues.GetInputsEnabled())
        {
            //gears
            if (move.face == FACES.R)
            {
                _playerValues.CheckIfStuck(true);
                if (move.direction == 1)
                {
                    if (_playerValues.GetGear() < 3)
                        _playerValues.RiseGear();
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
            //shoot and aim
            else if (move.face == FACES.F)
            {
                if (move.direction == -1)
                {
                    _manager.Aim();
                    _manager.StopAutomaticShooting();
                }
                else
                {
                    if (shootTimer.Elapsed.TotalMilliseconds > _arcadePlayerData.GetShootingCooldown())
                    {
                        shootTimer.Restart();
                        _manager.Shoot();
                        _manager.StopAim();
                    }
                }
            }
            //Armor wheel
            else if (move.face == FACES.D)
            {
                if (move.direction == -1)
                {
                    guiManager.ShowArmorWheel();
                    _manager.StopAutomaticShooting();
                }
                else
                {
                    guiManager.HideArmorWheel();
                    _manager.StopAutomaticShooting();
                    _manager.StopAim();
                }
            }
        }

        //camera movement
        if (move.face == FACES.L)
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
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.OpenArmorWheel)
        {
            _arcadePlayerData.isArmorWheelDisplayed = true;
        }
        else if (playerAction is PlayerActions.CloseArmorWheel)
        {
            _arcadePlayerData.isArmorWheelDisplayed = false;
        }
        else if (playerAction is PlayerActions.ChangeInputMode &&
                 _playerValues.GetCurrentInput() is CurrentInput.ArcadeMechanics)
        {
            UpdateTutorial();
        }
    }

    private void UpdateTutorial()
    {
        guiManager.ShowTutorial();
        guiManager.SetTutorial(
            _playerNewInputs.GearUpText() + "- Gear Up |" + _playerNewInputs.GearDownText() + "- Gear Down |" +
            _playerNewInputs.CamMovementText() + "- Camera |" + _playerNewInputs.LightsText() + "- Lights |" +
            _playerNewInputs.AimText() + "- Aim |" + _playerNewInputs.ShootText() + "- Shoot |" +
            _playerNewInputs.ArmorWheelText() +
            "- Armor Wheel |" + _playerNewInputs.GrenadeDistractionText() + "- Grenade");
    }
}