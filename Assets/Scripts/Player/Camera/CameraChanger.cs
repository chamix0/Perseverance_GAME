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
    private List<Camera> cameras;
    [SerializeField] private Camera screenCamera;
    private AudioListener audioListenerScreen;
    [NonSerialized] public ActiveCamera activeCamera = ActiveCamera.Orbit;
    ActiveCamera nextCamera = ActiveCamera.None;

    private bool transition, transitioned;

    private void Awake()
    {
        cameras = new List<Camera>();
    }

    private void Start()
    {
        audioListenerScreen = screenCamera.GetComponent<AudioListener>();
        _transitionCameraController = FindObjectOfType<TransitionCameraController>();
        _orbitCameraController = FindObjectOfType<OrbitCameraController>();
        _firstPersonCameraController = FindObjectOfType<FirstPersonCameraController>();

        cameras.Add(_orbitCameraController.regularCamera);
        cameras.Add(_firstPersonCameraController.regularCamera);
        cameras.Add(screenCamera);
        ActivateOrbit();
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
                        ActivateScreen();
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
        Transition(screenCamera.transform);
    }

    private void Transition(Transform to)
    {
        Transform from = transform;
        float fov = 50;

        switch (activeCamera)
        {
            case ActiveCamera.Orbit:
                from = _orbitCameraController.transform;
                break;
            case ActiveCamera.FirstPerson:
                from = _firstPersonCameraController.transform;
                break;
            case ActiveCamera.Screen:
                from = screenCamera.transform;
                break;
        }

        if (nextCamera is not ActiveCamera.None)
            fov = cameras[(int)nextCamera].fieldOfView;


        _transitionCameraController.Transition(from, to, fov);
        ActivateTransition();
        transition = true;
        transitioned = false;
    }

    public Camera GetActiveCam()
    {
        if (activeCamera is ActiveCamera.None)
            return cameras[(int)ActiveCamera.Orbit];
        return cameras[(int)activeCamera];
    }

    private void ActivateOrbit()
    {
        _orbitCameraController.EnableCamera();
        _firstPersonCameraController.DisableCamera();
        _transitionCameraController.DisableCamera();
        screenCamera.enabled = false;
        audioListenerScreen.enabled = false;
    }

    private void ActivateFirstPerson()
    {
        _firstPersonCameraController.regularCamera.transform.forward =
            _orbitCameraController.regularCamera.transform.forward;
        _orbitCameraController.DisableCamera();
        _firstPersonCameraController.EnableCamera();
        _transitionCameraController.DisableCamera();
        screenCamera.enabled = false;
        audioListenerScreen.enabled = false;
    }

    private void ActivateTransition()
    {
        _orbitCameraController.DisableCamera();
        _firstPersonCameraController.DisableCamera();
        _transitionCameraController.EnableCamera();
        screenCamera.enabled = false;
        audioListenerScreen.enabled = false;
    }

    private void ActivateScreen()
    {
        _orbitCameraController.DisableCamera();
        _firstPersonCameraController.DisableCamera();
        _transitionCameraController.DisableCamera();
        screenCamera.enabled = true;
        audioListenerScreen.enabled = true;
    }
}