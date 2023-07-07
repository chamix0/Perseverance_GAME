using Mechanics.General_Inputs;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class BasicCameraMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private GuiManager guiManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
        guiManager = FindObjectOfType<GuiManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.Movement && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                guiManager.SetTutorial(
                    "SPAM SPACE IF STUCKED!   ESC - Pause    WS - Increase/Descrease gear    D - Lights");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_playerValues.GetGear() < 3)
                    _playerValues.RiseGear();

                _playerValues.CheckIfStuck(true);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _playerValues.DecreaseGear();
                _playerValues.CheckIfStuck(true);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
            }
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        if (_playerValues.GetInputsEnabled())
        {
            guiManager.SetTutorial(
                "SPAM SPACE IF STUCKED!   R - Increase/Decrease gear   U - Camera horizonatal axis   L - Camera Vertical Axis     B - Lights");

            if (move.face == FACES.R)
            {
                _playerValues.CheckIfStuck(true);
                if (move.direction == 1)
                {
                    if (_playerValues.GetGear() < 3)
                        _playerValues.RiseGear();
                }
                else
                    _playerValues.DecreaseGear();
            }
        }

        //turn camera in y axis
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
        else if (move.face == FACES.B)
        {
            if (move.direction == 1)
            {
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
            }
            else 
            {
                if (!_playerValues.GetLights())
                _playerValues.NotifyAction(PlayerActions.TurnOnLights);
            }
        }
    }
}