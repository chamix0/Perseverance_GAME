using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MyDevices
{
    GamePad,
    KeyBoard,
    Cube
}

public class PlayerNewInputs : MonoBehaviour
{
    private Controls _controls;
    [SerializeField] private PlayerInput _playerInput;
    public MyDevices currentDevice;
    private bool InputChanged;

    private void Awake()
    {
        _controls = new Controls();
    }

    #region Enable

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    public void EnableControls()
    {
        _controls.Enable();
    }

    public void DisableControls()
    {
        _controls.Disable();
    }

    #endregion

    #region device control

    public bool CheckInputChanged()
    {
        if (InputChanged)
        {
            InputChanged = false;
            return true;
        }

        return false;
    }

    public void SetCurrentDevice(MyDevices device)
    {
        currentDevice = device;
    }

    public void UpdateDevice()
    {
        InputChanged = true;
        currentDevice = _playerInput.currentControlScheme == "KeyBoard" ? MyDevices.KeyBoard : MyDevices.GamePad;
    }

    public void SetCubeAsDevice()
    {
        InputChanged = true;
        currentDevice = MyDevices.Cube;
    }

    #endregion

    #region Eddo movement and actions

    public bool GearUp()
    {
        return _controls.Eddo.GearUp1.WasPressedThisFrame();
    }

    public bool GearDown()
    {
        return _controls.Eddo.GearDown1.WasPressedThisFrame();
    }

    public bool GearUpPressed()
    {
        return _controls.Eddo.GearUp1.IsPressed();
    }

    public bool GearDownPressed()
    {
        return _controls.Eddo.GearDown1.IsPressed();
    }

    public bool GearLeftPressed()
    {
        return _controls.Eddo.GearLeft.IsPressed();
    }

    public bool GearRightPressed()
    {
        return _controls.Eddo.GearRight.IsPressed();
    }

    public Vector2 GetMovementAxis()
    {
        return _controls.Eddo.Movement.ReadValue<Vector2>();
    }

    public bool GetRun()
    {
        return _controls.Eddo.Run.IsPressed();
    }

    public bool GetTurbo()
    {
        return _controls.Eddo.Turbo.IsPressed();
    }

    public bool ShootAutomatic()
    {
        return _controls.Eddo.Shoot.IsPressed();
    }

    public bool Shoot()
    {
        return _controls.Eddo.Shoot.WasPressedThisFrame();
    }

    public bool ShootReleased()
    {
        return _controls.Eddo.Shoot.WasReleasedThisFrame();
    }

    public bool Aim()
    {
        return _controls.Eddo.Aim.WasPressedThisFrame();
    }

    public bool AimRelease()
    {
        return _controls.Eddo.Aim.WasReleasedThisFrame();
    }

    public bool GrenadeDistraction()
    {
        return _controls.Eddo.Grenade.WasPressedThisFrame();
    }

    public bool ShowArmorWheel()
    {
        return _controls.Eddo.ArmorWheel.WasPressedThisFrame();
    }

    public bool HideArmorWheel()
    {
        return _controls.Eddo.ArmorWheel.WasReleasedThisFrame();
    }

    public bool HideArmorPressed()
    {
        return _controls.Eddo.ArmorWheel.IsPressed();
    }

    public bool ChangeWeapon()
    {
        return ShowArmorWheel();
    }

    public bool Pause()
    {
        return _controls.Eddo.Pause.WasPressedThisFrame();
    }

    public bool PlayerSelect()
    {
        return _controls.Eddo.Select.WasPressedThisFrame();
    }

    public bool Jump()
    {
        return _controls.Eddo.Jump.WasPressedThisFrame();
    }

    public bool Lights()
    {
        return _controls.Eddo.Lights.WasPressedThisFrame();
    }

    public bool PrevBulletType()
    {
        return _controls.Eddo.Prevbullettype.WasPressedThisFrame();
    }

    public bool NextBulletType()
    {
        return _controls.Eddo.Nextbullettype.WasPressedThisFrame();
    }

    public bool ChangeShootingMode()
    {
        return _controls.Eddo.ChangeShootingMode.WasPressedThisFrame();
    }

    public bool ChangeGrenade()
    {
        return _controls.Eddo.ChangeGrenade.WasPressedThisFrame();
    }

    public Vector2 CameraAxis()
    {
        float x = _controls.Eddo.RotationX.ReadValue<float>();
        float y = _controls.Eddo.RotationY.ReadValue<float>();
        return new Vector2(x, y);
    }

    #endregion

    #region EDDO actions text

    public string NextBulletTypeText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> R D-pad </color>",
            MyDevices.KeyBoard => "<color=white> 2 </color>",
            MyDevices.Cube => "<color=white>  </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string PrevBulletTypeText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> L D-pad </color>",
            MyDevices.KeyBoard => "<color=white> 1 </color>",
            MyDevices.Cube => "<color=white>  </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ChangeShootModeText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> U D-pad </color>",
            MyDevices.KeyBoard => "<color=white> LShift </color>",
            MyDevices.Cube => "<color=white>  </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public string RunText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> L3 </color>",
            MyDevices.KeyBoard => "<color=white> LShift </color>",
            MyDevices.Cube => "<color=white>  </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public string ChangeGrenadeText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> D D-pad </color>",
            MyDevices.KeyBoard => "<color=white> 3 </color>",
            MyDevices.Cube => "<color=white>  </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string PauseText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> Start </color>",
            MyDevices.KeyBoard => "<color=white> Esc </color>",
            MyDevices.Cube => "<color=white> T </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string PlayerSelectText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#5bbf30> A </color>",
            MyDevices.KeyBoard => "<color=white> E </color>",
            MyDevices.Cube => "<color=white> D </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string GearUpText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick UP </color>",
            MyDevices.KeyBoard => "<color=white> W </color>",
            MyDevices.Cube => "<color=white> R </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public string MovementText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick </color>",
            MyDevices.KeyBoard => "<color=white> WASD </color>",
            MyDevices.Cube => "<color=white> RR' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public string GearDownText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick Down </color>",
            MyDevices.KeyBoard => "<color=white> S </color>",
            MyDevices.Cube => "<color=white> R' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string CamMovementText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> RStick </color>",
            MyDevices.KeyBoard => "<color=white> Mouse </color>",
            MyDevices.Cube => "<color=white> L & U </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string LightsText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> R3 </color>",
            MyDevices.KeyBoard => "<color=white> L </color>",
            MyDevices.Cube => "<color=white> B </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string AimText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LT </color>",
            MyDevices.KeyBoard => "<color=white> Mouse R Click </color>",
            MyDevices.Cube => "<color=white> F' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ShootText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> RT </color>",
            MyDevices.KeyBoard => "<color=white> Mouse L Click </color>",
            MyDevices.Cube => "<color=white> F </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public string TurboText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> B </color>",
            MyDevices.KeyBoard => "<color=white> Space </color>",
            MyDevices.Cube => "<color=white> R </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ArmorWheelText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#c5d11d> Y </color>",
            MyDevices.KeyBoard => "<color=white> Mouse TAB Click </color>",
            MyDevices.Cube => "<color=white> D' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string GrenadeDistractionText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LB </color>",
            MyDevices.KeyBoard => "<color=white> Q </color>",
            MyDevices.Cube => "<color=white> M </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion

    #region Rotating wall actions

    public bool RotateWallClockWise()
    {
        return _controls.RotatingWall.RotateCW.WasPressedThisFrame();
    }

    public bool RotateWallCounterClockWise()
    {
        return _controls.RotatingWall.RotateCCW.WasPressedThisFrame();
    }

    public bool ExitWall()
    {
        return _controls.RotatingWall.Exit.WasPressedThisFrame();
    }

    #endregion

    #region rotating wall texts

    public string RotateCWText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LT </color>",
            MyDevices.KeyBoard => "<color=white> A </color>",
            MyDevices.Cube => "<color=white> D </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string RotateCCWText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> RT </color>",
            MyDevices.KeyBoard => "<color=white> D </color>",
            MyDevices.Cube => "<color=white> D' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ExitWallText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#ad233e> B </color>",
            MyDevices.KeyBoard => "<color=white> E </color>",
            MyDevices.Cube => "<color=white> R' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion

    #region Basic actions

    public bool UpTap()
    {
        return _controls.Basic.Up.WasPressedThisFrame();
    }

    public bool DownTap()
    {
        return _controls.Basic.Down.WasPressedThisFrame();
    }

    public bool LeftTap()
    {
        return _controls.Basic.Left.WasPressedThisFrame();
    }

    public bool RightTap()
    {
        return _controls.Basic.Right.WasPressedThisFrame();
    }

    public bool UpHold()
    {
        return _controls.Basic.Up.IsPressed();
    }

    public bool DownHold()
    {
        return _controls.Basic.Down.IsPressed();
    }

    public bool LeftHold()
    {
        return _controls.Basic.Left.IsPressed();
    }

    public bool RightHold()
    {
        return _controls.Basic.Right.IsPressed();
    }

    public bool SelectBasic()
    {
        return _controls.Basic.Select.WasPressedThisFrame();
    }

    public bool ReturnBasic()
    {
        return _controls.Basic.Return.WasPressedThisFrame();
    }

    public bool EnableLoad()
    {
        return _controls.Basic.EnableLoad.WasPressedThisFrame();
    }

    public bool EnableErase()
    {
        return _controls.Basic.EnableErase.WasPressedThisFrame();
    }

    #endregion

    #region basic actions texts

    public string EraseText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> Start </color>",
            MyDevices.KeyBoard => "<color=white> E </color>",
            MyDevices.Cube => "<color=white> D </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string LoadText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> Select </color>",
            MyDevices.KeyBoard => "<color=white> L </color>",
            MyDevices.Cube => "<color=white> D' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string UpText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick Up </color>",
            MyDevices.KeyBoard => "<color=white> W </color>",
            MyDevices.Cube => "<color=white> R </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string AllDirectionsText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick </color>",
            MyDevices.KeyBoard => "<color=white> WASD </color>",
            MyDevices.Cube => "<color=white> R & U </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string DownText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick Down </color>",
            MyDevices.KeyBoard => "<color=white> S </color>",
            MyDevices.Cube => "<color=white> R' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string RightText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick Right </color>",
            MyDevices.KeyBoard => "<color=white> D </color>",
            MyDevices.Cube => "<color=white> U </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string LeftText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LStick Left </color>",
            MyDevices.KeyBoard => "<color=white> A </color>",
            MyDevices.Cube => "<color=white> U' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string SelectBasicText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#5bbf30> A </color>",
            MyDevices.KeyBoard => "<color=white> Enter </color>",
            MyDevices.Cube => "<color=white> F </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ExitBasicText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#ad233e> B </color>",
            MyDevices.KeyBoard => "<color=white> Q </color>",
            MyDevices.Cube => "<color=white> B </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion

    #region Miniboss Actions

    public bool AttackMiniBoss()
    {
        return _controls.Miniboss.Attack.WasPressedThisFrame();
    }

    public bool SpecialAttackMiniBoss()
    {
        return _controls.Miniboss.SpecialAttack.WasPressedThisFrame();
    }

    public bool DefendMiniBoss()
    {
        return _controls.Miniboss.Defend.WasPressedThisFrame();
    }

    public bool SpecialDefendMiniBoss()
    {
        return _controls.Miniboss.SpecialDefense.WasPressedThisFrame();
    }

    #endregion

    #region Miniboss Texts

    public string AttackText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> RB </color>",
            MyDevices.KeyBoard => "<color=white> W </color>",
            MyDevices.Cube => "<color=white> R </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string DefendText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LB </color>",
            MyDevices.KeyBoard => "<color=white> Q </color>",
            MyDevices.Cube => "<color=white> L </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string SpecialAttackText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> RT </color>",
            MyDevices.KeyBoard => "<color=white> S </color>",
            MyDevices.Cube => "<color=white> R' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string SpecialDefenseText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LT </color>",
            MyDevices.KeyBoard => "<color=white> A </color>",
            MyDevices.Cube => "<color=white> L' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion

    #region Minigames

    public bool Sprint()
    {
        return _controls.Minigames.Sprint.IsPressed();
    }

    public bool RollClockWise()
    {
        return _controls.Minigames.RollCW.WasPressedThisFrame();
    }

    public bool RollCounterClockWise()
    {
        return _controls.Minigames.RollCCW.WasPressedThisFrame();
    }

    public bool ReRoll()
    {
        return _controls.Minigames.Reroll.WasPressedThisFrame();
    }

    #endregion

    #region minigames text

    public string RollCWText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> RT </color>",
            MyDevices.KeyBoard => "<color=white> D </color>",
            MyDevices.Cube => "<color=white> F </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string RollCCWText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=white> LT </color>",
            MyDevices.KeyBoard => "<color=white> A </color>",
            MyDevices.Cube => "<color=white> F' </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ReRollText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#1c5abd> X </color>",
            MyDevices.KeyBoard => "<color=white> R </color>",
            MyDevices.Cube => "<color=white> L </color>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion
}