using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerValues _playerValues;
    private Rigidbody _rigidbody;
    private float speed, _turnSmoothVel;
    [SerializeField] private float turnSmoothTime = 0.1f;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerValues = GetComponent<PlayerValues>();
    }

    private void FixedUpdate()
    {
        //move on ground
        if (_playerValues.canMove && _playerValues.GetGear() != 1)
        {
            Vector3 moveDirection = GetMoveDirection();
            _rigidbody.AddForce(moveDirection * _playerValues.GetSpeed() - _rigidbody.velocity,
                ForceMode.VelocityChange);
        }
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 direction = new Vector3(0, 0f, 1)
            .normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) *
                            Mathf.Rad2Deg +
                            _playerValues.mainCamera.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            targetAngle, ref _turnSmoothVel,
            turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        return moveDirection;
    }





}