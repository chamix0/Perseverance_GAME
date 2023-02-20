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

        void Start()
        {
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
                //next model
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    _newGameManager.ShowNext();

                //prev model
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    _newGameManager.ShowPrev();

                //confirm model
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    _saveData.StartNewGame(_newGameManager.GetModelIndex(), _newGameManager.GetName());
                    _jsonSaving.SaveTheData();
                    Debug.Log("nueva partida creada en el  slot " + _saveData.GetLastSessionSlotIndex());
                    //cambiar de escena
                }
                //go back to menu
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _newGameManager.HideUI();
                    _camerasController.SetCamera(0);
                    _menuManager.CheckForContinueAndNewGame();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
        }

        public void PerformAction(Move move)
        {
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
                    _saveData.StartNewGame(_newGameManager.GetModelIndex(), _newGameManager.GetName());
                    _jsonSaving.SaveTheData();
                    Debug.Log("nueva partida creada en el  slot " + _saveData.GetLastSessionSlotIndex());
                    //cambiar de escena  
                    print("change scene");
                }
                //go back to menu
                else
                {
                    _newGameManager.HideUI();
                    _camerasController.SetCamera(0);
                    _menuManager.CheckForContinueAndNewGame();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
        }
    }
}