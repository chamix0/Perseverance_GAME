using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool lookAtCamera;
    private CameraChanger cameraChanger;

    private void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
    }

    private void Update()
    {
        if (lookAtCamera)
            LookToCamera();
        else
            LookInCameraDirection();
    }

    void LookToCamera()
    {
        transform.LookAt(cameraChanger.GetActiveCam().transform);
    }

    void LookInCameraDirection()
    {
        Quaternion lookAtRotation =
            Quaternion.LookRotation(transform.position - cameraChanger.GetActiveCam().transform.position);

        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(0, lookAtRotation.eulerAngles.y, 0);
        transform.rotation = LookAtRotationOnly_Y;
    }
}