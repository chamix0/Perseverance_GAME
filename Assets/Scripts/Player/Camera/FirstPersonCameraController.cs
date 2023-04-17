using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FirstPersonCameraController : MonoBehaviour
{
    //parameters
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField] private bool invertX, invertY;
    public Camera regularCamera;
    [SerializeField] private Transform lookAtPoint;

    private AudioListener audioListener;
    //variables
    private Vector3 focusPoint, previousFocusPoint;
    [SerializeField] Vector2 orbitAngles = new Vector2(45f, 0f);

    //values

    private void Awake()
    {
                audioListener = GetComponent<AudioListener>();
                regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }



    private void LateUpdate()
    {
        ManualRotation();

        if (updateValueX)
        {
            orbitAngles.x = UpdateRotateCameraX();
        }

        if (updateValueY)
        {
            orbitAngles.y = UpdateRotateCameraY();
        }

        Quaternion lookRotation;
        if (ManualRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }

        Vector3 lookPosition = focus.position;
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }


    #region INPUT

    bool ManualRotation()
    {
        int xMod = 1, yMod = 1;
        if (invertX)
            xMod = -1;
        if (invertY)
            yMod = -1;


        Vector2 input = new Vector2(
            Input.GetAxis("Mouse Y") * yMod,
            Input.GetAxis("Mouse X") * xMod
        );
        const float e = 0.001f;
        if (input.x < e || input.x > e || input.y < e || input.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            return true;
        }

        return false;
    }

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

    #endregion

    public void EnableCamera()
    {
        regularCamera.enabled = true;
        audioListener.enabled = true;
    }
    public void DisableCamera()
    {
        regularCamera.enabled = false;
        audioListener.enabled = false;
    }


    #region My stuff

    private float rotationValueY, targetRotationY, tY = 0;
    private float rotationValueX, targetRotationX;
    private float originalYSpeed, originalXSpeed;

    public float rotateStepY = 15f, angleStepX = 15f;
    private bool updateValueY = false, updateValueX = false;

    //variables
    private float currentY;
    private float currentX;

    public void RotateYClockwise()
    {
        tY = 0.0f;
        updateValueY = true;
        currentY = orbitAngles.y;
        targetRotationY = currentY - rotateStepY;
    }

    public void RotateYCounterClockwise()
    {
        tY = 0.0f;
        updateValueY = true;
        currentY = orbitAngles.y;
        targetRotationY = currentY + rotateStepY;
    }

    public void RotateXClockwise()
    {
        currentX = orbitAngles.x;
        targetRotationX = SnapPosition(orbitAngles.x + angleStepX);
        updateValueX = true;
    }

    public void RotateXCounterClockwise()
    {
        currentX = orbitAngles.x;
        targetRotationX = SnapPosition(currentX - angleStepX);
        updateValueX = true;
    }

    public void RotateXCustom(float angle)
    {
        currentX = orbitAngles.x;
        targetRotationX = SnapPosition(angle);
        updateValueX = true;
    }

    public void RotateYCustom(float value)
    {
        currentY = orbitAngles.y;

        tY = 0;
        updateValueY = true;
    }


    private float UpdateRotateCameraY()
    {
        currentY = Mathf.Lerp(currentY, targetRotationY, tY);
        tY += 1f * Time.deltaTime;
        if (tY > 1.0f)
        {
            tY = 1.0f;
            updateValueY = false;
        }

        return currentY;
    }

    private float UpdateRotateCameraX()
    {
        // thirdPersonCamera.m_XAxis.Value;
        currentX = Quaternion
            .Lerp(Quaternion.Euler(0, currentX, 0),
                Quaternion.Euler(0, targetRotationX, 0), 5 * Time.deltaTime).eulerAngles.y;

        if (Mathf.Abs(currentX - targetRotationX) < 0.1f)
        {
            updateValueX = false;
        }

        return currentX;
    }


    public void FreezeCamera()
    {
    }

    public void UnFreezeCamera()
    {
    }


    float SnapPosition(float input)
    {
        return Mathf.Round(input / angleStepX) * angleStepX;
    }

    #endregion
}