using UnityEngine;

namespace Mechanics.Laberinth
{
    [DefaultExecutionOrder(3)]
    public class TerminalLaberinth : MonoBehaviour
    {
        private PlayerValues _playerValues;
        private GameObject _snapPos;
        private RigidbodyConstraints _rigidbodyOriginalConstraints;
        private bool minigameFinished;
        private MinigameManager _minigameManager;
        private CameraChanger cameraChanger;
        private PlayerAnimations _playerAnimations;

        // Start is called before the first frame update
        void Start()
        {
            minigameFinished = false;
            _playerAnimations = FindObjectOfType<PlayerAnimations>();
            cameraChanger = FindObjectOfType<CameraChanger>();
            _minigameManager = FindObjectOfType<MinigameManager>();
            _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
            _playerValues = FindObjectOfType<PlayerValues>();
        }


        // Update is called once per frame

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && !minigameFinished)
            {
                _playerValues.snapRotationTo(_snapPos.transform.eulerAngles.y);
                _playerValues.SnapPositionTo(_snapPos.transform.position);
                _playerValues.Sit();
                minigameFinished = true;
                _minigameManager.StartRandomMinigame();
                cameraChanger.SetScreenCamera();
            }
        }

        public bool GetMinigameFinished()
        {
            return minigameFinished;
        }
    }
}