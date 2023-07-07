using Main_menu;
using UnityEngine;

public class CreditsInputs : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private MenuCamerasController _camerasController;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        _menuManager = FindObjectOfType<MainMenuManager>();
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Credits && _myInputManager.GetInputsEnabled())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
        if (move.face == FACES.B)
        {
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