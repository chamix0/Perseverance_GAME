using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class MachingunRotation : MonoBehaviour
{
    private CameraChanger cameraChanger;

    

    // Start is called before the first frame update
    void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
        {
            var transform1 = cameraChanger.GetActiveCam().transform;
            Ray ray = new Ray(transform1.position, transform1.forward);
            transform.LookAt(ray.GetPoint(20f));
        }
        else
        {
            transform.rotation=Quaternion.Euler(0,cameraChanger.GetActiveCam().transform.rotation.eulerAngles.y,0);
        }
    }
}