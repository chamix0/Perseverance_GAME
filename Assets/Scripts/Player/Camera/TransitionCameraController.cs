using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransitionCameraController : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float speed = 0.01f, transitionTime = 3f, rotSpeed = 5;
    [SerializeField] private bool timeOrSpeed;
    float timeCount = 0.0f;

    private bool updatePosition;
    private float _tP;
    private bool finishedPos = true;

    public Camera regularCamera;

    private void Start()
    {
        regularCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (updatePosition)
        {
            UpdatePosition();
            UpdateRotation();
        }
    }


    public void Transition(Transform from, Transform to)
    {
        transform.position = from.position;
        transform.rotation = from.rotation;
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
        print(_tP);
        if (_tP >= 1f)
        {
            updatePosition = false;
            finishedPos = true;
            _tP = 1f;
        }
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotSpeed * Time.deltaTime);
    }

    bool RotationEquals(Quaternion r1, Quaternion r2)
    {
        float abs = Mathf.Abs(Quaternion.Dot(r1, r2));
        if (abs >= 0.9999999f)
            return true;
        return false;
    }
}