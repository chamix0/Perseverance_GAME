using System;
using General_Inputs;
using Mechanics.Locker_door.Caja_fuerte;
using Mechanics.MiniBoss;
using UnityEngine;

namespace Mechanics.General_Inputs
{
    [DefaultExecutionOrder(1)]
    public class MyInputManager : MonoBehaviour
    {
        private PlayerValues _playerValues;

        private MovesQueue _inputsMoves;

        // private BasicCameraMovementInputs _movementInputs;
        private RotatingWallInputs _rotatingWallInputs;
        private ColorsInputs _colorsInputs;
        private MemoryMinigameInputs _memoryMinigameInputs;
        private RollTheNutInputs _rollTheNutInputs;
        private AsteroidsInputs _asteroidsInputs;
        private AdjustValuesInputs _adjustValuesInputs;
        private PushFastInputs _pushFastInputs;
        private MiniBossInputs _miniBossInputs;
        private LockerInputs _lockerInputs;
        private DontTouchWallsInputs _dontTouchWallsInputs;
        private PuzzleInputs _puzzleInputs;
        private ConversationInputs _conversationInputs;
        private PlayerMechanicsArcadeInputs _arcadeInputs;
        private UnifiedMechanicsInput _unifiedMechanicsInput;
        private ShopInputs _shopInputs;
        private UpgradeInputs _upgradeInputs;

        private GuiManager gui;
        private Generalnputs generalnputs;

        private PauseInputs pauseInputs;

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
            _rotatingWallInputs = FindObjectOfType<RotatingWallInputs>();
            _colorsInputs = FindObjectOfType<ColorsInputs>();
            _memoryMinigameInputs = FindObjectOfType<MemoryMinigameInputs>();
            _rollTheNutInputs = FindObjectOfType<RollTheNutInputs>();
            _asteroidsInputs = FindObjectOfType<AsteroidsInputs>();
            _adjustValuesInputs = FindObjectOfType<AdjustValuesInputs>();
            _pushFastInputs = FindObjectOfType<PushFastInputs>();
            _miniBossInputs = FindObjectOfType<MiniBossInputs>();
            _lockerInputs = FindObjectOfType<LockerInputs>();
            _dontTouchWallsInputs = FindObjectOfType<DontTouchWallsInputs>();
            _puzzleInputs = FindObjectOfType<PuzzleInputs>();
            _conversationInputs = FindObjectOfType<ConversationInputs>();
            gui = FindObjectOfType<GuiManager>();
            generalnputs = FindObjectOfType<Generalnputs>();
            pauseInputs = FindObjectOfType<PauseInputs>();
            _arcadeInputs = FindObjectOfType<PlayerMechanicsArcadeInputs>();
            _unifiedMechanicsInput = FindObjectOfType<UnifiedMechanicsInput>();
            _shopInputs = FindObjectOfType<ShopInputs>();
            _upgradeInputs = FindObjectOfType<UpgradeInputs>();
        }

        private void Update()
        {
            if (_inputsMoves && _inputsMoves.HasMessages())
            {
                Move move = _inputsMoves.Dequeue();
                if (move.time.TotalMilliseconds + 500 < DateTime.Now.TimeOfDay.TotalMilliseconds) return;

                if (_playerValues.GetCurrentInput() is not CurrentInput.None)
                {
                    gui.SetLastMoveText(move);
                    generalnputs.PerformAction(move);
                }

                if (!_playerValues.GetPaused())
                {
                    switch (_playerValues.GetCurrentInput())
                    {
                        case CurrentInput.Movement:
                            _unifiedMechanicsInput.PerformAction(move);
                            // _movementInputs.PerformAction(move);
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
                        case CurrentInput.StealthMovement:
                            _unifiedMechanicsInput.PerformAction(move);
                            // _stealthMovementInputs.PerformAction(move);
                            break;
                        case CurrentInput.ShootMovement:
                            // _machinegunMovementInputs.PerformAction(move);
                            _unifiedMechanicsInput.PerformAction(move);
                            break;
                        case CurrentInput.RaceMovement:
                            // _runMovementInputs.PerformAction(move);
                            _unifiedMechanicsInput.PerformAction(move);
                            break;
                        case CurrentInput.DontTouchTheWallsMinigame:
                            _dontTouchWallsInputs.PerformAction(move);
                            break;
                        case CurrentInput.PuzzleMinigame:
                            _puzzleInputs.PerformAction(move);
                            break;
                        case CurrentInput.Conversation:
                            _conversationInputs.PerformAction(move);
                            break;
                        case CurrentInput.ArcadeMechanics:
                            _arcadeInputs.PerformAction(move);
                            break;
                        case CurrentInput.Shop:
                            _shopInputs.PerformAction(move);
                            break;
                        case CurrentInput.Upgrade:
                            _upgradeInputs.PerformAction(move);
                            break;
                        case CurrentInput.None:
                            break;
                    }
                }
                else
                {
                    pauseInputs.PerformAction(move);
                }
                //depending on the action we will give control to a different input manager
            }
        }
    }
}