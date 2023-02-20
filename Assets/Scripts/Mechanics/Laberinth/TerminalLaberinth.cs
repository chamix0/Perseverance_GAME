using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Update()
    {
        // if (endAnimation)
        // {
        //     if (AnimationFinished())
        //     {
        //         // _playerValues.SetCanMove(true);
        //         _playerValues.gameObject.transform.parent = null;
        //         _playerValues._rigidbody.constraints = _rigidbodyOriginalConstraints;
        //         _playerValues._rigidbody.isKinematic = false;
        //         _playerValues._rigidbody.useGravity = true;
        //         endAnimation = false;
        //         reset = true;
        //         StartCoroutine(EndAnimationCoroutine());
        //     }
        // }
        //
        // if (reset)
        // {
        //     if (AnimationCanReset())
        //     {
        //         _boxCollider.enabled = true;
        //         reset = false;
        //     }
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !minigameFinished)
        {
            _playerValues.snapRotationTo(transform.rotation.y + 180);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _playerValues.SetInputsEnabled(false);
            _playerValues.SetCanMove(false);
            _playerAnimations.SetSitAnim(true);
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