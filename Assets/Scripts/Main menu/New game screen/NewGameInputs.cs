using UnityEngine;

namespace Main_menu.New_game_screen
{
    [DefaultExecutionOrder(1)]
    public class NewGameInputs : MonoBehaviour
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
        }

        void Update()
        {
            if (_myInputManager.GetCurrentInput() == CurrentMenuInput.NewGame && _myInputManager.GetInputsEnabled())
            {
                if (Input.anyKey)
                {
                    _menuManager.SetTutortialText(
                        "D/A - change skin  Enter(click out of the text box) - select   esc - return");
                }

                //next model
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    _newGameManager.ShowNext();

                //prev model
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    _newGameManager.ShowPrev();

                // //confirm model
                // else if (Input.GetKeyDown(KeyCode.Return))
                // {
                //     _sounds.SelectOptionSound();
                //     _saveData.StartNewGame(_newGameManager.GetModelIndex(), _newGameManager.GetName());
                //     _jsonSaving.SaveTheData();
                //     loadScreen.LoadLevels();
                // }
                //go back to menu
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _sounds.ReturnSound();
                    _newGameManager.HideUI();
                    _camerasController.SetCamera(MenuCameras.EDDO);
                    _menuManager.CheckForContinueAndNewGame();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
        }

        public void PerformAction(Move move)
        {
            _menuManager.SetTutortialText("U - change skin  F - select   B' - return");

            //change model
            if (move.face == FACES.U)
            {
                if (move.direction == 1)
                    _newGameManager.ShowPrev();
                else
                    _newGameManager.ShowNext();
            }
            else if (move.face == FACES.F)
            {
                //confirm and start game
                if (move.direction == 1)
                {
                    _sounds.SelectOptionSound();
                    _saveData.StartNewGame(_newGameManager.GetModelIndex(), _newGameManager.GetName());
                    _jsonSaving.SaveTheData();
                    loadScreen.LoadLevels();
                }
            }
            else if (move.face == FACES.B)
            {
                //go back to menu
                if (move.direction != 1)
                {
                    _sounds.ReturnSound();
                    _newGameManager.HideUI();
                    _camerasController.SetCamera(MenuCameras.EDDO);
                    _menuManager.CheckForContinueAndNewGame();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
        }
    }
}