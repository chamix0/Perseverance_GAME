using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class ConversationInputs : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private ConversationManager _conversationManager;
    private GuiManager _guiManager;
    private PlayerNewInputs _newInputs;

    void Start()
    {
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraController = FindObjectOfType<CameraController>();
        _conversationManager = FindObjectOfType<ConversationManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.Conversation && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (_newInputs.SelectBasic())
                _conversationManager.NextDialog();

            if (_newInputs.UpTap() || _newInputs.DownTap())
                _conversationManager.ChangeAnswer();
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            if (move.face == FACES.F)
            {
                if (move.direction == 1)
                    _conversationManager.NextDialog();
            }
            else if (move.face == FACES.R)
            {
                _conversationManager.ChangeAnswer();
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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode && _playerValues.GetCurrentInput()==CurrentInput.Conversation)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.UpText() + "&" + _newInputs.DownText() + "- Change Dialog |" + _newInputs.SelectBasicText() +
            "- Select ");
    }
}