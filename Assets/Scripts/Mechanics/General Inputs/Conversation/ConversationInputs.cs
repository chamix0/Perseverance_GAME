using Mechanics.General_Inputs;
using UnityEngine;

public class ConversationInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private ConversationManager conversationManager;
    private GuiManager guiManager;

    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
        conversationManager = FindObjectOfType<ConversationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.Conversation && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                guiManager.SetTutorial("SPACE - Select/Skip     WS - Change option");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                conversationManager.NextDialog();
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow))
            {
                conversationManager.ChangeAnswer();
            }
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        guiManager.SetTutorial("F - Select/Skip     R - Change option");
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.F)
            {
                if (move.direction == 1)
                    conversationManager.NextDialog();
            }
            else if (move.face == FACES.R)
            {
                conversationManager.ChangeAnswer();
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
    }
}