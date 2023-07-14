using System;
using Mechanics.General_Inputs;
using UnityEngine;

public class RotatingWallInputs : MonoBehaviour,InputInterface
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    [NonSerialized] public RotatingWall _rotatingWall;
    private GuiManager _guiManager;
    private CameraController _cameraController;

    void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _guiManager = FindObjectOfType<GuiManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.RotatingWall && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                _guiManager.SetTutorial(
                    "QE - Rotate wall    WS - Exit   ");
            }

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
            _guiManager.SetTutorial(
                "D - Rotate wall   U - Camera horizonatal axis    L - Camera Vertical Axis     R - Exit");
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

            if (move.face == FACES.L)
            {
                if (move.direction == 1)
                    _cameraController.RotateVerticalDown();
                else
                    _cameraController.RotateVerticalUp();
            }
            else if (move.face == FACES.U)
            {
                if (move.direction == 1) _cameraController.RotateClockwise();
                else _cameraController.RotateCounterClockwise();
            }
        }
    }
}