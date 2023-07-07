using Main_menu;
using UnityEngine;

public class SettingsInputs : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private SettingsManager settingsManager;
    private MenuCamerasController _camerasController;
    private SaveData _saveData;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        settingsManager = FindObjectOfType<SettingsManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Settings && _myInputManager.GetInputsEnabled())
        {
            //next model
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                settingsManager.SelectPrev();

            //prev model
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                settingsManager.SelectNext();
            //prev model
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                settingsManager.IncreaseValue();

            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                settingsManager.DecreaseValue();

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
        if (move.face == FACES.R)
        {
            if (move.direction == 1)
                settingsManager.SelectPrev();
            else
                settingsManager.SelectNext();
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
                settingsManager.IncreaseValue();
            }
            else
            {
                settingsManager.DecreaseValue();
            }
        }
    }
}