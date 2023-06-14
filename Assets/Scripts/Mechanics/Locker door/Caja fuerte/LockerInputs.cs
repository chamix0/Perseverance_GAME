using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class LockerInputs : MonoBehaviour
{
    private LockerManager lockerManager;

    private PlayerValues _playerValues;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        lockerManager = FindObjectOfType<LockerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.LockerMinigame && _playerValues.GetInputsEnabled()&&!_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                lockerManager.ShowKeyTutorial();
            }
        }
    }

    public void PerformAction(Move move)
    {
        lockerManager.ShowCubeTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.R)
            {
                if (move.direction > 0)
                {
                    lockerManager.IncreaseValueCube();
                }
                else
                {
                    lockerManager.DecreaseValueCube();
                }
            }
            else if (move.face == FACES.F)
            {
                if (move.direction > 0)
                {
                    lockerManager.SelectNextNumber();
                }
                else
                {
                    lockerManager.SelectPrevNumber();
                }
            }
            else if (move.face == FACES.L)
            {
                if (move.direction < 0)
                {
                    lockerManager.CheckButton();
                }
            }
            else if (move.face == FACES.B)
            {
                if (move.direction > 0)
                {
                    lockerManager.ExitButton();
                }
            }
        }
    }
}