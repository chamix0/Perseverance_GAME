using System;
using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;

namespace General_Inputs
{
    public class Generalnputs : MonoBehaviour
    {
        private PauseManager pauseManager;
        private PlayerValues playerValues;
        private bool paused;

        private void Start()
        {
            playerValues = FindObjectOfType<PlayerValues>();
            pauseManager = FindObjectOfType<PauseManager>();
        }

        private void Update()
        {
            //keyboard inputs
            if (playerValues.GetCurrentInput() is not CurrentInput.None)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    pauseManager.Pause();
                }
            }
        }

        public void PerformAction(Move move)
        {
            if (pauseManager.CheckCubePause(move))
            {
                pauseManager.Pause();
            }
        }
    }
}