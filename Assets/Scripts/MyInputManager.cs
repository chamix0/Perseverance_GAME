using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentInput
{
    Movement,
    RotatingWall,
    Conversation,
    Battle,
    None
}

[DefaultExecutionOrder(1)]
public class MyInputManager : MonoBehaviour
{
    private MovesQueue _inputsMoves;
    private BasicCameraMovementInputs _movementInputs;
    private RotatingWallInputs _rotatingWallInputs;
    private CurrentInput _currentInput;
    private bool inputsEnabled = true;

    private void Awake()
    {
        // _inputsMoves = GameObject.FindGameObjectWithTag("UserCubeManager").GetComponent<MovesQueue>();
        _movementInputs = FindObjectOfType<BasicCameraMovementInputs>();
        _rotatingWallInputs = FindObjectOfType<RotatingWallInputs>();
    }

    private void Update()
    {
        if (_inputsMoves && _inputsMoves.HasMessages() && inputsEnabled)
        {
            Move move = _inputsMoves.Dequeue();
            switch (_currentInput)
            {
                case CurrentInput.Movement:
                    _movementInputs.PerformAction(move);
                    break;
                case CurrentInput.RotatingWall:
                    _rotatingWallInputs.PerformAction(move);
                    break;
                case CurrentInput.None:
                    break;
            }

            //depending on the action we will give control to a different input manager
        }
    }

    public void SetCurrentInput(CurrentInput currentInput)
    {
        _currentInput = currentInput;
    }

    public CurrentInput GetCurrentInput()
    {
        return _currentInput;
    }

    public bool GetInputsEnabled()
    {
        return inputsEnabled;
    }

    public void SetInputsEnabled(bool val)
    {
        inputsEnabled = val;
    }
}