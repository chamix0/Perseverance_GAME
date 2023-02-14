using System;
using System.Collections;
using System.Collections.Generic;
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

public class MyMenuInputManager : MonoBehaviour
{
    private MovesQueue _inputsMoves;
    private CurrentMenuInput _currentInput;
    private bool inputsEnabled = true;
    private MainMenuInputs _menuInputs;

    private void Awake()
    {
        _menuInputs = GetComponent<MainMenuInputs>();
    }

    private void Start()
    {
        _currentInput = CurrentMenuInput.Menu;
    }

    private void Update()
    {
        if (_inputsMoves && _inputsMoves.HasMessages() && inputsEnabled)
        {
            Move move = _inputsMoves.Dequeue();
            switch (_currentInput)
            {
                case CurrentMenuInput.Menu:
                    _menuInputs.PerformAction(move);
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
        return inputsEnabled;
    }

    public void SetInputsEnabled(bool val)
    {
        inputsEnabled = val;
    }
}