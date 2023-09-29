using Player.Observer_pattern;
using UnityEngine;

namespace Main_menu.New_game_screen
{
    [DefaultExecutionOrder(1)]
    public class NewGameInputs : MonoBehaviour, IObserver
    {
        //components
        private MyMenuInputManager _myInputManager;
        private NewGameManager _newGameManager;
        private MenuCamerasController _camerasController;
        private JSONsaving _jsonSaving;
        private SaveData _saveData;
        private MainMenuManager _menuManager;
        private LoadScreen loadScreen;
        private MainMenuSounds _sounds;
        private GuiManagerMainMenu _guiManagerMainMenu;
        private PlayerNewInputs _newInputs;
        private JSONsaving _jsoNsaving;

        void Start()
        {
            _sounds = FindObjectOfType<MainMenuSounds>();
            loadScreen = FindObjectOfType<LoadScreen>();
            _myInputManager = FindObjectOfType<MyMenuInputManager>();
            _newGameManager = FindObjectOfType<NewGameManager>();
            _camerasController = FindObjectOfType<MenuCamerasController>();
            _jsonSaving = FindObjectOfType<JSONsaving>();
            _saveData = _jsonSaving._saveData;
            _menuManager = FindObjectOfType<MainMenuManager>();
            _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
            _newInputs = FindObjectOfType<PlayerNewInputs>();
            _jsoNsaving = FindObjectOfType<JSONsaving>();
            _myInputManager.AddObserver(this);
        }

        void Update()
        {
            if (_myInputManager.GetCurrentInput() == CurrentMenuInput.NewGame && _myInputManager.GetInputsEnabled())
            {
                if (_newInputs.CheckInputChanged())
                    UpdateTutorial();

                if (_newInputs.RightTap())
                    _newGameManager.ShowNext();

                //prev model
                else if (_newInputs.LeftTap())
                    _newGameManager.ShowPrev();


                else if (_newInputs.ReturnBasic())
                {
                    _sounds.ReturnSound();
                    _newGameManager.HideUI();
                    _camerasController.SetCamera(MenuCameras.EDDO);
                    _menuManager.CheckForContinueAndNewGame();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.PreArcade);
                    _newGameManager.HideUI();
                }
            }
        }

        public void PerformAction(Move move)
        {
            if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Arcade && _myInputManager.GetInputsEnabled())
            {
                FACES face = move.face;
                int dir = move.direction;
                _newInputs.SetCubeAsDevice();
                if (_newInputs.CheckInputChanged())
                    UpdateTutorial();

                //next model
                if (face is FACES.U)
                {
                    if (dir == -1)
                        _newGameManager.ShowNext();
                    else
                        //prev model
                        _newGameManager.ShowPrev();
                }


                if (face is FACES.B)
                {
                    if (dir == 1)
                    {
                        _sounds.ReturnSound();
                        _newGameManager.HideUI();
                        _camerasController.SetCamera(MenuCameras.EDDO);
                        _menuManager.CheckForContinueAndNewGame();
                        _myInputManager.SetCurrentInput(CurrentMenuInput.PreArcade);
                    }
                }
            }
        }

        public void OnNotify(PlayerActions playerAction)
        {
            if (playerAction is PlayerActions.ChangeInputMode &&
                _myInputManager.GetCurrentInput() is CurrentMenuInput.NewGame)
            {
                _newGameManager.ShowPlayButton();
                UpdateTutorial();
            }
        }

        private void UpdateTutorial()
        {
            _guiManagerMainMenu.ShowTutorial();
            _guiManagerMainMenu.SetTutorial(
                _newInputs.RightText() + "- next |" + _newInputs.LeftText() +
                "- Prev |" + _newInputs.ExitBasicText() + "- return |");
        }
    }
}