using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private float rotationValueY, targetRotationY, tY = 0;
    private float rotationValueX, targetRotationX, tX = 0;


    public float rotateStepY = 0.1f, angleStepX = 15f;
    private bool updateValueY = false, updateValueX = false;


    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        targetRotationY = rotationValueY = _playerValues.thirdPersonCamera.m_YAxis.Value;
        angleStepX = 15f;
        rotateStepY = 0.25f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (updateValueY)
        {
            _playerValues.thirdPersonCamera.m_YAxis.Value = UpdateRotateCameraY();
        }

        if (updateValueX)
        {
            UpdateRotateCameraX();
        }
    }

    public void RotateYClockwise()
    {
        targetRotationY = _playerValues.thirdPersonCamera.m_YAxis.Value - rotateStepY;
        updateValueY = true;
        tY = 0.0f;
    }

    public void RotateYCounterClockwise()
    {
        targetRotationY = _playerValues.thirdPersonCamera.m_YAxis.Value + rotateStepY;
        updateValueY = true;
        tY = 0.0f;
    }

    public void RotateXClockwise()
    {
        targetRotationX = SnapPosition(_playerValues.thirdPersonCamera.m_XAxis.Value + angleStepX);
        updateValueX = true;
    }

    public void RotateXCounterClockwise()
    {
        targetRotationX = SnapPosition(_playerValues.thirdPersonCamera.m_XAxis.Value - angleStepX);
        print(targetRotationX);
        updateValueX = true;
    }

    private float UpdateRotateCameraY()
    {
        rotationValueY = Mathf.Lerp(_playerValues.thirdPersonCamera.m_YAxis.Value, targetRotationY, tY);

        tY += 0.5f * Time.deltaTime;

        if (tY > 1.0f)
        {
            tY = 1.0f;
            updateValueY = false;
        }

        return rotationValueY;
    }

    private void UpdateRotateCameraX()
    {
        _playerValues.thirdPersonCamera.m_XAxis.Value = Quaternion
            .Lerp(Quaternion.Euler(0, _playerValues.thirdPersonCamera.m_XAxis.Value, 0),
                Quaternion.Euler(0, targetRotationX, 0), 5 * Time.deltaTime).eulerAngles.y;

        if (Mathf.Abs(_playerValues.thirdPersonCamera.m_XAxis.Value - targetRotationX) < 0.01f)
        {
            updateValueX = false;
        }
    }

    float SnapPosition(float input)
    {
        return Mathf.Round(input / angleStepX) * angleStepX;
    }
}