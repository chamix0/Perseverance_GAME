using Player.Observer_pattern;
using UnityEngine;

namespace Main_menu.Load_game_screen
{
    public class LoadGameInputs : MonoBehaviour, IObserver
    {
        //components
        private MyMenuInputManager _myInputManager;
        private LoadGameManager _loadGameManager;
        private MainMenuSounds _sounds;
        private MainMenuManager _mainMenuManager;
        private GuiManagerMainMenu _guiManagerMainMenu;
        private PlayerNewInputs _newInputs;

        void Start()
        {
            _mainMenuManager = FindObjectOfType<MainMenuManager>();
            _sounds = FindObjectOfType<MainMenuSounds>();
            _myInputManager = FindObjectOfType<MyMenuInputManager>();
            _loadGameManager = FindObjectOfType<LoadGameManager>();
            _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
            _newInputs = FindObjectOfType<PlayerNewInputs>();
            _myInputManager.AddObserver(this);
        }

        // Update is called once per frame
        void Update()
        {
            if (_myInputManager.GetCurrentInput() == CurrentMenuInput.LoadGame && _myInputManager.GetInputsEnabled())
            {
                if (_newInputs.CheckInputChanged())
                    UpdateTutorial();

                //next slot
                if (_newInputs.DownTap())
                    _loadGameManager.SelectNextButton();

                //prev slot
                else if (_newInputs.UpTap())
                    _loadGameManager.SelectPrevButton();

                //select load
                else if (_newInputs.EnableLoad())
                    _loadGameManager.EnableLoad();

                //select erase
                else if (_newInputs.EnableErase())
                    _loadGameManager.EnableErase();

                //select
                else if (_newInputs.SelectBasic())
                    _loadGameManager.PressEnter();

                //go bak to menu
                else if (_newInputs.LeftTap() || _newInputs.ReturnBasic())
                {
                    _sounds.ReturnSound();
                    _loadGameManager.HideUI();
                    _mainMenuManager.UpdateColors();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
        }


        public void PerformAction(Move move)
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            //selecting button
            if (move.face == FACES.R)
            {
                if (move.direction == 1)
                    _loadGameManager.SelectPrevButton();
                else
                    _loadGameManager.SelectNextButton();
            }
            else if (move.face == FACES.F)
            {
                //select
                if (move.direction == 1)
                    _loadGameManager.PressEnter();
            }
            else if (move.face == FACES.B)
            {
                if (move.direction != 1)
                {
                    _sounds.ReturnSound();
                    _loadGameManager.HideUI();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
            else if (move.face == FACES.D)
            {
                //select load
                if (move.direction == 1)
                    _loadGameManager.EnableLoad();
                //select erase
                else
                    _loadGameManager.EnableErase();
            }
        }

        public void OnNotify(PlayerActions playerAction)
        {
            if (playerAction is PlayerActions.ChangeInputMode&&_myInputManager.GetCurrentInput() is CurrentMenuInput.LoadGame)
                UpdateTutorial();
        }

        private void UpdateTutorial()
        {
            _guiManagerMainMenu.ShowTutorial();
            _guiManagerMainMenu.SetTutorial(
                _newInputs.RightText() + "- next |" + _newInputs.LeftText() +
                "- Prev |" + _newInputs.EraseText() + "- Erase |" + _newInputs.LoadText() + "- Load |" +
                _newInputs.SelectBasicText() + "- Select |" + _newInputs.ExitBasicText() + "- return |");
        }
    }
}