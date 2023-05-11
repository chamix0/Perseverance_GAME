using System;
using System.Diagnostics;
using UnityEngine;

namespace General_Inputs
{
    public class Generalnputs : MonoBehaviour
    {
        private bool paused;
        private void Update()
        {
            //keyboard inputs
       
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = !paused;
                if (paused)
                {
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;

                }

            }
        }
    }
}
