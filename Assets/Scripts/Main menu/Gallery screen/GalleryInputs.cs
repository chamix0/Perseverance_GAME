using System.Collections;
using System.Collections.Generic;
using Main_menu;
using Main_menu.New_game_screen;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class GalleryInputs : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private GalleryManager galleryManager;
    private MenuCamerasController _camerasController;
    private SaveData _saveData;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        galleryManager = FindObjectOfType<GalleryManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Gallery && _myInputManager.GetInputsEnabled())
        {
            //next model
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                galleryManager.ShowNext();

            //prev model
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                galleryManager.ShowPrev();

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
                galleryManager.ShowPrev();
            else
                galleryManager.ShowNext();
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
    }
}