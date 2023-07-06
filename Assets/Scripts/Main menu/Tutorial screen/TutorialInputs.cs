using System.Collections;
using System.Collections.Generic;
using Main_menu;
using UnityEngine;

public class TutorialInputs : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private TutorialManager tutorialManager;
    private MenuCamerasController _camerasController;
    private SaveData _saveData;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        tutorialManager = FindObjectOfType<TutorialManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Tutorial && _myInputManager.GetInputsEnabled())
        {
            //next model
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                tutorialManager.ShowNext();

            //prev model
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                tutorialManager.ShowPrev();
            //prev model
            else if (Input.GetKeyDown(KeyCode.Return))
                tutorialManager.Select();

            //go back to menu
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                _sounds.ReturnSound();
                _camerasController.SetCamera(MenuCameras.EDDO);
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
                tutorialManager.ShowPrev();
            else
                tutorialManager.ShowNext();
        }
        else if (move.face == FACES.B)
        {
            //confirm and start game
            if (move.direction != 1)
            {
                _sounds.ReturnSound();
                _camerasController.SetCamera(MenuCameras.EDDO);
                _menuManager.CheckForContinueAndNewGame();
                _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
            }
        }
        else if (move.face == FACES.F)
        {
            //confirm and start game
            if (move.direction == 1)
            {
                tutorialManager.Select();
            }
        }
    }
}