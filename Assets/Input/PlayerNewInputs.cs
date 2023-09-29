using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        _controls = new Controls();
    }

    #region eneble

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    #endregion

    #region device control

    public void SetCurrentDevice(MyDevices device)
    {
        currentDevice = device;
    }

    public void UpdateDevice()
    {
        currentDevice = _playerInput.currentControlScheme == "KeyBoard" ? MyDevices.KeyBoard : MyDevices.GamePad;
    }

    public void SetCubeAsDevice()
    {
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

    public Vector2 CameraAxis()
    {
        float x = _controls.Eddo.RotationX.ReadValue<float>();
        float y = _controls.Eddo.RotationY.ReadValue<float>();
        return new Vector2(x, y);
    }

    #endregion

    #region EDDO actions text

    public string PlayerSelectText()
    {
        return currentDevice switch
        {
            MyDevices.GamePad => "<color=#5bbf30> A </color>",
            MyDevices.KeyBoard => " E ",
            MyDevices.Cube => " F ",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion
}