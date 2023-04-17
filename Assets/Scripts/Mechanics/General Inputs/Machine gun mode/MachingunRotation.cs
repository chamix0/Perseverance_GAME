using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class MachingunRotation : MonoBehaviour
{
    private CameraChanger cameraChanger;
    private PlayerValues playerValues;

    private Camera cam;
    public float aaa = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
        playerValues = FindObjectOfType<PlayerValues>();
        cam = playerValues.mainCamera;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
        {
            var transform1 = cam.transform;
            Ray ray = new Ray(transform1.position,
                transform1.forward);
            transform.LookAt(ray.GetPoint(20f));
        }
        else
        {
            transform.rotation=Quaternion.Euler(0,cam.transform.rotation.eulerAngles.y,0);
        }
    }
}