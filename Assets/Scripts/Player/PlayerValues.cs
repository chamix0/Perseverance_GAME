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

    public Vector3 auxDir;

    //status values
    [Header("VALUES")] [SerializeField] private List<float> movementSpeeds;
    [SerializeField] [Range(0, 4)] private int gear = 1;
    private bool isGrounded, lightsOn, cameraIsFreezed, stucked;
    private bool canMove;
    [SerializeField] private bool isArcade;

    //stamina values
    public float stamina, maxStamina;
    public int lives;
    public int MaxLives = 4;

    //stuck values
    private Stopwatch stuckTimer;
    private Vector3 stuckedPos, prevStuckedPos;
    private Vector3 lastValidPos;
    public float stuckTime = 3f;

    //compontes
    [Header("COMPONENTES")] [NonSerialized]
    public Rigidbody _rigidbody;

    public GenericScreenUi genericScreenUi;
    [NonSerialized] public bool allStaminaUsed = false;
    [NonSerialized] public RigidbodyConstraints _originalRigidBodyConstraints;
    [SerializeField] private Transform isgroundedPos;

    //observers
    private PlayerAnimations _playerAnimations;
    private PlayerLights _playerLights;
    private PlayerSounds _playerSounds;

    //Inputs
    [SerializeField] private CurrentInput _currentInput;
    private bool _inputsEnabled;

    //save data
    private JSONsaving _jsoNsaving;
    [NonSerialized] public SaveData _saveData;
    [NonSerialized] public GameData gameData;

    //variables
    private bool _updateSnap, _updateLookAt, moveForward;
    [NonSerialized] public bool dead = false;
    private float _targetAngle;
    private Vector3 _targetPos;
    private bool paused;

    //is grounded
    [Header("Grounded stuff")] public float raySize;
    public Vector3 RayOffset;
    [SerializeField] private LayerMask colisionLayers;

    //animator parameters
    private static readonly int Gear = Animator.StringToHash("Gear");
    private DissolveMaterials dissolveMaterials;

    //newInputSystem

    #endregion

    private void Awake()
    {
        _inputsEnabled = true;
        _rigidbody = GetComponent<Rigidbody>();
        genericScreenUi = GetComponent<GenericScreenUi>();
        //observers
        _playerSounds = FindObjectOfType<PlayerSounds>();
        _playerLights = FindObjectOfType<PlayerLights>();
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        AddObserver(_playerSounds);
        AddObserver(_playerAnimations);
        AddObserver(_playerLights);
        //save data
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _saveData = _jsoNsaving._saveData;
        gameData = _saveData.GetGameData(_saveData.GetLastSessionSlotIndex());
        _originalRigidBodyConstraints = _rigidbody.constraints;
        stuckTimer = new Stopwatch();
        dissolveMaterials = GetComponent<DissolveMaterials>();
    }

    private void Start()
    {
        prevStuckedPos = transform.position;
        lives = MaxLives;
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

    public void SaveGame()
    {
        _jsoNsaving.SaveTheData();
    }

    public void SetCurrentInput(CurrentInput currentInput)
    {
        _currentInput = currentInput;
        NotifyObservers(PlayerActions.ChangeInputMode);
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

    private void NotifyMovementAction()
    {
        if (gear == 0)
        {
            NotifyAction(PlayerActions.SlowWalking);
        }
        else if (gear == 2)
        {
            NotifyAction(PlayerActions.Walking);
        }
        else if (gear == 1)
        {
            NotifyAction(PlayerActions.Stop);
        }
        else if (gear == 3)
        {
            NotifyAction(PlayerActions.Runing);
        }
        else if (gear == 4)
        {
            NotifyAction(PlayerActions.Sprint);
        }
    }

    public void RiseGear()
    {
        if (isGrounded && canMove)
        {
            ResetRigidBodyConstraints();
            int old = gear;
            if (allStaminaUsed)
            {
                gear = Mathf.Min(gear + 1, 3);
                if (old + 1 <= 3)
                    NotifyObservers(PlayerActions.RiseGear);
            }
            else
            {
                gear = Mathf.Min(gear + 1, 4);
                if (old + 1 <= 4)
                    NotifyObservers(PlayerActions.RiseGear);
            }

            _playerAnimations.ChangeGearAnim(old, gear);
        }
        else
        {
            SetGear(1);
        }

        NotifyMovementAction();
    }

    public void DecreaseGear()
    {
        if (isGrounded && canMove)
        {
            ResetRigidBodyConstraints();
            int old = gear;
            gear = Mathf.Max(gear - 1, 0);
            _playerAnimations.ChangeGearAnim(old, gear);
            if (old - 1 >= 0)
                NotifyObservers(PlayerActions.DecreaseGear);
        }
        else
        {
            SetGear(1);
        }

        NotifyMovementAction();
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

        NotifyMovementAction();
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
        _targetPos = pos;
        _updateSnap = true;
        _rigidbody.useGravity = false;
    }

    Quaternion currentRotation;

    public void snapRotationTo(float angle)
    {
        _rigidbody.useGravity = false;
        _targetAngle = angle;
        currentRotation = transform.rotation;
        _updateLookAt = true;
    }

    private Vector3 UpdateSnapPosition()
    {
        Vector3 position = transform.position;
        Vector3 newPos = Vector3.MoveTowards(position, _targetPos, 4f * Time.deltaTime);

        if (Vector3.Distance(newPos, _targetPos) < 0.1f)
        {
            _rigidbody.useGravity = true;
            _updateSnap = false;
        }

        return newPos;
    }

    private void StopRigidBodyVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void UpdateSnapAngle()
    {
        currentRotation =
            Quaternion.Slerp(currentRotation, Quaternion.Euler(0, MyUtils.Clamp0360(_targetAngle), 0),
                Time.deltaTime * 5f);
        transform.rotation = currentRotation;
        if (Mathf.Abs(MyUtils.Clamp0360(currentRotation.eulerAngles.y) - MyUtils.Clamp0360(_targetAngle)) < 0.1f)
        {
            _updateLookAt = false;
        }
    }

    #endregion


    #region ACTIONS

    public void StopAllMovement(bool inputsEnabled)
    {
        StopRigidBodyVelocity();
        SetInputsEnabled(inputsEnabled);
        SetCanMove(false);
        NotifyObservers(PlayerActions.Stop);
    }

    public void ContinueAllMovement()
    {
        StopRigidBodyVelocity();
        SetInputsEnabled(true);
        SetCanMove(true);
    }

    public void Sit()
    {
        StopRigidBodyVelocity();
        SetInputsEnabled(false);
        SetCanMove(false);
        NotifyObservers(PlayerActions.Sit);
    }

    public void StandUp(bool inputs, float time)
    {
        StartCoroutine(StandUpCoroutine(inputs, time));
    }

    public void StandUp(bool inputs, float time, CurrentInput newInput)
    {
        StartCoroutine(StandUpCoroutine(inputs, time, newInput));
    }

    public void AddLive()
    {
        lives = Mathf.Min(lives + 1, MaxLives);
    }

    public int GetMaxLives()
    {
        return MaxLives;
    }

    public void AddMaxLives(int value)
    {
        MaxLives += value;
    }

    public void RecieveDamage(Vector3 spawnPos)
    {
        if (lives > 0)
        {
            lives--;
            NotifyCameraLives();
            NotifyAction(PlayerActions.Damage);
            if (lives <= 0)
                Die(spawnPos);
        }
    }

    public void RecieveDamage(Vector3 spawnPos, int damage)
    {
        if (lives > 0)
        {
            lives -= damage;
            NotifyCameraLives();
            NotifyAction(PlayerActions.Damage);
            if (lives <= 0)
                Die(spawnPos);
        }
    }

    public void NotifyCameraLives()
    {
        if (lives == 1)
            NotifyAction(PlayerActions.HighDamage);
        else if (lives == 2)
            NotifyAction(PlayerActions.MediumDamage);
        else if (lives == 3)
            NotifyAction(PlayerActions.LowDamage);
        else if (lives > 3)
            NotifyAction(PlayerActions.NoDamage);
    }

    public void Die(Vector3 spawnPos)
    {
        dead = true;
        lives = MaxLives;
        NotifyCameraLives();
        NotifyAction(PlayerActions.Die);
        StopRigidBodyVelocity();
        // TpToLastCheckPoint(spawnPos);
        StartCoroutine(ResetPosCoroutine(spawnPos));
    }

    IEnumerator StandUpCoroutine(bool inputs, float time)
    {
        yield return new WaitForSeconds(time);
        NotifyObservers(PlayerActions.StandUp);
        yield return new WaitForSeconds(2f);
        if (_currentInput is not (CurrentInput.ShootMovement or CurrentInput.StealthMovement
            or CurrentInput.RaceMovement or CurrentInput.ArcadeMechanics))
        {
            if (!isArcade)
                SetCurrentInput(CurrentInput.Movement);
            else
                SetCurrentInput(CurrentInput.ArcadeMechanics);
        }

        SetCanMove(true);
        SetInputsEnabled(inputs);
    }

    IEnumerator StandUpCoroutine(bool inputs, float time, CurrentInput newInput)
    {
        yield return new WaitForSeconds(time);
        NotifyObservers(PlayerActions.StandUp);
        yield return new WaitForSeconds(2f);
        if (_currentInput is not (CurrentInput.ShootMovement or CurrentInput.StealthMovement
            or CurrentInput.RaceMovement or CurrentInput.ArcadeMechanics))
            SetCurrentInput(newInput);
        SetCanMove(true);
        SetInputsEnabled(inputs);
    }

    #endregion

    #region Grounded and stuck

    public void CheckIfStuck(bool resetPos)
    {
        if (!stuckTimer.IsRunning)
        {
            stuckTimer.Start();
            stucked = false;
        }

        if (!isGrounded && Vector3.Distance(stuckedPos, prevStuckedPos) < 0.01f)
        {
            if (stuckTimer.Elapsed.TotalSeconds > stuckTime)
            {
                stucked = true;
                if (resetPos)
                    ResetPos();
            }
        }
        else
        {
            prevStuckedPos = stuckedPos;
            stuckTimer.Restart();
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
        RaycastHit hit;
        if (Physics.Raycast(GetPos(),
                Vector3.down, out hit, Single.PositiveInfinity))
        {
            if (!hit.transform.gameObject.CompareTag("Stair") &&
                MyUtils.IsInLayerMask(hit.transform.gameObject, colisionLayers))
            {
                StartCoroutine(ResetPosCoroutine(hit.point));
            }
            else
            {
                StartCoroutine(ResetPosCoroutine(lastValidPos));
            }
        }
    }

    IEnumerator ResetPosCoroutine(Vector3 pos)
    {
        CurrentInput aux = _currentInput;
        genericScreenUi.FadeOutText();
        dissolveMaterials.DissolveOut();
        _currentInput = CurrentInput.Movement;
        Sit();
        yield return new WaitForSeconds(2);
        StandUp(true, 3f);
        transform.position = pos;
        transform.up = Vector3.up;
        dissolveMaterials.DissolveIn();
        genericScreenUi.FadeInText();
        dead = false;
        stucked = false;
        _currentInput = aux;
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
                transform.TransformDirection(Vector3.down), out hit, raySize, colisionLayers))
        {
            if (!isGrounded)
            {
                SetCanMove(true);
                isGrounded = true;
                stucked = false;
                if (transform.up == Vector3.up)
                    _rigidbody.constraints = _originalRigidBodyConstraints;
            }

            if (!hit.transform.gameObject.CompareTag("Stair"))
            {
                lastValidPos = transform.position;
                stuckTimer.Restart();
            }
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

    #endregion

    public void NotifyAction(PlayerActions action)
    {
        NotifyObservers(action);
    }

    #region Lights

    protected internal void TurnOnLights()
    {
        lightsOn = true;
    }

    protected internal void TurnOffLights()
    {
        lightsOn = false;
    }

    public bool GetLights()
    {
        return lightsOn;
    }

    public Vector3 GetPos()
    {
        Vector3 offset = new Vector3(0, 1, 0);
        return transform.position + offset;
    }

    public void SetPaused(bool value)
    {
        paused = value;
    }

    public bool GetPaused()
    {
        return paused;
    }

    #endregion

    #region Time

    private float timeScale=1;
    public void StopGeneralTime()
    {
        timeScale = 0;
        Time.timeScale = 0;
    }

    public void ContinueGeneralTime()
    {
        timeScale = 1;
        Time.timeScale = 1;
    }

    public void StopRelativeTime()
    {
        Time.timeScale = 0;
    }

    public void ContinueRelativeTime()
    {
        Time.timeScale = timeScale;
    }

    #endregion
}