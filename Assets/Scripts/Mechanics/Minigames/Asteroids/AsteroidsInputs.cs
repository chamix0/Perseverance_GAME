using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class AsteroidsInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private AsteroidsManager asteroidsManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        asteroidsManager = FindObjectOfType<AsteroidsManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.AsteroidMinigame && _playerValues.GetInputsEnabled())
        {
            if (Input.anyKey)
            {
                asteroidsManager.SetGearsZero();
            }

            if (Input.GetKey(KeyCode.W))
            {
                asteroidsManager.VerticalMovement(1, asteroidsManager._speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                asteroidsManager.VerticalMovement(-1, asteroidsManager._speed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                asteroidsManager.HorizontalMovement(-1, asteroidsManager._speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                asteroidsManager.HorizontalMovement(1, asteroidsManager._speed);
            }
        }
    }

    public void PerformAction(Move move)
    {
        if (move.face == FACES.R)
            asteroidsManager.VerticalMovementCube(move.direction);


        if (move.face == FACES.U)
            asteroidsManager.HorizontalMovementCube(move.direction);
    }
}