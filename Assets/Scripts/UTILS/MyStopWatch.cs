using UnityEngine;

namespace UTILS
{
    public class MyStopWatch : MonoBehaviour
    {
        private float _elapsed = 0;

        private bool _stopped;

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_stopped)
                _elapsed += Time.deltaTime;
        }

        public void Stop()
        {
            _stopped = true;
        }

        public void StartStopwatch()
        {
            _stopped = false;
        }

        public void ResetStopwatch()
        {
            _elapsed = 0;
        }

        public void Restart()
        {
            ResetStopwatch();
            StartStopwatch();
        }

        public bool IsRunning()
        {
            return !_stopped;
        }

        public float GetElapsedMiliseconds()
        {
            return _elapsed * 1000;
        }

        public float GetElapsedSeconds()
        {
            return _elapsed;
        }
    }
}