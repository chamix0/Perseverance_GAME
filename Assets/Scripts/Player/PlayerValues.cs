using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    #region DATA

    [Header("VALUES")] [SerializeField] private List<float> movementSpeeds;
    public bool canMove = true, isGrounded, lightsOn = false;
    [SerializeField] [Range(0, 4)] private int gear = 1;
    public Camera mainCamera;
    [SerializeField] private LayerMask colisionLayers;


    [Header("COMPONENTES")] public CinemachineFreeLook thirdPersonCamera;
    [NonSerialized]public Rigidbody _rigidbody;

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
        _targetAngle;

    public RigidbodyConstraints _originalRigidBodyConstraints;

    private Vector3 _lookAtPos;
    private bool _updateSnap, _updateLookAt;

    #endregion

    private void Awake()
    {
        // movementSpeeds = new List<float> { -1, 0, 1, 2, 3 };
        _rigidbody = GetComponent<Rigidbody>();
        _playerLights = GetComponent<PlayerLights>();
        _originalRigidBodyConstraints = _rigidbody.constraints;
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
    }

    #region GEAR

    public int GetGear()
    {
        return gear;
    }

    public void RiseGear()
    {
        gear = Mathf.Min(gear + 1, 4);
    }

    public void DecreaseGear()
    {
        gear = Mathf.Max(gear - 1, 0);
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
        gear = 1;
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
                gear = 1;
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
}