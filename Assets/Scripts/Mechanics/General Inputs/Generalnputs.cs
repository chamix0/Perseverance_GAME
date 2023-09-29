using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;

namespace General_Inputs
{
    public class Generalnputs : MonoBehaviour, InputInterface
    {
        private PlayerNewInputs _playerNewInputs;
        private PauseManager pauseManager;
        private PlayerValues playerValues;
        private Stopwatch timer;
        private bool paused;

        private void Start()
        {
            timer = new Stopwatch();
            timer.Start();
            playerValues = FindObjectOfType<PlayerValues>();
            pauseManager = FindObjectOfType<PauseManager>();
            _playerNewInputs = FindObjectOfType<PlayerNewInputs>();
        }

        private void Update()
        {
            //keyboard inputs
            if (playerValues.GetCurrentInput() is not CurrentInput.None)
            {
                if (_playerNewInputs.Pause())
                {
                    pauseManager.Pause();
                }
                else if (_playerNewInputs.Jump())
                {
                    if (!playerValues.GetIsGrounded() && playerValues.GetCurrentInput() is CurrentInput.Movement
                            or CurrentInput.RaceMovement or CurrentInput.StealthMovement or CurrentInput.ShootMovement
                            or CurrentInput.ArcadeMechanics)
                    {
                        playerValues.CheckIfStuck(false);
                        if (playerValues.GetIsStucked())
                        {
                            if (timer.Elapsed.TotalSeconds > 1)
                            {
                                timer.Restart();
                                playerValues._rigidbody.AddForce(Vector3.up * 12, ForceMode.Impulse);
                                playerValues._rigidbody.AddTorque(
                                    Vector3.Cross(Vector3.up, -playerValues.transform.up) * 10,
                                    ForceMode.Impulse);
                            }
                        }
                    }
                }
            }
        }

        public void PerformAction(Move move)
        {
            if (pauseManager.CheckCubePause(move))
                pauseManager.Pause();
        }
    }
}