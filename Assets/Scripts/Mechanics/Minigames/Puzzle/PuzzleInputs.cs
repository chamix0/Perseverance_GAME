using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class PuzzleInputs : MonoBehaviour
{
    private PuzzleManager _puzzleManager;

    private PlayerValues _playerValues;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _puzzleManager = FindObjectOfType<PuzzleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.PuzzleMinigame && _playerValues.GetInputsEnabled())
        {
            if (Input.anyKey)
            {
                _puzzleManager.ShowKeyTutorial();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
            }
        }
    }

    public void PerformAction(Move move)
    {
        _puzzleManager.ShowCubeTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.R)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftRightRow(1);
                else _puzzleManager.ShiftRightRow(-1);
            }
            else if (move.face == FACES.L)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftLeftRow(1);
                else _puzzleManager.ShiftLeftRow(-1);
            }
            else if (move.face == FACES.U)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftTopRow(1);
                else _puzzleManager.ShiftTopRow(-1);
            }
            else if (move.face == FACES.D)
            {
                if (move.direction == 1)
                    _puzzleManager.ShiftBotRow(1);
                else _puzzleManager.ShiftBotRow(-1);
            }
        }
    }
}