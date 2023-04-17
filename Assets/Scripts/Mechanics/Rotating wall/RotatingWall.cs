using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class RotatingWall : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private OrbitCameraController _cameraController;
    private RotatingWallInputs _rotatingWallInputs;
    public GameObject snapPos;
    public float targetWallAngle, angleStep;
    [SerializeField] private Transform baseTransform;
    private PlayerAnimations _playerAnimations;
    private bool _updateRotation, inside;
    public int exitAngle;

    // Start is called before the first frame update
    void Start()
    {
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _cameraController = FindObjectOfType<OrbitCameraController>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _rotatingWallInputs = FindObjectOfType<RotatingWallInputs>();
        inside = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_updateRotation)
        {
            UpdateSnapAngle();
        }
    }

    public void RotateWallClockWise()
    {
        targetWallAngle = SnapPosition(baseTransform.eulerAngles.y + angleStep);
        _updateRotation = true;
    }

    public void RotateWallCounterClockWise()
    {
        targetWallAngle = SnapPosition(baseTransform.eulerAngles.y - angleStep);
        _updateRotation = true;
    }

    private void UpdateSnapAngle()
    {
        baseTransform.rotation =
            Quaternion.Slerp(baseTransform.rotation, Quaternion.Euler(0, targetWallAngle, 0), Time.deltaTime * 3f);
        _cameraController.RotateXCustom(baseTransform.eulerAngles.y + 90);

        if (Mathf.Abs(MyUtils.Clamp0360(baseTransform.eulerAngles.y) - MyUtils.Clamp0360(targetWallAngle)) <
            0.01f)
        {
            _updateRotation = false;
        }
    }

    float SnapPosition(float input)
    {
        return Mathf.Round(input / angleStep) * angleStep;
    }

    public bool CanExitRotatingWall()
    {
        if (Mathf.Abs(baseTransform.eulerAngles.y - MyUtils.Clamp0360(exitAngle)) < 0.01f ||
            Mathf.Abs(baseTransform.eulerAngles.y - MyUtils.Clamp0360(exitAngle + 180)) < 0.01f)
            return true;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !inside)
        {
            inside = true;
            _rotatingWallInputs._rotatingWall = this;
            _playerValues.SetCurrentInput(CurrentInput.RotatingWall);
            _playerValues.SetInputsEnabled(false);
            //camera snap
            _cameraController.RotateXCustom(transform.rotation.y + 90);
            _cameraController.RotateYCustom(0.5f);
            _cameraController.FreezeCamera();
            //player snap
            _playerValues.snapRotationTo(transform.rotation.y + 90);
            _playerValues.SnapPositionTo(snapPos.transform.position);
            _playerValues.Sit();
            _playerValues.gameObject.transform.parent = this.transform;
            //rigid body
            _playerValues._rigidbody.isKinematic = true;
            _playerValues._rigidbody.useGravity = false;
            _playerValues._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            StartCoroutine(WaitToSnapCoroutine());
        }
    }

    public void ExitRotatingWall()
    {
        _playerValues.gameObject.transform.parent = null;
        _playerValues._rigidbody.isKinematic = false;
        _playerValues._rigidbody.useGravity = true;
        _playerValues._rigidbody.constraints = _playerValues._originalRigidBodyConstraints;
        exitAngle = (-exitAngle);
        _playerValues.SetInputsEnabled(false);
        _playerValues.StandUp(false, 3f);
        StartCoroutine(ExitRotWallCoroutine());
    }


    IEnumerator WaitToSnapCoroutine()
    {
        yield return new WaitForSeconds(4f);
        _playerValues.SetInputsEnabled(true);
    }

    IEnumerator ExitRotWallCoroutine()
    {
        yield return new WaitForSeconds(3f);
        _playerValues.SetGear(0);
        yield return new WaitForSeconds(3f);
        _playerValues.StopMovement();
        _cameraController.UnFreezeCamera();
        _playerValues.SetInputsEnabled(true);
        inside = false;
    }
}