using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class ColorsInputs : MonoBehaviour, IObserver
{
    private PlayerValues _playerValues;
    private ColorsManager colorsManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        colorsManager = FindObjectOfType<ColorsManager>();
        _guiManager = FindObjectOfType<GuiManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues.AddObserver(this);
    }

    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.ColorsMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
                CursorManager.ShowCursor();

            if (_newInputs.LeftTap())
                colorsManager.SelectPrev();

            if (_newInputs.RightTap())
                colorsManager.SelectNext();

            if (_newInputs.SelectBasic())
                colorsManager.SelectButton();
        }
    }

    // Update is called once per frame
    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            colorsManager.MoveFace(move);
        }
    }


    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _playerValues.GetCurrentInput() == CurrentInput.ColorsMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.RightText() + "- next |" + _newInputs.LeftText() + "- prev |" + _newInputs.SelectBasicText() +
            "- select");
    }
}