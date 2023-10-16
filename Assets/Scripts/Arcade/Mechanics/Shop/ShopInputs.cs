using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class ShopInputs : MonoBehaviour, IObserver
{
    private BulletShopManager _bulletShopManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;
    private PlayerValues _playerValues;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _bulletShopManager = FindObjectOfType<BulletShopManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.Shop && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            //next 
            if (_newInputs.RightTap())
                _bulletShopManager.SelectNext();
            //prev 
            if (_newInputs.LeftTap())
                _bulletShopManager.SelectPrev();
            //exit 
            if (_newInputs.ReturnBasic())
                _bulletShopManager.EndShop();
            //select value
            if (_newInputs.SelectBasic())
                _bulletShopManager.SelectButton();
        }
    }

    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (move.face == FACES.U)
            {
                if (move.direction == 1)
                    _bulletShopManager.SelectPrev();
                else
                    _bulletShopManager.SelectNext();
            }

            if (move.face == FACES.F)
            {
                if (move.direction == 1)
                    _bulletShopManager.SelectButton();
            }

            if (move.face == FACES.B)
            {
                if (move.direction == 1)
                    _bulletShopManager.EndShop();
            }
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _playerValues.GetCurrentInput() == CurrentInput.Shop)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.LeftText() + "- Prev |" + _newInputs.RightText() +
            "- Next |" +
            _newInputs.SelectBasicText() + "- select |" +
            _newInputs.ExitBasicText() + "- exit ");
    }
}