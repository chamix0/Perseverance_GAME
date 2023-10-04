using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class PuzzleInputs : MonoBehaviour, IObserver
{
    private PuzzleManager _puzzleManager;
    private PlayerValues _playerValues;
    private GuiManager _guiManager;
    private PlayerNewInputs _newInputs;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _puzzleManager = FindObjectOfType<PuzzleManager>();
        _guiManager = FindObjectOfType<GuiManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.PuzzleMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            if (Input.anyKey)
                CursorManager.ShowCursor();
            
            if (_newInputs.LeftTap())
                _puzzleManager.Left();

            if (_newInputs.RightTap())
                _puzzleManager.Right();

            if (_newInputs.UpTap())
                _puzzleManager.Up();

            if (_newInputs.DownTap())
                _puzzleManager.Down();

            if (_newInputs.SelectBasic())
                _puzzleManager.SelectButton();
        }
    }

    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            if (move.face == FACES.R)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftRightRow(1);
                else _puzzleManager.ShiftRightRow(-1);
            }
            else if (move.face == FACES.L)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftLeftRow(1);
                else _puzzleManager.ShiftLeftRow(-1);
            }
            else if (move.face == FACES.U)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftTopRow(1);
                else _puzzleManager.ShiftTopRow(-1);
            }
            else if (move.face == FACES.D)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftBotRow(1);
                else _puzzleManager.ShiftBotRow(-1);
            }
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _playerValues.GetCurrentInput() == CurrentInput.MemoryMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.UpText() + "- up |" + _newInputs.DownText() + "- down |" + _newInputs.LeftText() + "- left |" +
            _newInputs.RightText() + "- right |" + _newInputs.SelectBasicText() +
            "- select");
    }
}