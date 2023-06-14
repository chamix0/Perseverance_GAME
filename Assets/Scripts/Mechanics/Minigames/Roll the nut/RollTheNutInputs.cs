using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class RollTheNutInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update  
    private PlayerValues _playerValues;
    private RollTheNutManager rollTheNutManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        rollTheNutManager = FindObjectOfType<RollTheNutManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.RollTheNutMinigame && _playerValues.GetInputsEnabled()&&!_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                rollTheNutManager.ShowKeyTutorial();
            }
        }
    }

    public void PerformAction(Move move)
    {
        rollTheNutManager.ShowCubeTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.F)
            {
                if (move.direction == 1)
                {
                    rollTheNutManager.RollClockWise();
                }
                else
                {
                    rollTheNutManager.RollCounterClockWise();
                }
            }
        }
    }
}