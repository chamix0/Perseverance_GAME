using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class PushFastInputs : MonoBehaviour, IObserver
{
    private PushFastManager _pushFastManager;
    private PlayerValues _playerValues;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;


    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _pushFastManager = FindObjectOfType<PushFastManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.ClickFastMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
            {
                CursorManager.ShowCursor();
                UpdateTutorial();
            }

            if (_newInputs.SelectBasic())
                _pushFastManager.Click();
        }
    }

    public void PerformAction(Move move)
    {
        _newInputs.SetCubeAsDevice();
        if (_newInputs.CheckInputChanged())
            UpdateTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            _pushFastManager.Click();
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.ClickFastMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.SelectBasicText() + "- Tap");
    }
}