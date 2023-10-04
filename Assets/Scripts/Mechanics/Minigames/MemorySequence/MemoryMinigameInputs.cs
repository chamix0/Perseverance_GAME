using System;
using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class MemoryMinigameInputs : MonoBehaviour,IObserver
{
  private PlayerValues _playerValues;
    private MemoryMingameManager memoryMingame;

    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        memoryMingame = FindObjectOfType<MemoryMingameManager>();
        _guiManager = FindObjectOfType<GuiManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _playerValues.AddObserver(this);
    }

    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.MemoryMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
                CursorManager.ShowCursor();

            if (_newInputs.LeftTap())
                memoryMingame.SelectPrev();

            if (_newInputs.RightTap())
                memoryMingame.SelectNext();

            if (_newInputs.SelectBasic())
                memoryMingame.SelectButton();
        }
    }

    // Update is called once per frame
    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();
            memoryMingame.Select(move.color);
        }
    }


    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode &&
            _playerValues.GetCurrentInput() == CurrentInput.MemoryMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.RightText() + "- next |" + _newInputs.LeftText() + "- prev |" + _newInputs.SelectBasicText() +
            "- select");
    }
}