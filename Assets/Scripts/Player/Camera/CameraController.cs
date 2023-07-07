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

    public void RotateVerticalDown()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateVerticalDown();
        else
            orbitCameraController.RotateVerticalDown();
    }

    public void RotateVerticalUp()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateVerticalUp();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateVerticalUp();
    }

    public void RotateClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateClockwise();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateClockwise();
    }

    public void RotateCounterClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateCounterClockwise();
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateCounterClockwise();
    }

    public void RotateCustom(float angle)
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateCustom(angle);
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateCustom(angle);
    }

    public void RotateVerticalCustom(float value)
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
            firstPersonCameraController.RotateVerticalCustom(value);
        else if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateVerticalCustom(value);
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