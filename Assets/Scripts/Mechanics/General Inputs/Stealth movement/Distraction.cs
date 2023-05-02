using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Distraction : Subject
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    public bool beingUsed;
    private Rigidbody _rigidbody;
    private bool isVisible;
    [SerializeField] private Transform iddlePos;
    [SerializeField] private float force, throwForce;
    private Renderer _renderer;
    private static readonly int Alpha = Shader.PropertyToID("_alpha");
    [SerializeField] private float throwDrag = 1, recoverDrag = 10;

    //outline
    private Outline _outline;
    private float outlineWidth;

    //cooldown
    private Stopwatch _timer;
    [SerializeField] private float distractionCooldown = 5;
    private float _targetAlpha, _tA;
    private bool _updateAlpha, _lockPos;
    private CameraChanger cameraChanger;

    private void Awake()
    {
        _timer = new Stopwatch();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _outline = GetComponent<Outline>();
        cameraChanger = FindObjectOfType<CameraChanger>();
    }


    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        AddObserver(_playerValues.GetComponent<PlayerSounds>());
        Physics.IgnoreCollision(_playerValues.GetComponent<BoxCollider>(), GetComponent<BoxCollider>());

        outlineWidth = _outline.OutlineWidth;
        _outline.OutlineWidth = 0;
        transform.parent = null;

        _renderer.material.SetFloat(Alpha, 0);
    }

    private void Update()
    {
        if (_updateAlpha)
            SmoothAlpha();
    }

    private void FixedUpdate()
    {
        if (!beingUsed)
            HoldObject();
    }


    public void ThrowDistraction()
    {
        if (!beingUsed)
        {
            NotifyObservers(PlayerActions.ThrowDistraction);
            beingUsed = true;
            _rigidbody.drag = throwDrag;
            _rigidbody.AddForce(cameraChanger.GetActiveCam().transform.forward * throwForce, ForceMode.Impulse);
            _timer.Start();
        }
    }

    public void RecuperateDistraction()
    {
        if (_timer.Elapsed.TotalSeconds > distractionCooldown)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, _playerValues.transform.position - transform.position, out hit,
                    Mathf.Infinity))
            {
                if (hit.transform.gameObject.layer != 9)
                {
                    SetAlpha(0);
                    StartCoroutine(RecuperateDistractionCoroutine());
                }
            }

            NotifyObservers(PlayerActions.RecuperateDistraction);
            _lockPos = false;
            beingUsed = false;
            _rigidbody.drag = recoverDrag;
            _timer.Stop();
            _timer.Reset();
        }
    }

    IEnumerator RecuperateDistractionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        transform.position = iddlePos.position;
        SetAlpha(1);
    }

    public void SetAlpha(float targetAlpha)
    {
        _targetAlpha = targetAlpha;
        _tA = 0;
        _updateAlpha = true;
    }

    private void SmoothAlpha()
    {
        _renderer.material.SetFloat(Alpha, Mathf.Lerp(_renderer.material.GetFloat(Alpha), _targetAlpha, _tA));
        _tA += 0.5f * Time.deltaTime;
        if (_tA > 1.0f)
        {
            _tA = 1.0f;
            _updateAlpha = false;
        }
    }

    public void SetVisible(bool val)
    {
        isVisible = val;

        if (val)
        {
            SetAlpha(0.75f);
            _outline.OutlineWidth = outlineWidth;
        }
        else
        {
            SetAlpha(0);
            _outline.OutlineWidth = 0;
        }
    }

    public bool GetIsVisible()
    {
        return isVisible;
    }

    public bool GetBeingUsed()
    {
        return beingUsed;
    }

    void HoldObject()
    {
        if (!_lockPos)
        {
            if (Vector3.Distance(transform.position, iddlePos.position) > 0.01f)
            {
                Vector3 moveDir = (iddlePos.position - transform.position);
                _rigidbody.AddForce(moveDir * force, ForceMode.Acceleration);
            }
            else
                _lockPos = true;
        }
        else
        {
            transform.position = iddlePos.position;
            _rigidbody.angularVelocity = new Vector3(1, 1, 1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            _lockPos = false;
            beingUsed = false;
            _rigidbody.drag = recoverDrag;
        }
    }
}