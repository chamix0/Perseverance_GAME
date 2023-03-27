using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    private float rotationValueY, targetRotationY, tY = 0;
    private float rotationValueX, targetRotationX;

    private float originalYSpeed, originalXSpeed;


    public float rotateStepY = 0.1f, angleStepX = 15f;
    private bool updateValueY = false, updateValueX = false;


    void Start()
    {
        targetRotationY = rotationValueY = thirdPersonCamera.m_YAxis.Value;
        angleStepX = 15f;
        rotateStepY = 0.25f;
        originalXSpeed = thirdPersonCamera.m_XAxis.m_MaxSpeed;
        originalYSpeed = thirdPersonCamera.m_YAxis.m_MaxSpeed;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (updateValueY)
        {
            thirdPersonCamera.m_YAxis.Value = UpdateRotateCameraY();
        }

        if (updateValueX)
        {
            UpdateRotateCameraX();
        }
    }

    public void RotateYClockwise()
    {
        targetRotationY = thirdPersonCamera.m_YAxis.Value - rotateStepY;
        updateValueY = true;
        tY = 0.0f;
    }

    public void RotateYCounterClockwise()
    {
        targetRotationY = thirdPersonCamera.m_YAxis.Value + rotateStepY;
        updateValueY = true;
        tY = 0.0f;
    }

    public void RotateXClockwise()
    {
        targetRotationX = SnapPosition(thirdPersonCamera.m_XAxis.Value + angleStepX);
        updateValueX = true;
    }

    public void RotateXCustom(float angle)
    {
        targetRotationX = SnapPosition(angle);
        updateValueX = true;
    }

    public void RotateYCustom(float value)
    {
        targetRotationY = Mathf.Clamp(value, 0, 1);
        tY = 0;
        updateValueY = true;
    }

    public void RotateXCounterClockwise()
    {
        targetRotationX = SnapPosition(thirdPersonCamera.m_XAxis.Value - angleStepX);
        updateValueX = true;
    }

    private float UpdateRotateCameraY()
    {
        rotationValueY = Mathf.Lerp(thirdPersonCamera.m_YAxis.Value, targetRotationY, tY);
        tY += 1f * Time.deltaTime;

        if (tY > 1.0f)
        {
            tY = 1.0f;
            updateValueY = false;
        }

        return rotationValueY;
    }

    public CinemachineFreeLook GetThirdPErsonCamera()
    {
        return thirdPersonCamera;
    }

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

    private void UpdateRotateCameraX()
    {
        thirdPersonCamera.m_XAxis.Value = Quaternion
            .Lerp(Quaternion.Euler(0, thirdPersonCamera.m_XAxis.Value, 0),
                Quaternion.Euler(0, targetRotationX, 0), 5 * Time.deltaTime).eulerAngles.y;

        if (Mathf.Abs(thirdPersonCamera.m_XAxis.Value - targetRotationX) < 0.1f)
        {
            updateValueX = false;
        }
    }

    float SnapPosition(float input)
    {
        return Mathf.Round(input / angleStepX) * angleStepX;
    }
}