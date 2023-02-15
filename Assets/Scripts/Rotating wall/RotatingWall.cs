using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWall : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private MyInputManager _myInputManager;
    private RotatingWallInputs _rotatingWallInputs;
    public GameObject snapPos;
    public float targetWallAngle, angleStep;
    [SerializeField] private Transform baseTransform;
    private bool _updateRotation, inside;
    public int exitAngle;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _myInputManager = FindObjectOfType<MyInputManager>();
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
        _playerValues.thirdPersonCamera.m_XAxis.Value = baseTransform.eulerAngles.y + 90;

        if (Mathf.Abs(PlayerValues.Clamp0360(baseTransform.eulerAngles.y) - PlayerValues.Clamp0360(targetWallAngle)) <
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
        if (Mathf.Abs(baseTransform.eulerAngles.y - PlayerValues.Clamp0360(exitAngle)) < 0.01f ||
            Mathf.Abs(baseTransform.eulerAngles.y - PlayerValues.Clamp0360(exitAngle + 180)) < 0.01f)
            return true;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !inside)
        {
            inside = true;
            _rotatingWallInputs._rotatingWall = this;
            _myInputManager.SetCurrentInput(CurrentInput.RotatingWall);
            _myInputManager.SetInputsEnabled(false);
            //camera snap
            _playerValues._cameraController.RotateXCustom(transform.rotation.y + 90);
            _playerValues._cameraController.RotateYCustom(0.5f);
            _playerValues.FreezeCamera();
            //player snap
            _playerValues.snapRotationTo(transform.rotation.y + 90);
            _playerValues.SnapPositionTo(snapPos.transform.position);
            _playerValues.SetCanMove(false);
            _playerValues.SetSitAnim(true);
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
        _playerValues.SetSitAnim(false);
        _myInputManager.SetInputsEnabled(false);
        StartCoroutine(ExitRotWallCoroutine());
    }


    IEnumerator WaitToSnapCoroutine()
    {
        yield return new WaitForSeconds(4f);
        _myInputManager.SetInputsEnabled(true);
    }

    IEnumerator ExitRotWallCoroutine()
    {
        _myInputManager.SetCurrentInput(CurrentInput.Movement);
        yield return new WaitForSeconds(3f);
        _playerValues.SetCanMove(true);
        _playerValues.SetGear(0);
        yield return new WaitForSeconds(3f);
        _playerValues.StopMovement();
        _playerValues.UnFreezeCamera();
        _myInputManager.SetInputsEnabled(true);
        inside = false;
    }
}