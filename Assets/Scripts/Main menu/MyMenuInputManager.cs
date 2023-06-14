using System;
using System.Collections;
using System.Collections.Generic;
using Main_menu.Load_game_screen;
using Main_menu.New_game_screen;
using UnityEngine;

public enum CurrentMenuInput
{
    Menu,
    NewGame,
    LoadGame,
    Settings,
    Gallery,
    Credits,
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

    //variables
    private bool _inputsEnabled = true;

    private void Awake()
    {
        _menuInputs = GetComponent<MainMenuInputs>();
        _newGameInputs = FindObjectOfType<NewGameInputs>();
        _loadGameInputs = FindObjectOfType<LoadGameInputs>();
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
            }

            //depending on the action we will give control to a different input manager
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