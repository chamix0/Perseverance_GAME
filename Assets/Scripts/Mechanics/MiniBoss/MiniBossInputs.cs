using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

namespace Mechanics.MiniBoss
{
    public class MiniBossInputs : MonoBehaviour, IObserver
    {
        private PlayerValues _playerValues;
        private MiniBossManager _miniBossManager;
        private GuiManager _guiManager;
        private PlayerNewInputs _newInputs;
        private KeyCode kCode; //this stores your custom key
        private Stopwatch _timer;
        private float _keyCooldown = 50;

        private List<string> alphabetList;

        private void Awake()
        {
            _timer = new Stopwatch();
            alphabetList = new List<string>(new[]
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
                "v",
                "w", "x", "y", "z"
            });
        }

        private void Start()
        {
            _playerValues = FindObjectOfType<PlayerValues>();
            _miniBossManager = FindObjectOfType<MiniBossManager>();
            _guiManager = FindObjectOfType<GuiManager>();
            _newInputs = FindObjectOfType<PlayerNewInputs>();
            _playerValues.AddObserver(this);
        }

        void Update()
        {
            if (_playerValues.GetCurrentInput() == CurrentInput.MiniBoss && _playerValues.GetInputsEnabled() &&
                !_playerValues.GetPaused())
            {
                if (_newInputs.CheckInputChanged())
                    UpdateTutorial();

                if (_miniBossManager._phase == ScreenPhase.Game)
                {
                    if (!_timer.IsRunning)
                        _timer.Start();
                    foreach (KeyCode vkey in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (_timer.Elapsed.TotalMilliseconds > _keyCooldown && Input.GetKeyDown(vkey))
                        {
                            _timer.Restart();
                            if (vkey != KeyCode.Return)
                            {
                                kCode = vkey; //this saves the key being pressed               
                                //print(kCode);
                                String letter = "" + kCode;
                                if (kCode == KeyCode.None) return;
                                if (!alphabetList.Contains(letter.ToLower())) return;
                                _miniBossManager.ProcessInput(letter);
                            }
                        }
                    }
                }
                else if (_miniBossManager._phase == ScreenPhase.Boss)
                {
                    if (_newInputs.AttackMiniBoss())
                        _miniBossManager.SelectAction(PlayerFightAction.Attack);
                    if (_newInputs.SpecialAttackMiniBoss())
                        _miniBossManager.SelectAction(PlayerFightAction.SpecialAttack);
                    if (_newInputs.DefendMiniBoss())
                        _miniBossManager.SelectAction(PlayerFightAction.Defend);
                    if (_newInputs.SpecialDefendMiniBoss())
                        _miniBossManager.SelectAction(PlayerFightAction.SpecialDefense);
                }
            }
        }

        public void PerformAction(Move move)
        {
            if (_playerValues.GetInputsEnabled())
            {
                _miniBossManager.ChangeInputToCube();
                _newInputs.SetCubeAsDevice();
                if (_newInputs.CheckInputChanged())
                    UpdateTutorial();
                if (_miniBossManager._phase == ScreenPhase.Game)
                {
                    _miniBossManager.ProcessInput(move);
                }
                else if (_miniBossManager._phase == ScreenPhase.Boss)
                {
                    if (move.face == FACES.L)
                    {
                        if (move.direction > 0) _miniBossManager.SelectAction(PlayerFightAction.SpecialDefense);
                        else
                            _miniBossManager.SelectAction(PlayerFightAction.Defend);
                    }
                    else if (move.face == FACES.R)
                    {
                        if (move.direction > 0)
                            _miniBossManager.SelectAction(PlayerFightAction.Attack);
                        else
                            _miniBossManager.SelectAction(PlayerFightAction.SpecialAttack);
                    }
                }
            }
        }

        public void OnNotify(PlayerActions playerAction)
        {
            if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.MiniBoss)
                UpdateTutorial();
        }

        private void UpdateTutorial()
        {
            _guiManager.ShowTutorial();
            _guiManager.SetTutorial(
                _newInputs.AttackText() + "- Attack |" + _newInputs.SpecialAttackText() +
                "- Special attack |" +
                _newInputs.DefendText() + "- Defend |"+_newInputs.SpecialDefenseText()+"- Special defense");
        }
    }
}