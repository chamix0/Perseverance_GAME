using System.Diagnostics;
using Mechanics.General_Inputs;
using Mechanics.General_Inputs.Machine_gun_mode;
using Player.Observer_pattern;
using UnityEngine;
using UTILS;

public class PlayerMechanicsArcadeInputs : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    private PlayerNewInputs _playerNewInputs;
    private PlayerValues _playerValues;
    private PlayerMechanicsArcadeManager _manager;
    private ArcadePlayerData _arcadePlayerData;
    private CameraController _cameraController;
    private GuiManager guiManager;
    private ArmorWheel _armorWheel;

    //run mode
    private Stamina stamina;
    private bool running = false;

    [SerializeField] private ParticleSystem turboParticles;

    //shoot stuff
    private MyStopWatch shootTimer;

    private void Awake()
    {
        shootTimer = gameObject.AddComponent<MyStopWatch>();
        shootTimer.StartStopwatch();
    }

    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        guiManager.AddObserver(this);
        _playerNewInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _arcadePlayerData = FindObjectOfType<ArcadePlayerData>();
        _cameraController = FindObjectOfType<CameraController>();
        stamina = FindObjectOfType<Stamina>();
        _manager = FindObjectOfType<PlayerMechanicsArcadeManager>();
        _armorWheel = FindObjectOfType<ArmorWheel>();
        _playerValues.AddObserver(this);
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
        {
            _manager.StopAim();
            _manager.StopAutomaticShooting();
            _manager.HideMachineGuns();
        }

        if (_playerValues.GetPaused())
            _manager.StopAutomaticShooting();

        if (_playerValues.GetCurrentInput() == CurrentInput.ArcadeMechanics && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_playerNewInputs.CheckInputChanged())
                UpdateTutorial();

            if (!_arcadePlayerData.isArmorWheelDisplayed)
            {
                //common actions
                //get movement vector
                Vector2 movementVector = Vector2.zero;
                Vector2 movementVectorGamepad = _playerNewInputs.GetMovementAxis();
                if (_playerNewInputs.GearUpPressed())
                    movementVector.y = 1;
                if (_playerNewInputs.GearDownPressed())
                    movementVector.y = -1;
                if (_playerNewInputs.GearLeftPressed())
                    movementVector.x = -1;
                else if (_playerNewInputs.GearRightPressed())
                    movementVector.x = 1;

                float magnitude = movementVector.magnitude + movementVectorGamepad.magnitude;

                if (magnitude > 0)
                {
                    if (_arcadePlayerData.unlockedGear == 4 && _playerNewInputs.GetTurbo())
                    {
                        _playerValues.SetGear(4);
                    }
                    else
                    {
                        if (_playerNewInputs.GetRun() && _playerValues.gampadAddedSpeed > 0.5f)
                        {
                            running = true;
                            _playerValues.SetGear(3);
                        }
                        else
                        {
                            if (running)
                            {
                                if (_playerNewInputs.currentDevice is not MyDevices.GamePad ||
                                    _playerValues.gampadAddedSpeed < 0.5f)
                                {
                                    running = false;
                                    _playerValues.SetGear(2);
                                }
                            }
                            else
                            {
                                _playerValues.SetGear(2);
                            }
                        }
                    }

                    if (_playerValues.GetGear() == 4)
                        turboParticles.Play();
                    else
                        turboParticles.Stop();
                    _playerValues.CheckIfStuck(true);
                }

                else
                {
                    running = false;
                    _playerValues.SetGear(1);
                }


                //shoot
                if (_arcadePlayerData.GetShootingMode() is ShootingMode.Automatic)
                {
                    if (_playerNewInputs.ShootAutomatic())
                    {
                        if (shootTimer.GetElapsedMiliseconds() > _arcadePlayerData.GetShootingCooldown())
                        {
                            shootTimer.Restart();
                            _manager.Shoot();
                        }
                    }
                }
                else
                {
                    if (_playerNewInputs.Shoot())
                    {
                        if (shootTimer.GetElapsedMiliseconds() > _arcadePlayerData.GetShootingCooldown())
                        {
                            shootTimer.Restart();
                            _manager.Shoot();
                        }
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
                    _cameraController.SlowCamera(3);
                    _manager.Aim();
                }

                //stop aim
                if (_playerNewInputs.AimRelease())
                {
                    _cameraController.NormalCameraSpeed();
                    _manager.StopAim();
                }

                //grenade
                if (_playerNewInputs.GrenadeDistraction())
                {
                    _manager.ThrowGrenade();
                }

                if (_playerNewInputs.NextBulletType())
                    _armorWheel.SelectNextBullets();


                if (_playerNewInputs.PrevBulletType())
                    _armorWheel.SelectPrevBullets();


                if (_playerNewInputs.ChangeShootingMode())
                    _armorWheel.SelectNextShootingMode();


                if (_playerNewInputs.ChangeGrenade())
                    _armorWheel.SelectNextGrenade();


                //open armor wheel
                if (_playerNewInputs.ShowArmorWheel())
                {
                    guiManager.ShowArmorWheel();
                    _manager.StopAutomaticShooting();
                }
            }
            else if (_playerNewInputs.HideArmorPressed() && _playerNewInputs.UpTap())
            {
                _armorWheel.SelectUp();
            }
            else if (_playerNewInputs.HideArmorPressed() && _playerNewInputs.DownTap())
            {
                _armorWheel.SelectDown();
            }
            else if (_playerNewInputs.HideArmorPressed() && _playerNewInputs.RightTap())
            {
                _armorWheel.SelectNext();
            }
            else if (_playerNewInputs.HideArmorPressed() && _playerNewInputs.LeftTap())
            {
                _armorWheel.SelectPrev();
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
        if (_playerValues.GetInputsEnabled())
        {
            _playerNewInputs.SetCubeAsDevice();
            if (_playerNewInputs.CheckInputChanged())
                UpdateTutorial();
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
                    if (shootTimer.GetElapsedMiliseconds() > _arcadePlayerData.GetShootingCooldown())
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
        if (_playerNewInputs.currentDevice is MyDevices.Cube)
        {
            guiManager.SetTutorial(
                _playerNewInputs.GearUpText() + "- Gear Up |" + _playerNewInputs.GearDownText() + "- Gear Down |" +
                _playerNewInputs.CamMovementText() + "- Camera |" + _playerNewInputs.LightsText() + "- Lights |" +
                _playerNewInputs.AimText() + "- Aim |" + _playerNewInputs.ShootText() + "- Shoot |" +
                _playerNewInputs.ArmorWheelText() +
                "- Armor Wheel |" + _playerNewInputs.GrenadeDistractionText() + "- Grenade |" +
                _playerNewInputs.NextBulletTypeText() + "/" + _playerNewInputs.PrevBulletTypeText() +
                "- bullet type |" +
                _playerNewInputs.ChangeGrenadeText() + "- Grenade type |" + _playerNewInputs.ChangeShootModeText() +
                "- Shoot mode");
        }
        else
        {
            guiManager.SetTutorial(
                _playerNewInputs.MovementText() + "- Movement |" + _playerNewInputs.CamMovementText() + "- Camera |" +
                _playerNewInputs.LightsText() + "- Lights |" +
                _playerNewInputs.AimText() + "- Aim |" + _playerNewInputs.ShootText() + "- Shoot |" +
                _playerNewInputs.TurboText() + "- Turbo |" +
                _playerNewInputs.ArmorWheelText() +
                "- Armor Wheel |" + _playerNewInputs.GrenadeDistractionText() + "- Grenade |" +
                _playerNewInputs.NextBulletTypeText() + "/" + _playerNewInputs.PrevBulletTypeText() +
                "- bullet type |" +
                _playerNewInputs.ChangeGrenadeText() + "- Grenade type |" + _playerNewInputs.ChangeShootModeText() +
                "- Shoot mode");
        }
    }
}