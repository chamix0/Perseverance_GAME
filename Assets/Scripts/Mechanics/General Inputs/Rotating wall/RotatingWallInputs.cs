using System;
using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class RotatingWallInputs : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private PlayerNewInputs _newInputs;
    [NonSerialized] public RotatingWall _rotatingWall;
    private GuiManager _guiManager;
    private CameraController _cameraController;

    void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _guiManager = FindObjectOfType<GuiManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.RotatingWall && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (_newInputs.RotateWallCounterClockWise())
            {
                _rotatingWall.RotateWallCounterClockWise();
            }

            if (_newInputs.RotateWallClockWise())
            {
                _rotatingWall.RotateWallClockWise();
            }

            if (_newInputs.ExitWall())
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
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode && _playerValues.GetCurrentInput() == CurrentInput.RotatingWall )
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.RotateCWText() + "- Rotate wall Clockwise |" + _newInputs.RotateCCWText() +
            "- Rotate wall Counter Clockwise |" +
            _newInputs.ExitWallText() + "- Exit ");
    }
}