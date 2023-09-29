using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class RollTheNutInputs : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    // Start is called before the first frame update  
    private PlayerValues _playerValues;
    private RollTheNutManager _rollTheNutManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _rollTheNutManager = FindObjectOfType<RollTheNutManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.RollTheNutMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
            {
                CursorManager.ShowCursor();
                UpdateTutorial();
            }

            if (_newInputs.RollClockWise())
                _rollTheNutManager.RollClockWise();

            else if (_newInputs.RollCounterClockWise())
                _rollTheNutManager.RollCounterClockWise();
        }
    }

    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (move.face == FACES.F)
            {
                if (move.direction == 1)
                {
                    _rollTheNutManager.RollClockWise();
                }
                else
                {
                    _rollTheNutManager.RollCounterClockWise();
                }
            }
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.RollTheNutMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.RollCWText() + "- Roll Clockwise |" + _newInputs.RollCCWText() +
            "- Roll Counter Clockwise ");
    }
}