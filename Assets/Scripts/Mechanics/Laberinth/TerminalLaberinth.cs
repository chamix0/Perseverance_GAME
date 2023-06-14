using System.Collections;
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
        private bool minigameFinished,minigameActuallyFinished;
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
                StartCoroutine(ChangeCameraCoroutine());
            }
        }

        IEnumerator ChangeCameraCoroutine()
        {
            yield return new WaitForSeconds(3f);
            _minigameManager.StartRandomMinigame();
            StartCoroutine(FinisghMinigameCoroutine());
            cameraChanger.SetScreenCamera();
            terminalText.SetText("Terminal operative");
        }
        IEnumerator FinisghMinigameCoroutine()
        {
            yield return new WaitUntil(_minigameManager.GetMinigameFinished);
            minigameActuallyFinished = true;
        }
        public bool GetMinigameFinished()
        {
            return minigameActuallyFinished;
        }
    }
}