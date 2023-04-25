using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform camLook;
    public bool lookAtCamera;

    private void Start()
    {
        if (Camera.main != null) camLook = Camera.main.transform;
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
        transform.LookAt(camLook);
    }

    void LookInCameraDirection()
    {
        Quaternion lookAtRotation = Quaternion.LookRotation(transform.position - camLook.position);

        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(0, lookAtRotation.eulerAngles.y, 0);
        transform.rotation = LookAtRotationOnly_Y;
    }
}