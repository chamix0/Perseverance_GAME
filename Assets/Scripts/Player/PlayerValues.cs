using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    #region DATA

    [Header("VALUES")] [SerializeField] private List<float> movementSpeeds;
    public bool canMove = true, isGrounded, lightsOn = false, updateGearAnim;
    [SerializeField] [Range(0, 4)] private int gear = 1;
    private float gearAnim, gearAnimTarget;
    public Camera mainCamera;
    [SerializeField] private LayerMask colisionLayers;


    [Header("COMPONENTES")] public CinemachineFreeLook thirdPersonCamera;
    [NonSerialized] public Rigidbody _rigidbody;
    private Animator _animator;
    private PlayerLights _playerLights;

    //variables
    private float _snapPosX,
        _snapPosY,
        _snapPosZ,
        _targetPosX,
        _targetPosY,
        _targetPosZ,
        _tX,
        _tY,
        _tZ,
        _tR,
        _tG,
        _targetAngle;

    public float stamina, maxStamina;

    [NonSerialized] public RigidbodyConstraints _originalRigidBodyConstraints;

    private Vector3 _lookAtPos;
    private bool _updateSnap, _updateLookAt;
    private static readonly int Gear = Animator.StringToHash("Gear");

    #endregion

    private void Awake()
    {
        // movementSpeeds = new List<float> { -1, 0, 1, 2, 3 };
        _rigidbody = GetComponent<Rigidbody>();
        _playerLights = GetComponent<PlayerLights>();
        _animator = GetComponentInChildren<Animator>();
        _originalRigidBodyConstraints = _rigidbody.constraints;
    }

    private void Update()
    {
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

    private void Start()
    {
        canMove = true;
        gear = 1;
        _animator.SetFloat("Gear", gear);
    }

    #region GEAR

    public int GetGear()
    {
        return gear;
    }

    public void RiseGear()
    {
        int old = gear;
        gear = Mathf.Min(gear + 1, 4);
        ChangeGearAnim(old, gear);
    }

    public void DecreaseGear()
    {
        int old = gear;
        gear = Mathf.Max(gear - 1, 0);
        ChangeGearAnim(old, gear);
    }

    #endregion

    public float GetSpeed()
    {
        return movementSpeeds[gear];
    }

    #region Movement control

    public void SetCanMove(bool val)
    {
        canMove = val;
        int old = gear;
        gear = 1;
        ChangeGearAnim(old, gear);
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
        _tR = 0f;
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

    private void ChangeGearAnim(int oldGear, int newGear)
    {
        updateGearAnim = true;
        _tG = 0.0f;
        gearAnim = oldGear;
        gearAnimTarget = newGear;
    }


    private void UpdateSnapAngle()
    {
        transform.rotation =
            Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _targetAngle, 0), Time.deltaTime * 3f);

        if (Mathf.Abs(transform.rotation.y - _targetAngle) < 0.01f)
        {
            _updateLookAt = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entered");
        if (IsInLayerMask(collision.gameObject, colisionLayers))
        {
            if (!isGrounded)
            {
                canMove = true;
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exited");
        if (IsInLayerMask(collision.gameObject, colisionLayers))
        {
            if (isGrounded)
            {
                _rigidbody.constraints = RigidbodyConstraints.None;
                int old = gear;
                gear = 1;
                ChangeGearAnim(old, 1);
                SetCanMove(false);
                isGrounded = false;
            }
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)

    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }

    public void ResetRigidBodyConstraints()
    {
        if (isGrounded)
        {
            _rigidbody.constraints = _originalRigidBodyConstraints;
        }
    }

    public void StopMovement()
    {
        int old = gear;

        gear = 1;
        ChangeGearAnim(old, gear);
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
}