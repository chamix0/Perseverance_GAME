using Player.Observer_pattern;
using UnityEngine;

public class MainMenuInputs : MonoBehaviour, IObserver
{
    //components
    private MyMenuInputManager _myInputManager;
    private MainMenuManager _menuManager;
    private GuiManagerMainMenu _guiManagerMainMenu;
    private PlayerNewInputs _newInputs;

    void Start()
    {
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        _menuManager = FindObjectOfType<MainMenuManager>();
        _guiManagerMainMenu = FindObjectOfType<GuiManagerMainMenu>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _myInputManager.AddObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Menu && _myInputManager.GetInputsEnabled())
        {
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            //select next button
            if (_newInputs.UpTap())
                _menuManager.SelectPrevButton();

            //select previous button 
            else if (_newInputs.DownTap())
                _menuManager.SelectNextButton();

            //select button
            else if (_newInputs.SelectBasic() || _newInputs.RightTap())
            {
                _menuManager.PressEnter();
            }
        }
    }

    public void PerformAction(Move move)
    {
        _newInputs.SetCubeAsDevice();
        if (_newInputs.CheckInputChanged())
            UpdateTutorial();
        //move between buttons
        if (move.face == FACES.R)
        {
            if (move.direction == 1)
                _menuManager.SelectPrevButton();
            else
                _menuManager.SelectNextButton();
        }

        //select button
        if (move.face == FACES.F)
        {
            if (move.direction == 1)
                _menuManager.PressEnter();
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode && _myInputManager.GetCurrentInput() is CurrentMenuInput.Menu)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManagerMainMenu.ShowTutorial();
        _guiManagerMainMenu.SetTutorial(
            _newInputs.DownText() + "- next |" + _newInputs.UpText() +
            "- Prev |" +
            _newInputs.SelectBasicText() + "- Select ");
    }
}