using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;

namespace General_Inputs
{
    public class Generalnputs : MonoBehaviour
    {
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
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!playerValues.GetIsGrounded() && playerValues.GetCurrentInput() is CurrentInput.Movement
                            or CurrentInput.RaceMovement or CurrentInput.StealthMovement or CurrentInput.ShootMovement)
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