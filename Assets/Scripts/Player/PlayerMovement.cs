using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerValues _playerValues;
    private Rigidbody _rigidbody;
    private float speed, _turnSmoothVel;
    public float stompUmbral, rayOffset;

    public float staminaUsage = 1f, staminaRecovery = 0.5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private LayerMask rayLayers;
    private CameraChanger cameraChanger;

    private void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerValues = GetComponent<PlayerValues>();
    }

    private void Update()
    {
        if (_playerValues.GetGear() > 1 && CheckIfStopm() || _playerValues.GetGear() == 0 && CheckIfBackStopm())
        {
            _playerValues.StopMovement();
            //play animation
        }
    }

    private void FixedUpdate()
    {
        //move on ground
        if (_playerValues.GetCanMove() && _playerValues.GetGear() != 1)
        {
            if (_playerValues.allStaminaUsed)
                _playerValues.allStaminaUsed = !(_playerValues.stamina >= 100);

            if (_playerValues.GetGear() == 4)
            {
                if (_playerValues.stamina > 0)
                {
                    _playerValues.stamina = Mathf.Max(_playerValues.stamina - staminaUsage, 0);
                }
                else
                {
                    _playerValues.allStaminaUsed = true;
                    _playerValues.DecreaseGear();
                }
            }
            else
            {
                if (_playerValues.GetGear() == 3)
                    _playerValues.stamina =
                        Mathf.Min(_playerValues.stamina + staminaRecovery / 4, _playerValues.maxStamina);
                else
                    _playerValues.stamina =
                        Mathf.Min(_playerValues.stamina + staminaRecovery, _playerValues.maxStamina);
            }

            Vector3 moveDirection = GetMoveDirection();
            _rigidbody.AddForce(moveDirection * _playerValues.GetSpeed() - _rigidbody.velocity,
                ForceMode.VelocityChange);
        }
        else
        {
            _playerValues.stamina = Mathf.Min(_playerValues.stamina + staminaRecovery, _playerValues.maxStamina);
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawRay(transform.position + new Vector3(0, rayOffset, 0),
        //     transform.TransformDirection(Vector3.forward) * stompUmbral);
        // Gizmos.color = Color.green;
        // Gizmos.DrawRay(transform.position + new Vector3(0, rayOffset, 0),
        //     transform.TransformDirection(-Vector3.forward) * stompUmbral * 1.5f);
    }

    private bool CheckIfStopm()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, rayOffset, 0),
                transform.TransformDirection(Vector3.forward), out hit, stompUmbral, rayLayers))
        {
            _playerValues.NotifyAction(PlayerActions.Stomp);
            return true;
        }

        return false;
    }

    private bool CheckIfBackStopm()
    {
        RaycastHit hit;


        if (Physics.Raycast(transform.localPosition + new Vector3(0, rayOffset, 0),
                transform.TransformDirection(-Vector3.forward), out hit, stompUmbral * 1.5f, rayLayers))
        {
            return true;
        }

        return false;
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 direction = new Vector3(0, 0f, 1)
            .normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) *
                            Mathf.Rad2Deg +
                            cameraChanger.GetActiveCam().transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            targetAngle, ref _turnSmoothVel,
            turnSmoothTime);


        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        return moveDirection;
    }
}