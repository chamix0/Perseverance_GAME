using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum ActiveCamera
{
    Orbit,
    FirstPerson,
    Screen,
    None
}

public class CameraChanger : MonoBehaviour
{
    private OrbitCameraController _orbitCameraController;
    private FirstPersonCameraController _firstPersonCameraController;
    private TransitionCameraController _transitionCameraController;

    [NonSerialized] public ActiveCamera activeCamera = ActiveCamera.Orbit;
    ActiveCamera nextCamera = ActiveCamera.None;

    private bool transition, transitioned;


    private void Start()
    {
        _transitionCameraController = FindObjectOfType<TransitionCameraController>();
        _orbitCameraController = FindObjectOfType<OrbitCameraController>();
        _firstPersonCameraController = FindObjectOfType<FirstPersonCameraController>();
        _orbitCameraController.regularCamera.enabled = true;
        _firstPersonCameraController.regularCamera.enabled = false;
        _transitionCameraController.regularCamera.enabled = false;
    }

    private void LateUpdate()
    {
        if (transition)
        {
            transitioned = _transitionCameraController.Finished();
            if (transitioned)
            {
                transition = false;
                transitioned = false;
                activeCamera = nextCamera;
                nextCamera = ActiveCamera.None;
                switch (activeCamera)
                {
                    case ActiveCamera.Orbit:
                        ActivateOrbit();
                        break;
                    case ActiveCamera.FirstPerson:
                        ActivateFirstPerson();
                        break;
                    case ActiveCamera.Screen:
                        break;
                }
            }
        }
    }

    public void SetOrbitCamera()
    {
        nextCamera = ActiveCamera.Orbit;
        Transition(_orbitCameraController.transform);
    }

    public void SetFirstPersonCamera()
    {
        nextCamera = ActiveCamera.FirstPerson;
        Transition(_firstPersonCameraController.transform);
    }

    public void SetScreenCamera()
    {
        nextCamera = ActiveCamera.Screen;
        // Transition(_orbitCameraController.regularCamera.transform);
    }

    private void Transition(Transform to)
    {
        Transform from = transform;
        switch (activeCamera)
        {
            case ActiveCamera.Orbit:
                from = _orbitCameraController.transform;
                break;
            case ActiveCamera.FirstPerson:
                from = _firstPersonCameraController.transform;
                break;
            case ActiveCamera.Screen:
                break;
        }

        _transitionCameraController.Transition(from, to);
        ActivateTransition();
        transition = true;
        transitioned = false;
    }

    private void ActivateOrbit()
    {
        _orbitCameraController.EnableCamera();
        _firstPersonCameraController.DisableCamera();
        _transitionCameraController.regularCamera.enabled = false;
    }

    private void ActivateFirstPerson()
    {
        _orbitCameraController.DisableCamera();
        _firstPersonCameraController.EnableCamera();
        _transitionCameraController.regularCamera.enabled = false;
    }

    private void ActivateTransition()
    {
        _orbitCameraController.DisableCamera();
        _firstPersonCameraController.DisableCamera();
        _transitionCameraController.regularCamera.enabled = true;
    }
}