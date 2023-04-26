using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCameraController : MonoBehaviour
{
    //parameters
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 20f)] private float distance = 5f;
    [SerializeField, Min(0f)] float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] float focusCentering = 0.5f;
    [SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField] private bool invertX, invertY;
    [SerializeField, Min(0f)] float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] float alignSmoothRange = 45f;
    [SerializeField] LayerMask CollisionLayers = -1;
    public Camera regularCamera;

    //variables
    private Vector3 focusPoint, previousFocusPoint;
    [SerializeField] Vector2 orbitAngles = new Vector2(45f, 0f);

    //values
    float lastManualRotationTime;
    private bool freezed;

    private void Awake()
    {
        audioListener = GetComponent<AudioListener>();

        regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }


    private void LateUpdate()
    {
        UpdateFocusPoint();
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
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(
                castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
                lookRotation, castDistance, CollisionLayers
            ))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        if (!freezed)
            transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    void UpdateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
                // t = Mathf.Pow(1f - focusCentering, Time.deltaTime);
            }

            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }

            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }
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
            lastManualRotationTime = Time.unscaledTime;
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

    bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z
        );
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange =
            rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }

        orbitAngles.y =
            Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    #region camera collision

    Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y =
                regularCamera.nearClipPlane *
                Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    #endregion

    #region My stuff

    private float verticalValue, targetVerticalRotation, tV = 0;
    private float rotationValue, targetRotation;
    private float originalVerticalSpeed, originalRotationSpeed;

    public float rotateStepVertical = 15f, angleStep = 15f;
    private bool updateVertical = false, updateRotation = false;

    private AudioListener audioListener;

    //variables
    private float currentVerticalValue;
    private float currentRotation;

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
        targetRotation = MyUtils.Clamp0360(SnapPosition(currentRotation- angleStep));
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
            .Lerp(Quaternion.Euler(0,currentRotation , 0), Quaternion.Euler(0,targetRotation , 0), 5 * Time.deltaTime).eulerAngles.y;

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

    #endregion
}