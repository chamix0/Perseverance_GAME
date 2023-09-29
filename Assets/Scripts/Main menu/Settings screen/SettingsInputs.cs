using Main_menu;
using Player.Observer_pattern;
using UnityEngine;

public class SettingsInputs : MonoBehaviour, IObserver
{
    //components
    private MyMenuInputManager _myInputManager;
    private SettingsManager settingsManager;
    private MenuCamerasController _camerasController;
    private SaveData _saveData;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;
    private GuiManagerMainMenu _guiManagerMainMenu;
    private PlayerNewInputs _newInputs;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        settingsManager = FindObjectOfType<SettingsManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
        _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _myInputManager.AddObserver(this);
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Settings && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();


            //next model
            if (_newInputs.UpTap())
                settingsManager.SelectPrev();
            //prev model
            else if (_newInputs.DownTap())
                settingsManager.SelectNext();
            //increase value
            else if (_newInputs.RightTap())
                settingsManager.IncreaseValue();
            //decrease value
            else if (_newInputs.LeftTap())
                settingsManager.DecreaseValue();

            //go back to menu
            else if (_newInputs.ReturnBasic())
            {
                settingsManager.HideUI();
                _sounds.ReturnSound();
                _camerasController.SetCamera(MenuCameras.EDDO);
                _menuManager.CheckForContinueAndNewGame();
                _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
            }
        }
    }

    public void PerformAction(Move move)
    {
        _newInputs.SetCubeAsDevice();
        if (_newInputs.CheckInputChanged())
            UpdateTutorial();
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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&_myInputManager.GetCurrentInput() is CurrentMenuInput.Settings)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManagerMainMenu.ShowTutorial();
        _guiManagerMainMenu.SetTutorial(
            _newInputs.DownText() + "- next |" + _newInputs.UpText() +
            "- Prev |" +
            _newInputs.RightText() + "- increase value |" + _newInputs.LeftText() + "- decrease value |" + _newInputs.ExitBasicText() + "- return |");
    }
}