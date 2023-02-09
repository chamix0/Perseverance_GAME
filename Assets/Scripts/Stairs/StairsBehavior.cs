using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class StairsBehavior : MonoBehaviour
{
    private PlayerValues _playerValues;
    public GameObject snapPos;
    public float targetAngle;
    private Animator _animator;
    private bool endAnimation, reset;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private static readonly int Move1 = Animator.StringToHash("move");
    private BoxCollider _boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _rigidbodyOriginalConstraints = _playerValues._rigidbody.constraints;
        _animator = GetComponentInParent<Animator>();
        endAnimation = false;
        reset = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (endAnimation)
        {
            if (AnimationFinished())
            {
                _playerValues.SetCanMove(true);
                _playerValues.gameObject.transform.parent = null;
                _playerValues._rigidbody.constraints = _rigidbodyOriginalConstraints;
                _playerValues._rigidbody.isKinematic = false;
                _playerValues._rigidbody.useGravity = true;
                endAnimation = false;
                reset = true;
            }
        }

        if (reset)
        {
            if (AnimationCanReset())
            {
                _boxCollider.enabled = true;
                reset = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !endAnimation)
        {
            _playerValues.snapRotationTo(targetAngle);
            _playerValues.SnapPositionTo(snapPos.transform.position);
            _playerValues.SetCanMove(false);
            StartCoroutine(startAnimationCoroutine());
        }
    }

    bool AnimationFinished()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("UpStay");
    }

    bool AnimationCanReset()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("DownStay");
    }

    IEnumerator startAnimationCoroutine()
    {
        _playerValues._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _playerValues._rigidbody.isKinematic = true;
        _playerValues._rigidbody.useGravity = false;
        _playerValues.gameObject.transform.parent = this.transform;
        _boxCollider.enabled = false;
        yield return new WaitForSeconds(2f);
        _animator.SetTrigger(Move1);
        _animator.SetTrigger(Move1);
        endAnimation = true;
    }
}