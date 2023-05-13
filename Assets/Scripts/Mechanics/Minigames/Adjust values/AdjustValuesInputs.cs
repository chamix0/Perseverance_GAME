using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class AdjustValuesInputs : MonoBehaviour
{
    private AdjustValuesManager _adjustValuesManager;

    private PlayerValues _playerValues;

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _adjustValuesManager = FindObjectOfType<AdjustValuesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.AdjustValuesMinigame && _playerValues.GetInputsEnabled())
        {
            if (Input.anyKey)
            {
                _adjustValuesManager.ShowKeyTutorial();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _adjustValuesManager.SelectNextSlider();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                _adjustValuesManager.SelectPrevSlider();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                _adjustValuesManager.IncreaseValue();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _adjustValuesManager.DecreaseValue();
            }
        }
    }

    public void PerformAction(Move move)
    {
        _adjustValuesManager.ShowCubeTutorial();
        if (move.face == FACES.R)
        {
            if (move.direction == 1)
                _adjustValuesManager.SelectPrevSlider();

            else
                _adjustValuesManager.SelectNextSlider();
        }

        if (move.face == FACES.F)
        {
            if (move.direction == 1)
                _adjustValuesManager.IncreaseValue();

            else
                _adjustValuesManager.DecreaseValue();
        }
    }
}