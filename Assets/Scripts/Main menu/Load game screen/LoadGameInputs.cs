using UnityEngine;

namespace Main_menu.Load_game_screen
{
    public class LoadGameInputs : MonoBehaviour
    {
        //components
        private MyMenuInputManager _myInputManager;
        private LoadGameManager _loadGameManager;
        private MainMenuSounds _sounds;
        private MainMenuManager _mainMenuManager;

        void Start()
        {
            _mainMenuManager = FindObjectOfType<MainMenuManager>();
            _sounds = FindObjectOfType<MainMenuSounds>();
            _myInputManager = FindObjectOfType<MyMenuInputManager>();
            _loadGameManager = FindObjectOfType<LoadGameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_myInputManager.GetCurrentInput() == CurrentMenuInput.LoadGame && _myInputManager.GetInputsEnabled())
            {
                if (Input.anyKey)
                {
                    _mainMenuManager.SetTutortialText(
                        "W - prev slot   S - nex slot   L - enable load   E - enable erase  Enter - confirm    ESC - return");
                }

                //next slot
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    _loadGameManager.SelectNextButton();

                //prev slot
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    _loadGameManager.SelectPrevButton();

                //select load
                else if (Input.GetKeyDown(KeyCode.L))
                    _loadGameManager.EnableLoad();

                //select erase
                else if (Input.GetKeyDown(KeyCode.E))
                    _loadGameManager.EnableErase();

                //select
                else if (Input.GetKeyDown(KeyCode.Return))
                    _loadGameManager.PressEnter();

                //go bak to menu
                else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.A) ||
                         Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _sounds.ReturnSound();
                    _loadGameManager.HideUI();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }
        }


        public void PerformAction(Move move)
        {
            _mainMenuManager.SetTutortialText("R - Next/prev Slot  F - select   B' - return   D - Enable load  D' - Enable Erase");
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
    }
}