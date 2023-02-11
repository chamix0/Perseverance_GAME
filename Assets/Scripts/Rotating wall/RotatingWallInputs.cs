using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWallInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private MyInputManager _myInputManager;
    [NonSerialized] public RotatingWall _rotatingWall;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _myInputManager = FindObjectOfType<MyInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentInput.RotatingWall && _myInputManager.GetInputsEnabled())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _rotatingWall.RotateWallCounterClockWise();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _rotatingWall.RotateWallClockWise();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_rotatingWall.CanExitRotatingWall())
                {
                    _myInputManager.SetCurrentInput(CurrentInput.Movement);
                    _rotatingWall.ExitRotatingWall();
                }
            }
        }
    }

    public void PerformAction(Move move)
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