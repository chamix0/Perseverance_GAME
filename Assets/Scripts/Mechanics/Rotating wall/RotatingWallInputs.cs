using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class RotatingWallInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    [NonSerialized] public RotatingWall _rotatingWall;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.RotatingWall && _playerValues.GetInputsEnabled()&&!_playerValues.GetPaused())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _rotatingWall.RotateWallCounterClockWise();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _rotatingWall.RotateWallClockWise();
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W))
            {
                if (_rotatingWall.CanExitRotatingWall())
                {
                    _playerValues.SetCurrentInput(CurrentInput.Movement);
                    _rotatingWall.ExitRotatingWall();
                }
            }
        }
    }

    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.D)
            {
                if (move.direction == 1)
                    _rotatingWall.RotateWallClockWise();
                else
                    _rotatingWall.RotateWallCounterClockWise();
            }
            //turn camera in y axis
            else if (move.face == FACES.R)
            {
                if (move.direction == -1)
                    if (_rotatingWall.CanExitRotatingWall())
                        _rotatingWall.ExitRotatingWall();
            }
        }
    }
}