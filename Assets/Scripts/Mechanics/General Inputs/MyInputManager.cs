using System;
using UnityEngine;

namespace Mechanics.General_Inputs
{
    [DefaultExecutionOrder(1)]
    public class MyInputManager : MonoBehaviour
    {
        private PlayerValues _playerValues;
        private MovesQueue _inputsMoves;
        private BasicCameraMovementInputs _movementInputs;
        private RotatingWallInputs _rotatingWallInputs;
        private ColorsInputs _colorsInputs;
        private MemoryMinigameInputs _memoryMinigameInputs;
        private RollTheNutInputs _rollTheNutInputs;

        private void Awake()
        {
            try
            {
                _inputsMoves = GameObject.FindGameObjectWithTag("UserCubeManager").GetComponent<MovesQueue>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _playerValues = FindObjectOfType<PlayerValues>();
            _movementInputs = FindObjectOfType<BasicCameraMovementInputs>();
            _rotatingWallInputs = FindObjectOfType<RotatingWallInputs>();
            _colorsInputs = FindObjectOfType<ColorsInputs>();
            _memoryMinigameInputs = FindObjectOfType<MemoryMinigameInputs>();
            _rollTheNutInputs = FindObjectOfType<RollTheNutInputs>();
        }

        private void Update()
        {
            if (_inputsMoves && _inputsMoves.HasMessages() && _playerValues.GetInputsEnabled())
            {
                Move move = _inputsMoves.Dequeue();
                switch (_playerValues.GetCurrentInput())
                {
                    case CurrentInput.Movement:
                        _movementInputs.PerformAction(move);
                        break;
                    case CurrentInput.RotatingWall:
                        _rotatingWallInputs.PerformAction(move);
                        break;
                    case CurrentInput.Colors_Minigame:
                        _colorsInputs.PerformAction(move);
                        break;
                    case CurrentInput.Memory_Minigame:
                        _memoryMinigameInputs.PerformAction(move);
                        break;
                    case CurrentInput.Roll_The_Nut_Minigame:
                        break;
                    case CurrentInput.None:
                        _rollTheNutInputs.PerformAction(move);
                        break;
                }

                //depending on the action we will give control to a different input manager
            }
        }
    }
}