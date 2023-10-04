using Main_menu;
using Player.Observer_pattern;
using UnityEngine;

public class CreditsInputs : MonoBehaviour, IObserver
{
    //components
    private MyMenuInputManager _myInputManager;
    private MenuCamerasController _camerasController;
    private MainMenuManager _menuManager;
    private MainMenuSounds _sounds;
    private PlayerNewInputs _newInputs;
    private GuiManagerMainMenu _guiManagerMainMenu;

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        _menuManager = FindObjectOfType<MainMenuManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
        _myInputManager.AddObserver(this);
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Credits && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (_newInputs.ReturnBasic())
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
        _newInputs.SetCubeAsDevice();
        if (_newInputs.CheckInputChanged())
            UpdateTutorial();

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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _myInputManager.GetCurrentInput() is CurrentMenuInput.Credits)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManagerMainMenu.ShowTutorial();
        _guiManagerMainMenu.SetTutorial(
            _newInputs.ExitBasicText() + "- Exit ");
    }
}