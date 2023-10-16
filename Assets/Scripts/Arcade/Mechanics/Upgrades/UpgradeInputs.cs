using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class UpgradeInputs : MonoBehaviour, IObserver
{
    private UpgradeManager _upgradeManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;
    private PlayerValues _playerValues;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _upgradeManager = FindObjectOfType<UpgradeManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.Upgrade && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            //next 
            if (_newInputs.UpTap())
                _upgradeManager.SelectPrev();
            //prev 
            if (_newInputs.DownTap())
                _upgradeManager.SelectNext();
            //exit 
            if (_newInputs.ReturnBasic())
                _upgradeManager.EndShop();
            //select value
            if (_newInputs.SelectBasic())
                _upgradeManager.SelectButton();
            //re roll
            if (_newInputs.ReRoll())
                _upgradeManager.TryReroll();
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
                    _upgradeManager.SelectPrev();
                else
                    _upgradeManager.SelectNext();
            }

            else if (move.face == FACES.F)
            {
                if (move.direction == 1)
                    _upgradeManager.SelectButton();
            }

            else if (move.face == FACES.B)
            {
                if (move.direction == 1)
                    _upgradeManager.EndShop();
            }

            else if (move.face == FACES.L)
            {
                if (move.direction == 1)
                    _upgradeManager.TryReroll();
            }
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _playerValues.GetCurrentInput() == CurrentInput.Upgrade)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.UpText() + "- Prev |" + _newInputs.DownText() +
            "- Next |" + _newInputs.ReRollText() + "- Reroll |" +
            _newInputs.SelectBasicText() + "- select |" +
            _newInputs.ExitBasicText() + "- exit ");
    }
}