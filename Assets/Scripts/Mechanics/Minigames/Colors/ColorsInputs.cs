using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class ColorsInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private ColorsManager colorsManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        colorsManager = FindObjectOfType<ColorsManager>();
    }

    // Update is called once per frame

    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            colorsManager.MoveFace(move);
        }
    }
}