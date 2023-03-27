using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;
using Debug = UnityEngine.Debug;

[DefaultExecutionOrder(1)]
public class PlayerValues : Subject
{
    #region DATA

    //status values
    [Header("VALUES")] [SerializeField] private List<float> movementSpeeds;
    [SerializeField] [Range(0, 4)] private int gear = 1;
    private bool isGrounded, lightsOn, cameraIsFreezed, stucked;
    private bool canMove;

    //stamina values
    public float stamina, maxStamina;
    public int lives = 3;

    //stuck values
    private Stopwatch stuckTimer;
    private Vector3 stuckedPos;
    private Vector3 lastValidPos;
    public float stuckTime = 3f;

    //compontes
    [Header("COMPONENTES")] [NonSerialized]
    public Rigidbody _rigidbody;

    private PlayerLights _playerLights;
    [NonSerialized] public Camera mainCamera;
    [NonSerialized] public bool allStaminaUsed = false;
    [NonSerialized] public RigidbodyConstraints _originalRigidBodyConstraints;
    private PlayerAnimations _playerAnimations;

    [SerializeField] private Transform isgroundedPos;

    //Inputs
    private CurrentInput _currentInput;
    private bool _inputsEnabled;

    //stealth mode
 [SerializeField]   private bool _distractionEnabled;

    //save data
    private JSONsaving _jsoNsaving;
    private SaveData _saveData;
    [NonSerialized] public GameData gameData;

    //variables
    private bool _updateSnap, _updateLookAt, moveForward;


    private float _snapPosX,
        _snapPosY,
        _snapPosZ,
        _targetPosX,
        _targetPosY,
        _targetPosZ,
        _tX,
        _tY,
        _tZ,
        _targetAngle;


    //is grounded
    [Header("Grounded stuff")] public float raySize;
    public Vector3 RayOffset;
    [SerializeField] private LayerMask colisionLayers;

    //animator parameters
    private static readonly int Gear = Animator.StringToHash("Gear");

    #endregion

    private void Awake()
    {
        _currentInput = CurrentInput.Movement;
        _inputsEnabled = true;
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerLights = FindObjectOfType<PlayerLights>();
        mainCamera = Camera.main;
        //observers
        AddObserver(_playerAnimations);
        AddObserver(_playerLights);
        //save data
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _saveData = _jsoNsaving._saveData;
        gameData = _saveData.GetGameData(_saveData.GetLastSessionSlotIndex());
        _originalRigidBodyConstraints = _rigidbody.constraints;
        stuckTimer = new Stopwatch();
    }

    private void Start()
    {
        SetCanMove(true);
    }

    private void Update()
    {
        CheckIfGrounded();
        if (_updateSnap)
        {
            transform.position = UpdateSnapPosition();
        }

        if (_updateLookAt)
        {
            UpdateSnapAngle();
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(isgroundedPos.position + RayOffset,
            transform.TransformDirection(Vector3.down) * raySize, Color.red);
    }

    public void SetCurrentInput(CurrentInput currentInput)
    {
        _currentInput = currentInput;
    }

    public CurrentInput GetCurrentInput()
    {
        return _currentInput;
    }

    public bool GetInputsEnabled()
    {
        return _inputsEnabled;
    }

    public void SetInputsEnabled(bool val)
    {
        _inputsEnabled = val;
    }

    #region GEAR

    public int GetGear()
    {
        return gear;
    }

    public void RiseGear()
    {
        if (isGrounded && canMove)
        {
            ResetRigidBodyConstraints();
            int old = gear;
            if (allStaminaUsed)
                gear = Mathf.Min(gear + 1, 3);
            else
                gear = Mathf.Min(gear + 1, 4);
            _playerAnimations.ChangeGearAnim(old, gear);
            NotifyObservers(PlayerActions.RiseGear);
        }
        else
            SetGear(1);
    }

    public void DecreaseGear()
    {
        if (isGrounded && canMove)
        {
            ResetRigidBodyConstraints();
            int old = gear;
            gear = Mathf.Max(gear - 1, 0);
            _playerAnimations.ChangeGearAnim(old, gear);
            NotifyObservers(PlayerActions.DecreaseGear);
        }
        else
        {
            SetGear(1);
        }
    }

    public void SetGear(int value)
    {
        if (isGrounded)
        {
            int old = gear;
            gear = Mathf.Clamp(value, 0, 4);
            _playerAnimations.ChangeGearAnim(old, gear);
        }
        else
        {
            int old = gear;
            gear = 1;
            _playerAnimations.ChangeGearAnim(old, gear);
        }
    }

    #endregion

    #region Movement

    public void SetMoveForward(bool val) => moveForward = val;

    public void StopMovement() => SetGear(1);

    public float GetSpeed()
    {
        return movementSpeeds[gear];
    }

    public bool GetMoveForward()
    {
        return moveForward;
    }

    public void SetCanMove(bool val)
    {
        canMove = val;
        SetGear(1);
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public void SnapPositionTo(Vector3 pos)
    {
        _targetPosX = pos.x;
        _targetPosY = pos.y;
        _targetPosZ = pos.z;
        _tX = 0.0f;
        _tY = 0.0f;
        _tZ = 0.0f;
        _updateSnap = true;
    }

    public void snapRotationTo(float angle)
    {
        _targetAngle = angle;
        _updateLookAt = true;
    }

    private Vector3 UpdateSnapPosition()
    {
        Vector3 position = transform.position;
        _snapPosX = Mathf.Lerp(position.x, _targetPosX, _tX);
        _snapPosY = Mathf.Lerp(position.y, _targetPosY, _tY);
        _snapPosZ = Mathf.Lerp(position.z, _targetPosZ, _tZ);

        _tX += 0.5f * Time.deltaTime;
        _tY += 0.5f * Time.deltaTime;
        _tZ += 0.5f * Time.deltaTime;

        if (_tX > 1.0f && _tY > 1.0f && _tZ > 1.0f)
        {
            _tX = 1.0f;
            _tY = 1.0f;
            _tZ = 1.0f;
            _updateSnap = false;
        }

        return new Vector3(_snapPosX, _snapPosY, _snapPosZ);
    }

    private void StopRigidBodyVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void UpdateSnapAngle()
    {
        transform.rotation =
            Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, MyUtils.Clamp0360(_targetAngle), 0),
                Time.deltaTime * 5f);

        if (Mathf.Abs(transform.eulerAngles.y - MyUtils.Clamp0360(_targetAngle)) < 0.01f)
        {
            _updateLookAt = false;
        }
    }

    #endregion

    #region Stealth

    public bool GetDistractionEnabled()
    {
        return _distractionEnabled;
    }

    public void SetDistractionEnabled(bool val)
    {
        _distractionEnabled = val;
    }

    #endregion

    #region ACTIONS

    public void Sit()
    {
        StopRigidBodyVelocity();
        SetInputsEnabled(false);
        SetCanMove(false);
        NotifyObservers(PlayerActions.Sit);
    }

    public void StandUp(bool inputs, float time)
    {
        NotifyObservers(PlayerActions.StandUp);
        StartCoroutine(StandUpCoroutine(inputs, time));
    }

    public void Die(Vector3 spawnPos)
    {
        NotifyObservers(PlayerActions.Die);
        StopRigidBodyVelocity();
        transform.rotation = default;
        TpToLastCheckPoint(spawnPos);
    }


    IEnumerator StandUpCoroutine(bool inputs, float time)
    {
        yield return new WaitForSeconds(time);
        SetCurrentInput(CurrentInput.Movement);
        SetCanMove(true);
        SetInputsEnabled(inputs);
    }

    #endregion

    #region Grounded and stuck

    public void CheckIfStuck()
    {
        if (!stuckTimer.IsRunning)
        {
            stuckTimer.Start();
            stucked = false;
        }

        if (!isGrounded && (transform.position - stuckedPos).magnitude < 0.01f)
        {
            if (stuckTimer.Elapsed.TotalSeconds > stuckTime)
            {
                stucked = true;
                ResetPos();
            }
        }
    }

    public void ResetRigidBodyConstraints()
    {
        if (isGrounded)
        {
            _rigidbody.constraints = _originalRigidBodyConstraints;
        }
    }

    public void ResetPos()
    {
        //añadir animación de respawn
        stuckTimer.Reset();
        SetGear(1);
        stucked = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position,
                Vector3.down, out hit, Single.PositiveInfinity))
        {
            if (MyUtils.IsInLayerMask(hit.transform.gameObject, colisionLayers))
            {
                transform.position = hit.point;
            }
            else
            {
                transform.position = lastValidPos;
            }
        }

        transform.up = Vector3.up;
    }

    public void TpToLastCheckPoint(Vector3 position)
    {
        transform.position = position;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public bool GetIsStucked()
    {
        return stucked;
    }

    private void CheckIfGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(isgroundedPos.position + RayOffset,
                transform.TransformDirection(Vector3.down), out hit, raySize))
        {
            if (MyUtils.IsInLayerMask(hit.transform.gameObject, colisionLayers))
            {
                if (!isGrounded)
                {
                    SetCanMove(true);
                    isGrounded = true;
                    _rigidbody.constraints = _originalRigidBodyConstraints;
                }

                lastValidPos = transform.position;
                stuckTimer.Restart();
            }
            else
            {
                if (isGrounded)
                {
                    SetGear(1);
                    SetCanMove(false);
                    _rigidbody.constraints = RigidbodyConstraints.None;
                    isGrounded = false;
                }

                stuckedPos = transform.position;
            }
        }
        else
        {
            if (isGrounded)
            {
                SetCanMove(false);
                _rigidbody.constraints = RigidbodyConstraints.None;
                isGrounded = false;
            }

            stuckedPos = transform.position;
        }
    }

    #endregion

    #region Lights

    public void TurnOnLights()
    {
        NotifyObservers(PlayerActions.TurnOnLights);
        lightsOn = true;
    }

    public void TurnOffLights()
    {
        NotifyObservers(PlayerActions.TurnOffLights);
        lightsOn = false;
    }

    public bool GetLights()
    {
        return lightsOn;
    }

    #endregion
}