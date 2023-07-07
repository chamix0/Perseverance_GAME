using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;

namespace Mechanics.MiniBoss
{
    public class MiniBossInputs : MonoBehaviour
    {
        private PlayerValues _playerValues;
        private MiniBossManager _miniBossManager;
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
        }

        void Update()
        {
            if (_playerValues.GetCurrentInput() == CurrentInput.MiniBoss && _playerValues.GetInputsEnabled() &&
                !_playerValues.GetPaused())
            {
                if (Input.anyKey)
                    CursorManager.ShowCursor();
                

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
            }
        }

        public void PerformAction(Move move)
        {
            _miniBossManager.ChangeInputToCube();
            if (_playerValues.GetInputsEnabled())
            {
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
    }
}