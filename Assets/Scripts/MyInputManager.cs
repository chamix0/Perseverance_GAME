using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(1)]
public class MyInputManager : MonoBehaviour
{
    private MovesQueue _inputsMoves;
    private BasicCameraMovementInputs _movementInputs;

    private void Awake()
    {
        _inputsMoves = GameObject.FindGameObjectWithTag("UserCubeManager").GetComponent<MovesQueue>();
        _movementInputs = FindObjectOfType<BasicCameraMovementInputs>();
    }

    private void Update()
    {
        if (_inputsMoves.HasMessages())
        {
            Move move = _inputsMoves.Dequeue();
            //depending on the action we will give control to a different input manager
            _movementInputs.PerformAction(move);
        }
    }
}