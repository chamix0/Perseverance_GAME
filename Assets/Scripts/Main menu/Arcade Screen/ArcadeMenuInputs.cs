using System.Collections;
using System.Collections.Generic;
using Main_menu;
using Main_menu.New_game_screen;
using Player.Observer_pattern;
using UnityEngine;

public class ArcadeMenuInputs : MonoBehaviour, IObserver
{
    //components
    private MyMenuInputManager _myInputManager;
    private NewGameManager _newGameManager;
    private MenuCamerasController _camerasController;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;
    private GuiManagerMainMenu _guiManagerMainMenu;
    private PlayerNewInputs _newInputs;
    private JSONsaving _jsoNsaving;
    private LoadScreen _loadScreen;
    private ArcadeMenuManager _arcadeMenuManager;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        _newGameManager = FindObjectOfType<NewGameManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
        _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _loadScreen = FindObjectOfType<LoadScreen>();
        _arcadeMenuManager = FindObjectOfType<ArcadeMenuManager>();
        _myInputManager.AddObserver(this);
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.PreArcade && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.ReturnBasic())
            {
                _sounds.ReturnSound();
                _menuManager.CheckForContinueAndNewGame();
                _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
            }
            else if (_newInputs.SelectBasic())
            {
                _arcadeMenuManager.PlayButtonAction();
            }
        }


        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Arcade && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            //next model
            if (_newInputs.RightTap())
                _newGameManager.ShowNext();

            //prev model
            else if (_newInputs.LeftTap())
                _newGameManager.ShowPrev();


            else if (_newInputs.SelectBasic())
            {
                _sounds.SelectOptionSound();
                _jsoNsaving._saveData.SetArcadeModel(_newGameManager.GetModelIndex());
                _jsoNsaving._saveData.SetArcadeName(_newGameManager.GetName());
                _jsoNsaving.SaveTheData();
                _loadScreen.LoadArcadeGame();
            }

            else if (_newInputs.ReturnBasic())
            {
                _sounds.ReturnSound();
                _newGameManager.HideUI();
                _camerasController.SetCamera(MenuCameras.EDDO);
                _menuManager.CheckForContinueAndNewGame();
                _myInputManager.SetCurrentInput(CurrentMenuInput.PreArcade);
                _arcadeMenuManager.HidePlayButton();
            }
        }
    }

    public void PerformAction(Move move)
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.PreArcade && _myInputManager.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (move.face is FACES.B)
            {
                if (move.direction == 1)
                {
                    _sounds.ReturnSound();
                    _menuManager.CheckForContinueAndNewGame();
                    _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
                }
            }

            else if (move.face is FACES.F)
            {
                if (move.direction == 1)
                {
                    _arcadeMenuManager.PlayButtonAction();
                }
            }
        }


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

            if (face is FACES.F)
            {
                if (dir == 1)
                {
                    _sounds.SelectOptionSound();
                    _jsoNsaving._saveData.SetArcadeModel(_newGameManager.GetModelIndex());
                    _jsoNsaving._saveData.SetArcadeName(_newGameManager.GetName());
                    _jsoNsaving.SaveTheData();
                    _loadScreen.LoadArcadeGame();
                }
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
            _myInputManager.GetCurrentInput() is CurrentMenuInput.PreArcade)
        {
            UpdateTutorial();
            _arcadeMenuManager.ShowPlayButton();
        }
    }

    private void UpdateTutorial()
    {
        _guiManagerMainMenu.ShowTutorial();
        _guiManagerMainMenu.SetTutorial(
            _newInputs.RightText() + "- next |" + _newInputs.LeftText() +
            "- Prev |" +
            _newInputs.SelectBasicText() + "- Select |" + _newInputs.ExitBasicText() + "- return |");
    }
}