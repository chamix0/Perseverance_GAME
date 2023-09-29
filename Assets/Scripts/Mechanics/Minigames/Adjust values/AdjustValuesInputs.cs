using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class AdjustValuesInputs : MonoBehaviour, IObserver
{
    private AdjustValuesManager _adjustValuesManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;
    private PlayerValues _playerValues;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _adjustValuesManager = FindObjectOfType<AdjustValuesManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.AdjustValuesMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            //next slider
            if (_newInputs.DownTap())
                _adjustValuesManager.SelectNextSlider();
            //prev slider
            if (_newInputs.UpTap())
                _adjustValuesManager.SelectPrevSlider();
            //increase value
            if (_newInputs.RightTap())
                _adjustValuesManager.IncreaseValue();
            //decrease value
            if (_newInputs.LeftTap())
                _adjustValuesManager.DecreaseValue();
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
                    _adjustValuesManager.SelectPrevSlider();
                else
                    _adjustValuesManager.SelectNextSlider();
            }

            if (move.face == FACES.U)
            {
                if (move.direction == -1)
                    _adjustValuesManager.IncreaseValue();
                else
                    _adjustValuesManager.DecreaseValue();
            }
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.AdjustValuesMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.UpText() + "- Prev |" + _newInputs.DownText() +
            "- Next |" +
            _newInputs.RightText() + "- Increase value |" +
            _newInputs.LeftText() + "- Decrease value ");
    }
}