using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CurrentInput
{
    Movement,
    Conversation,
    Battle
}

[DefaultExecutionOrder(1)]
public class MyInputManager : MonoBehaviour
{
    private MovesQueue _inputsMoves;
    private BasicCameraMovementInputs _movementInputs;
    private CurrentInput _currentInput;

    private void Awake()
    {
        _inputsMoves = GameObject.FindGameObjectWithTag("UserCubeManager").GetComponent<MovesQueue>();
        _movementInputs = FindObjectOfType<BasicCameraMovementInputs>();
    }

    private void Update()
    {
        if (_inputsMoves.HasMessages())
        {
            
            switch (_currentInput)
            {
                case CurrentInput.Movement:
                    
                    break;
            }

            Move move = _inputsMoves.Dequeue();
            //depending on the action we will give control to a different input manager
            _movementInputs.PerformAction(move);
        }
    }
}