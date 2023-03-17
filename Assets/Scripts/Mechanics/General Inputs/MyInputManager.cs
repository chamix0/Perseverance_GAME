using System;
using Mechanics.MiniBoss;
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
        private AsteroidsInputs _asteroidsInputs;
        private AdjustValuesInputs _adjustValuesInputs;
        private PushFastInputs _pushFastInputs;
        private MiniBossInputs _miniBossInputs;
        private LockerInputs _lockerInputs;


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
            _asteroidsInputs = FindObjectOfType<AsteroidsInputs>();
            _adjustValuesInputs = FindObjectOfType<AdjustValuesInputs>();
            _pushFastInputs = FindObjectOfType<PushFastInputs>();
            _miniBossInputs = FindObjectOfType<MiniBossInputs>();
            _lockerInputs = FindObjectOfType<LockerInputs>();
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
                    case CurrentInput.ColorsMinigame:
                        _colorsInputs.PerformAction(move);
                        break;
                    case CurrentInput.MemoryMinigame:
                        _memoryMinigameInputs.PerformAction(move);
                        break;
                    case CurrentInput.RollTheNutMinigame:
                        _rollTheNutInputs.PerformAction(move);
                        break;
                    case CurrentInput.AsteroidMinigame:
                        _asteroidsInputs.PerformAction(move);
                        break;
                    case CurrentInput.AdjustValuesMinigame:
                        _adjustValuesInputs.PerformAction(move);
                        break;
                    case CurrentInput.ClickFastMinigame:
                        _pushFastInputs.PerformAction(move);
                        break;
                    case CurrentInput.LockerMinigame:
                        _lockerInputs.PerformAction(move);
                        break;
                    case CurrentInput.MiniBoss:
                        _miniBossInputs.PerformAction(move);
                        break;
                    case CurrentInput.None:

                        break;
                }

                //depending on the action we will give control to a different input manager
            }
        }
    }
}