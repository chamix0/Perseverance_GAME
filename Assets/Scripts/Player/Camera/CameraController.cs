using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    private float rotationValueY, targetRotationY, tY = 0;
    private float rotationValueX, targetRotationX;
    private float originalYSpeed, originalXSpeed;

    public float rotateStepY = 0.1f, angleStepX = 15f;
    private bool updateValueY = false, updateValueX = false;

    //variables
    private float currentY;
    private float currentX;

    //collision
    private CinemachineCameraOffset cinemachineCameraOffset;
    [SerializeField] private LayerMask collisionLayers;
    private float originalOffset, originalDistance;
    public float offset;


    void Start()
    {
        targetRotationY = rotationValueY = thirdPersonCamera.m_YAxis.Value;
        angleStepX = 15f;
        rotateStepY = 0.25f;
        originalXSpeed = thirdPersonCamera.m_XAxis.m_MaxSpeed;
        originalYSpeed = thirdPersonCamera.m_YAxis.m_MaxSpeed;
        cinemachineCameraOffset = thirdPersonCamera.transform.gameObject.GetComponent<CinemachineCameraOffset>();
        originalOffset = cinemachineCameraOffset.m_Offset.z;
        originalDistance = Vector3.Distance(thirdPersonCamera.transform.position, thirdPersonCamera.LookAt.position);
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
            thirdPersonCamera.m_XAxis.Value = UpdateRotateCameraX();
        }

        CameraCollisionCheck();
    }

    private void CameraCollisionCheck()
    {
        RaycastHit hit;
        float distance = Vector3.Distance(thirdPersonCamera.transform.position, thirdPersonCamera.LookAt.position);
        if (Physics.Raycast(thirdPersonCamera.LookAt.position,
                thirdPersonCamera.transform.position - thirdPersonCamera.LookAt.position, out hit, originalDistance
                , collisionLayers))
        {
            cinemachineCameraOffset.m_Offset.z = originalOffset + Mathf.Abs(distance - hit.distance) + offset;
        }
        else
        {
            cinemachineCameraOffset.m_Offset.z = originalOffset;
        }
    }

    public void RotateYClockwise()
    {
        targetRotationY = thirdPersonCamera.m_YAxis.Value - rotateStepY;
        currentY = thirdPersonCamera.m_YAxis.Value;
        updateValueY = true;
        tY = 0.0f;
    }

    public void RotateYCounterClockwise()
    {
        targetRotationY = thirdPersonCamera.m_YAxis.Value + rotateStepY;
        currentY = thirdPersonCamera.m_YAxis.Value;
        updateValueY = true;
        tY = 0.0f;
    }

    public void RotateXClockwise()
    {
        targetRotationX = SnapPosition(thirdPersonCamera.m_XAxis.Value + angleStepX);
        currentX = thirdPersonCamera.m_XAxis.Value;
        updateValueX = true;
    }

    public void RotateXCustom(float angle)
    {
        targetRotationX = SnapPosition(angle);
        currentX = thirdPersonCamera.m_XAxis.Value;
        updateValueX = true;
    }

    public void RotateYCustom(float value)
    {
        targetRotationY = Mathf.Clamp(value, 0, 1);
        currentY = thirdPersonCamera.m_YAxis.Value;
        tY = 0;
        updateValueY = true;
    }

    public void RotateXCounterClockwise()
    {
        targetRotationX = SnapPosition(thirdPersonCamera.m_XAxis.Value - angleStepX);
        currentX = thirdPersonCamera.m_XAxis.Value;
        updateValueX = true;
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

    float SnapPosition(float input)
    {
        return Mathf.Round(input / angleStepX) * angleStepX;
    }
}