using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

namespace Mechanics.Locker_door.Caja_fuerte
{
    public class LockerInputs : MonoBehaviour, IObserver
    {
        private LockerManager _lockerManager;
        private PlayerValues _playerValues;
        private PlayerNewInputs _newInputs;
        private GuiManager _guiManager;

        // Start is called before the first frame update
        void Start()
        {
            _playerValues = FindObjectOfType<PlayerValues>();
            _lockerManager = FindObjectOfType<LockerManager>();
            _newInputs = FindObjectOfType<PlayerNewInputs>();
            _guiManager = FindObjectOfType<GuiManager>();
            _playerValues.AddObserver(this);
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerValues.GetCurrentInput() == CurrentInput.LockerMinigame && _playerValues.GetInputsEnabled() &&
                !_playerValues.GetPaused())
            {
                if (Input.anyKey)
                    CursorManager.ShowCursor();
                if (_newInputs.CheckInputChanged())
                    UpdateTutorial();

                //select previous
                if (_newInputs.LeftTap())
                    _lockerManager.SelectPrevNumber();
                //select next
                else if (_newInputs.RightTap())
                    _lockerManager.SelectNextNumber();
                // increase value
                else if (_newInputs.UpTap())
                    _lockerManager.IncreaseValueCube();
                //decrease value
                else if (_newInputs.DownTap())
                    _lockerManager.DecreaseValueCube();
                //exit
                else if (_newInputs.ReturnBasic())
                    _lockerManager.ExitButton();
                //check
                else if (_newInputs.SelectBasic())
                    _lockerManager.CheckButton();
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
                    if (move.direction > 0)
                    {
                        _lockerManager.IncreaseValueCube();
                    }
                    else
                    {
                        _lockerManager.DecreaseValueCube();
                    }
                }
                else if (move.face == FACES.U)
                {
                    if (move.direction < 0)
                    {
                        _lockerManager.SelectNextNumber();
                    }
                    else
                    {
                        _lockerManager.SelectPrevNumber();
                    }
                }
                else if (move.face == FACES.F)
                {
                    if (move.direction < 0)
                    {
                        _lockerManager.CheckButton();
                    }
                }
                else if (move.face == FACES.B)
                {
                    if (move.direction > 0)
                    {
                        _lockerManager.ExitButton();
                    }
                }
            }
        }

        public void OnNotify(PlayerActions playerAction)
        {
            if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.LockerMinigame)
                UpdateTutorial();
        }

        private void UpdateTutorial()
        {
            _guiManager.ShowTutorial();
            _guiManager.SetTutorial(
                _newInputs.RightText() + "- Next |" + _newInputs.LeftText() +
                "- Previous |" + _newInputs.SelectBasicText() +
                _newInputs.ExitBasicText() + "- Exit ");
        }
    }
}