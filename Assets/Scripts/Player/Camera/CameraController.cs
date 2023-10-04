using EZCameraShake;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private OrbitCameraController orbitCameraController;
    private CameraChanger cameraChanger;

    //shake
    [Header("Shake")] [Range(0, 10)] public float magnitude = 2f;
    [Range(0, 10)] public float roughness = 2f;
    [Range(0, 1)] public float fadeInTime = .1f;
    [Range(0, 5)] public float fadeOutTime = 1f;


    void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
        orbitCameraController = FindObjectOfType<OrbitCameraController>();
    }

    public void SlowCamera(int speed)
    {
        orbitCameraController.SlowCameraSpeed(speed);
    }

    public void NormalCameraSpeed()
    {
        orbitCameraController.ReturnToNormalCameraSpeed();
    }
    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
    }

    public void RotateVerticalDown()
    {
        orbitCameraController.RotateVerticalDown();
    }

    public void RotateVerticalUp()
    {
        if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateVerticalUp();
    }

    public void RotateClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateClockwise();
    }

    public void RotateCounterClockwise()
    {
        if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateCounterClockwise();
    }

    public void RotateCustom(float angle)
    {
        if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateCustom(angle);
    }

    public void RotateVerticalCustom(float value)
    {
        if (cameraChanger.activeCamera is ActiveCamera.Orbit)
            orbitCameraController.RotateVerticalCustom(value);
    }
}