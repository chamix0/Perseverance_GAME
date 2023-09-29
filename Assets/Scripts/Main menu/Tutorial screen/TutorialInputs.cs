using Main_menu;
using Player.Observer_pattern;
using UnityEngine;

public class TutorialInputs : MonoBehaviour, IObserver
{
    //components
    private MyMenuInputManager _myInputManager;
    private TutorialManager tutorialManager;
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
        tutorialManager = FindObjectOfType<TutorialManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuManager = FindObjectOfType<MainMenuManager>();
        _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _myInputManager.AddObserver(this);
    }

    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Tutorial && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            //next model
            if (_newInputs.DownTap())
                tutorialManager.ShowNext();
            //prev model
            else if (_newInputs.UpTap())
                tutorialManager.ShowPrev();
            //prev model
            else if (_newInputs.SelectBasic())
                tutorialManager.Select();
            //go back to menu
            else if (_newInputs.ReturnBasic())
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

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _myInputManager.GetCurrentInput() is CurrentMenuInput.Tutorial)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManagerMainMenu.ShowTutorial();
        _guiManagerMainMenu.SetTutorial(
            _newInputs.DownText() + "- next |" + _newInputs.UpText() +
            "- Prev |" +
            _newInputs.SelectBasicText() + "- Select |" + _newInputs.ExitBasicText() + "- return ");
    }
}