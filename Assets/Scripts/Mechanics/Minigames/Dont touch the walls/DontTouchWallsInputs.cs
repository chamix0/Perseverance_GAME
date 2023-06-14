using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class DontTouchWallsInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private DontTouchWallsManager dontTouchWallsManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        dontTouchWallsManager = FindObjectOfType<DontTouchWallsManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.DontTouchTheWallsMinigame &&
            _playerValues.GetInputsEnabled()&&!_playerValues.GetPaused())
        {
            Vector2 dir = Vector2.zero;
            if (Input.anyKey)
            {
                dontTouchWallsManager.SetGearsZero();
                dontTouchWallsManager.ShowKeyTutorial();
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y += 1;
                dontTouchWallsManager.VerticalMovement(1, dontTouchWallsManager._speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                dir.y -= 1;
                dontTouchWallsManager.VerticalMovement(-1, dontTouchWallsManager._speed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                dir.x -= 1;
                dontTouchWallsManager.HorizontalMovement(-1, dontTouchWallsManager._speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                dir.x += 1;
                dontTouchWallsManager.HorizontalMovement(1, dontTouchWallsManager._speed);
            }

            dontTouchWallsManager.SpriteRotation(dir);
        }
    }

    public void PerformAction(Move move)
    {
        dontTouchWallsManager.ShowCubeTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.R)
                dontTouchWallsManager.VerticalMovementCube(move.direction);


            else if (move.face == FACES.U)
                dontTouchWallsManager.HorizontalMovementCube(move.direction);
        }
    }
}