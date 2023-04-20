using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransitionCameraController : MonoBehaviour
{
    private Transform target;
    private float targetFov;
    [SerializeField] private float speed = 0.01f, transitionTime = 3f, rotSpeed = 5;
    [SerializeField] private bool timeOrSpeed;
    float timeCount = 0.0f;

    private bool updatePosition;
    private float _tP, _tF;
    private bool finishedPos = true;
    private AudioListener audioListener;

    public Camera regularCamera;

    private void Awake()
    {
        regularCamera = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();
    }


    private void Update()
    {
        if (updatePosition)
        {
            UpdatePosition();
            UpdateRotation();
            UpdateFov();
        }
    }


    public void Transition(Transform from, Transform to, float fov)
    {
        transform.position = from.position;
        transform.rotation = from.rotation;
        targetFov = fov;
        target = to;
        updatePosition = true;
        finishedPos = false;
        _tP = 0f;
    }

    public bool Finished()
    {
        return finishedPos;
    }

    private void UpdatePosition()
    {
        float rateTiempo = 1f / transitionTime;
        float rateVelocity = 1f / Vector3.Distance(transform.position, target.position) * speed;
        if (timeOrSpeed)
            _tP += Time.deltaTime * rateTiempo;
        else
            _tP += Time.deltaTime * rateVelocity;
        transform.position = Vector3.Lerp(transform.position, target.position, _tP);
        if (_tP >= 1f)
        {
            updatePosition = false;
            finishedPos = true;
            _tP = 1f;
        }
    }
    private void UpdateFov()
    {
        regularCamera.fieldOfView = Mathf.Lerp(regularCamera.fieldOfView, targetFov, Time.deltaTime * 2);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotSpeed * Time.deltaTime);
    }

    public void EnableCamera()
    {
        regularCamera.enabled = true;
        audioListener.enabled = true;
    }

    public void DisableCamera()
    {
        regularCamera.enabled = false;
        audioListener.enabled = false;
    }
}