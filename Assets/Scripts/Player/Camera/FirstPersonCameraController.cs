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

    private AudioListener audioListener;

    //variables
    private Vector3 focusPoint, previousFocusPoint;
    [SerializeField] Vector2 orbitAngles = new Vector2(45f, 0f);

    private bool freezed;
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
        if (updateVertical)
        {
            orbitAngles.x = UpdateRotateCameraVertical();
        }

        if (updateRotation)
        {
            orbitAngles.y = UpdateRotateCamera();
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
        if (!freezed)
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

    private float verticalValue, targetVerticalRotation, tV = 0;
    private float rotationValue, targetRotation;
    private float originalVerticalSpeed, originalRotationSpeed;

    public float rotateStepVertical = 15f, angleStep = 15f;
    private bool updateVertical = false, updateRotation = false;


    //variables
    private float currentVerticalValue;
    private float currentRotation;


    public void RotateVerticalDown()
    {
        tV = 0.0f;
        updateVertical = true;
        currentVerticalValue = orbitAngles.x;
        targetVerticalRotation = Mathf.Max(minVerticalAngle, currentVerticalValue - rotateStepVertical);
    }

    public void RotateVerticalUp()
    {
        tV = 0.0f;
        updateVertical = true;
        currentVerticalValue = orbitAngles.x;
        targetVerticalRotation = Mathf.Min(maxVerticalAngle, currentVerticalValue + rotateStepVertical);
    }

    public void RotateClockwise()
    {
        currentRotation = orbitAngles.y;
        targetRotation = MyUtils.Clamp0360(SnapPosition(currentRotation + angleStep));
        updateRotation = true;
    }

    public void RotateCounterClockwise()
    {
        currentRotation = orbitAngles.y;
        targetRotation = MyUtils.Clamp0360(SnapPosition(currentRotation - angleStep));
        updateRotation = true;
    }

    public void RotateCustom(float angle)
    {
        currentRotation = orbitAngles.y;
        targetRotation = MyUtils.Clamp0360(SnapPosition(angle));
        updateRotation = true;
    }

    public void RotateVerticalCustom(float value)
    {
        currentVerticalValue = orbitAngles.x;
        targetVerticalRotation = value;
        tV = 0;
        updateVertical = true;
    }


    private float UpdateRotateCameraVertical()
    {
        currentVerticalValue = Mathf.Lerp(currentVerticalValue, targetVerticalRotation, tV);
        tV += 1f * Time.deltaTime;
        if (tV > 1.0f)
        {
            tV = 1.0f;
            updateVertical = false;
        }

        return currentVerticalValue;
    }

    private float UpdateRotateCamera()
    {
        currentRotation = Quaternion
            .Lerp(Quaternion.Euler(0, currentRotation, 0), Quaternion.Euler(0, targetRotation, 0), 5 * Time.deltaTime)
            .eulerAngles.y;

        if (Mathf.Abs(currentRotation - targetRotation) < 0.1f)
        {
            updateRotation = false;
        }

        return currentRotation;
    }


    public void FreezeCamera()
    {
        freezed = true;
    }

    public void UnFreezeCamera()
    {
        freezed = false;
    }


    float SnapPosition(float input)
    {
        return Mathf.Round(input / angleStep) * angleStep;
    }
}