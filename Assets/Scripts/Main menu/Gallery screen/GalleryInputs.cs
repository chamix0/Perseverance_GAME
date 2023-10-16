using Main_menu;
using Player.Observer_pattern;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class GalleryInputs : MonoBehaviour, IObserver
{
    //components
    private MyMenuInputManager _myInputManager;
    private GalleryManager galleryManager;
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
        galleryManager = FindObjectOfType<GalleryManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
        _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _myInputManager.AddObserver(this);
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Gallery && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            //next model
            if (_newInputs.RightTap())
                galleryManager.ShowNext();

            //prev model
            else if (_newInputs.LeftTap())
                galleryManager.ShowPrev();

            //go back to menu
            else if (_newInputs.ReturnBasic())
            {
                _sounds.ReturnSound();
                _camerasController.SetCamera(MenuCameras.EDDO);
                _menuManager.CheckForContinueAndNewGame();
                _menuManager.UpdateColors();
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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode&& _myInputManager.GetCurrentInput() is CurrentMenuInput.Gallery)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManagerMainMenu.ShowTutorial();
        _guiManagerMainMenu.SetTutorial(
            _newInputs.LeftText() + "- Prev |" + _newInputs.RightText() +
            "- Next |" + _newInputs.ExitBasicText() + "- Exit ");
    }
}