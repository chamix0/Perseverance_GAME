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
    private PlayerValues _playerValues;
    private PlayerMechanicsArcadeManager _manager;
    private ArcadePlayerData _arcadePlayerData;
    private CameraChanger _cameraChanger;
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
        _playerValues = FindObjectOfType<PlayerValues>();
        _arcadePlayerData = FindObjectOfType<ArcadePlayerData>();
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
                if (Input.GetKeyDown(KeyCode.W))
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
                if (Input.GetKeyDown(KeyCode.S))
                {
                    _playerValues.DecreaseGear();
                    turboParticles.Stop();
                    _playerValues.CheckIfStuck(true);
                }

                //shoot
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (shootTimer.Elapsed.TotalMilliseconds > _arcadePlayerData.GetShootingCooldown())
                    {
                        shootTimer.Restart();
                        _manager.Shoot();
                    }
                }

                //stop  shooting
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    _manager.StopAutomaticShooting();
                }

                //aim
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    _manager.Aim();
                }
                else if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    _manager.StopAim();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _manager.ThrowGrenade();
                }

                //open armor wheel
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    guiManager.ShowArmorWheel();
                    _manager.StopAutomaticShooting();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                guiManager.HideArmorWheel();
                _manager.StopAutomaticShooting();
                _manager.StopAim();
            }


            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     foreach (var machineGun in machineGuns)
            //     {
            //         machineGun.NextShootingMode();
            //     }
            // }
            //
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     foreach (var machineGun in machineGuns)
            //     {
            //         machineGun.PrevShootingMode();
            //     }
            // }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
            }
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
    }
}