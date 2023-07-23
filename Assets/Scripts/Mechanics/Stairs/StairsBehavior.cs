using System.Collections;
using UnityEngine;

public class StairsBehavior : MonoBehaviour
{
    private PlayerValues _playerValues;
    private GameObject snapPos;
    private Animator _animator;
    private PlayerAnimations _playerAnimations;
    private bool endAnimation, reset;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private static readonly int Move1 = Animator.StringToHash("move");
    private BoxCollider _boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
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
                _playerValues.gameObject.transform.parent = null;
                _playerValues._rigidbody.constraints = _rigidbodyOriginalConstraints;
                _playerValues._rigidbody.isKinematic = false;
                _playerValues._rigidbody.useGravity = true;
                endAnimation = false;
                reset = true;
                _playerValues.StandUp(true, 0f);
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
        if (collision.gameObject.CompareTag("Player") && _playerValues.GetIsGrounded() && !endAnimation)
        {
            _playerValues.transform.up = Vector3.up;
            _playerValues.snapRotationTo(-snapPos.transform.rotation.eulerAngles.y);
            _playerValues.SnapPositionTo(snapPos.transform.position);
            _playerValues.Sit();
            endAnimation = true;
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
        yield return new WaitForSeconds(1f);
        _playerValues._rigidbody.useGravity = false;
        _playerValues.gameObject.transform.parent = transform;
        _boxCollider.enabled = false;
        _animator.SetTrigger(Move1);
        _animator.SetTrigger(Move1);
        endAnimation = true;
    }
}