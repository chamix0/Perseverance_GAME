using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private OrbitCameraController orbitCameraController;
    private FirstPersonCameraController firstPersonCameraController;

    private CameraChanger cameraChanger;

    //shake
    [Header("Shake")]
    [Range(0, 10)] public float magnitude = 2f;
    [Range(0, 10)] public float roughness = 2f;
    [Range(0, 1)] public float fadeInTime = .1f;
    [Range(0, 5)] public float fadeOutTime = 1f;


    void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
        orbitCameraController = FindObjectOfType<OrbitCameraController>();
        firstPersonCameraController = FindObjectOfType<FirstPersonCameraController>();
    }

    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
    }

    public void RotateYClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateXClockwise();
        else
            orbitCameraController.RotateXClockwise();
    }

    public void RotateYCounterClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateXCounterClockwise();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateXCounterClockwise();
    }

    public void RotateXClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateYClockwise();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateYClockwise();
    }

    public void RotateXCounterClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateYCounterClockwise();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateYCounterClockwise();
    }

    public void RotateXCustom(float angle)
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateYCounterClockwise();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateYCounterClockwise();
    }

    public void RotateYCustom(float value)
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateXCustom(value);
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateXCustom(value);
    }

    public void FreezeCamera()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.FreezeCamera();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.FreezeCamera();
    }

    public void UnFreezeCamera()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.UnFreezeCamera();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.UnFreezeCamera();
    }
}