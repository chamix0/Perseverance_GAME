using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    private CameraController _cameraController;
    private CinemachineFreeLook thirdPersonCamera;
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] private CinemachineVirtualCamera screenCamera;

    private void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
        thirdPersonCamera = _cameraController.GetThirdPErsonCamera();
    }


    public void SetOrbitCamera()
    {
        thirdPersonCamera.Priority = 10;
        firstPersonCamera.Priority = 0;
        screenCamera.Priority = 0;
    }

    public void SetFirstPersonCamera()
    {
        thirdPersonCamera.Priority = 0;
        firstPersonCamera.Priority = 10;
        screenCamera.Priority = 0;
    }

    public void SetScreenCamera()
    {
        thirdPersonCamera.Priority = 0;
        firstPersonCamera.Priority = 0;
        screenCamera.Priority = 10;
    }
}