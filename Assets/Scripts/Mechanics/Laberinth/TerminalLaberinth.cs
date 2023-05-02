using TMPro;
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

        [SerializeField] private TMP_Text terminalText; 
        // Start is called before the first frame update
        void Start()
        {
            minigameFinished = false;
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
                terminalText.SetText("Terminal operative");
            }
        }

        public bool GetMinigameFinished()
        {
            return minigameFinished;
        }
    }
}