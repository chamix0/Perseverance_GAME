using System.Collections;
using System.Collections.Generic;
using Main_menu;
using Main_menu.New_game_screen;
using UnityEngine;

public class ArcadeMenuInputs : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private NewGameManager _newGameManager;
    private MenuCamerasController _camerasController;
    private ArcadeMenuManager _arcadeMenuManager;
    private JSONsaving _jsonSaving;
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
        _menuManager = FindObjectOfType<MainMenuManager>();
        _arcadeMenuManager = FindObjectOfType<ArcadeMenuManager>();
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.PreArcade && _myInputManager.GetInputsEnabled())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _sounds.ReturnSound();
                _menuManager.CheckForContinueAndNewGame();
                _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
            }
        }


        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Arcade && _myInputManager.GetInputsEnabled())
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
            //     _jsonSaving._saveData.SetArcadeModel(_newGameManager.GetModelIndex());
            //     _jsonSaving._saveData.SetArcadeName(_newGameManager.GetName());
            //     _jsonSaving.SaveTheData();
            //     loadScreen.LoadArcadeGame();
            // }
            //go back to menu
            else if (Input.GetKeyDown(KeyCode.Escape))
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