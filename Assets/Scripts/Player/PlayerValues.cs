using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cinemachine;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerValues : MonoBehaviour
{
    #region DATA

    //status values
    [Header("VALUES")] [SerializeField] private List<float> movementSpeeds;
    [SerializeField] [Range(0, 4)] private int gear = 1;
    private bool isGrounded, lightsOn, updateGearAnim, cameraIsFreezed, stucked;
    private bool canMove;

    //stamina values
    public float stamina, maxStamina;

    //stuck values
    private Stopwatch stuckTimer;
    private Vector3 stuckedPos;
    private Vector3 lastValidPos;
    public float stuckTime = 3f;

    //compontes
    [Header("COMPONENTES")] public CinemachineFreeLook thirdPersonCamera;
    [NonSerialized] public Rigidbody _rigidbody;
    [NonSerialized] public CameraController _cameraController;
    private Animator _animator;
    private PlayerLights _playerLights;
    public Camera mainCamera;
    [NonSerialized] public RigidbodyConstraints _originalRigidBodyConstraints;

    //variables
    private float gearAnim, gearAnimTarget;
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
        _tG,
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
        // movementSpeeds = new List<float> { -1, 0, 1, 2, 3 };
        _rigidbody = GetComponent<Rigidbody>();
        _playerLights = GetComponent<PlayerLights>();
        _animator = GetComponentInChildren<Animator>();
        _cameraController = GetComponent<CameraController>();
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
        if (updateGearAnim)
        {
            _animator.SetFloat("Gear", UpdateGearValAnim());
        }
    }

    private void FixedUpdate()
    {
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
        Debug.DrawRay(transform.position + RayOffset,
            transform.TransformDirection(Vector3.down) * raySize, Color.red);
    }

    #region GEAR

    public int GetGear()
    {
        return gear;
    }

    public void RiseGear()
    {
        if (isGrounded)
        {
            int old = gear;
            gear = Mathf.Min(gear + 1, 4);
            ChangeGearAnim(old, gear);
        }
        else
        {
            SetGear(1);
        }
    }

    public void DecreaseGear()
    {
        if (isGrounded)
        {
            int old = gear;
            gear = Mathf.Max(gear - 1, 0);
            ChangeGearAnim(old, gear);
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
            ChangeGearAnim(old, gear);
        }
        else
        {
            int old = gear;
            gear = 1;
            ChangeGearAnim(old, gear);
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


    private void UpdateSnapAngle()
    {
        transform.rotation =
            Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, Clamp0360(_targetAngle), 0), Time.deltaTime * 5f);

        if (Mathf.Abs(transform.eulerAngles.y - Clamp0360(_targetAngle)) < 0.01f)
        {
            _updateLookAt = false;
        }
    }

    #endregion

    #region animator

    private void ChangeGearAnim(int oldGear, int newGear)
    {
        updateGearAnim = true;
        _tG = 0.0f;
        gearAnim = oldGear;
        gearAnimTarget = newGear;
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
            if (IsInLayerMask(hit.transform.gameObject, colisionLayers))
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
        if (Physics.Raycast(transform.position + RayOffset,
                transform.TransformDirection(Vector3.down), out hit, raySize))
        {
            if (IsInLayerMask(hit.transform.gameObject, colisionLayers))
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
        _playerLights.TurnOnLights();
        lightsOn = true;
    }

    public void TurnOffLights()
    {
        _playerLights.TurnOffLights();
        lightsOn = false;
    }

    public bool GetLights()
    {
        return lightsOn;
    }

    #endregion

    #region animation

    private float UpdateGearValAnim()
    {
        gearAnim = Mathf.Lerp(gearAnim, gearAnimTarget, _tG);
        _tG += 0.5f * Time.deltaTime;

        if (_tG > 1.0f)
        {
            _tG = 1.0f;
            updateGearAnim = false;
        }

        return gearAnim;
    }

    public void SetSitAnim(bool val)
    {
        _animator.SetBool("Sit", val);
    }

    #endregion

    #region Camera

    public void FreezeCamera()
    {
        thirdPersonCamera.m_XAxis.m_MaxSpeed = 0;
        thirdPersonCamera.m_YAxis.m_MaxSpeed = 0;
    }


    public void UnFreezeCamera()
    {
        thirdPersonCamera.m_XAxis.m_MaxSpeed = 300;
        thirdPersonCamera.m_YAxis.m_MaxSpeed = 2;
    }

    #endregion

    #region utils

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)

    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }

    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
            result += 360f;

        return result;
    }

    #endregion
}