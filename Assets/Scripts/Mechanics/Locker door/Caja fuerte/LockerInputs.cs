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
        if (_playerValues.GetCurrentInput() == CurrentInput.LockerMinigame && _playerValues.GetInputsEnabled())
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
            }
        }
    }

    public void PerformAction(Move move)
    {
        
    }
}
