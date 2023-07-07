using System;
using Main_menu.Load_game_screen;
using Main_menu.New_game_screen;
using TMPro;
using UnityEngine;

public enum CurrentMenuInput
{
    Menu,
    NewGame,
    LoadGame,
    Settings,
    Gallery,
    Credits,
    Tutorial,
    None
}

[DefaultExecutionOrder(1)]
public class MyMenuInputManager : MonoBehaviour
{
    //components
    private MovesQueue _inputsMoves;
    private CurrentMenuInput _currentInput;
    private MainMenuInputs _menuInputs;
    private NewGameInputs _newGameInputs;
    private LoadGameInputs _loadGameInputs;
    private GalleryInputs _galleryInputs;
    private TutorialInputs _tutorialInputs;
    private SettingsInputs _settingsInputs;
    private CreditsInputs _creditsInputs;

    //variables
    private bool _inputsEnabled = true;

    private void Awake()
    {
        _galleryInputs = FindObjectOfType<GalleryInputs>();
        _menuInputs = GetComponent<MainMenuInputs>();
        _newGameInputs = FindObjectOfType<NewGameInputs>();
        _loadGameInputs = FindObjectOfType<LoadGameInputs>();
        _tutorialInputs = FindObjectOfType<TutorialInputs>();
        _settingsInputs = FindObjectOfType<SettingsInputs>();
        _creditsInputs = FindObjectOfType<CreditsInputs>();
        try
        {
            _inputsMoves = GameObject.FindGameObjectWithTag("UserCubeManager").GetComponent<MovesQueue>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void Start()
    {
        _currentInput = CurrentMenuInput.Menu;
    }

    private void Update()
    {
        if (_inputsMoves && _inputsMoves.HasMessages() && _inputsEnabled)
        {
           
            
            Move move = _inputsMoves.Dequeue();
            switch (_currentInput)
            {
                case CurrentMenuInput.Menu:
                    _menuInputs.PerformAction(move);
                    break;
                case CurrentMenuInput.NewGame:
                    _newGameInputs.PerformAction(move);
                    break;
                case CurrentMenuInput.LoadGame:
                    _loadGameInputs.PerformAction(move);
                    break;
                case CurrentMenuInput.Gallery:
                    _galleryInputs.PerformAction(move);
                    break;
                case CurrentMenuInput.Tutorial:
                    _tutorialInputs.PerformAction(move);
                    break;
                case CurrentMenuInput.Settings:
                    _settingsInputs.PerformAction(move);
                    break;
                case CurrentMenuInput.Credits:
                    _creditsInputs.PerformAction(move);
                    break;
            }
        }
    }

    public void SetCurrentInput(CurrentMenuInput currentInput)
    {
        _currentInput = currentInput;
    }

    public CurrentMenuInput GetCurrentInput()
    {
        return _currentInput;
    }

    public bool GetInputsEnabled()
    {
        return _inputsEnabled;
    }

    public void SetInputsEnabled(bool val)
    {
        _inputsEnabled = val;
    }
}