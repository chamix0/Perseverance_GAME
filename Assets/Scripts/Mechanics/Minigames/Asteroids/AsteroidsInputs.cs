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
            Vector2 dir = Vector2.zero;
            if (Input.anyKey)
            {
                asteroidsManager.SetGearsZero();
                asteroidsManager.ShowKeyTutorial();
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y += 1;
                asteroidsManager.VerticalMovement(1, asteroidsManager._speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                dir.y -= 1;
                asteroidsManager.VerticalMovement(-1, asteroidsManager._speed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                dir.x -= 1;
                asteroidsManager.HorizontalMovement(-1, asteroidsManager._speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                dir.x += 1;
                asteroidsManager.HorizontalMovement(1, asteroidsManager._speed);
            }

            asteroidsManager.SpriteRotation(dir);
        }
    }

    public void PerformAction(Move move)
    {
        asteroidsManager.ShowCubeTutorial();

        if (move.face == FACES.R)
            asteroidsManager.VerticalMovementCube(move.direction);


        if (move.face == FACES.U)
            asteroidsManager.HorizontalMovementCube(move.direction);
    }
}