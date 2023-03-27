using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

enum States
{
    Off,
    Iddle,
    Searching,
    DestroyDistraction,
    Alert
}

public class CameraEnemy : MonoBehaviour
{
    [SerializeField] private States _state;
    private PlayerValues _playerValues;
    [SerializeField] private float offset;

    //patrol points
    [SerializeField] private GameObject patrolPointContainer;
    private List<Transform> _patrolPoints;
    private Stopwatch _timer;
    [SerializeField] private int timerCooldown = 5;
    private Vector3 _targetPoint;
    private float _tX, _tY, _tZ;
    private bool _updateCamera;
    private int _currentPatrolPoint;

    //CONE detector
    [SerializeField] private Renderer cone;
    [SerializeField] private float radius;
    [SerializeField] private float detectionDepth = 10;
    [SerializeField] private LayerMask collision;
    private RaycastHit hit;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");

    //distractions
    private Distraction _targetDistraction;

    //shooting
    private ShootBullet _shootBullet;
    [SerializeField] private float shootCooldown = 4;
    [SerializeField] private float shootSpeed = 10;
    [SerializeField] private Transform respawn;

    //light
    [SerializeField] private Light _light;

    //alerts
    [SerializeField] private List<Sprite> _sprites; //0 searching 1 alert
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _patrolPoints = new List<Transform>();
        _timer = new Stopwatch();
    }

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _targetDistraction = FindObjectOfType<Distraction>();
        ChangeState(States.Iddle, Color.blue, null);
        _shootBullet = GetComponentInChildren<ShootBullet>();
        // _light = GetComponentInChildren<Light>();
        _playerValues = FindObjectOfType<PlayerValues>();
        foreach (var point in patrolPointContainer.gameObject.GetComponentsInChildren<Transform>())
        {
            _patrolPoints.Add(point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateCamera)
            SmoothllylookAt();
        //special transitions
        //if a player throws a distraction
        if (_targetDistraction.GetBeingUsed() && InSight(_targetDistraction.transform.position, "Distraction"))
        {
            ChangeState(States.DestroyDistraction, Color.red, _sprites[1]);
        }

        //if player has the lights on and in sight
        if (_playerValues.GetLights() && InSight())
        {
            ChangeState(States.Alert, Color.red, _sprites[1]);
        }

        //if its moving too fast and in sight
        if (_playerValues.GetGear() > 2 && InSight())
        {
            ChangeState(States.Alert, Color.red, _sprites[1]);
        }


        if (_state is States.Iddle)
            Iddle();
        else if (_state is States.Searching)
            Searching();
        else if (_state is States.DestroyDistraction)
            DestroyDistraction();
        else if (_state is States.Alert)
            Alert();

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,
            Single.PositiveInfinity, collision);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hit.point, radius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionDepth);
        Gizmos.DrawRay(transform.position, hit.point - transform.position);
    }

    #region states

    private void Iddle()
    {
        if (!_timer.IsRunning)
            _timer.Start();
        if (_timer.Elapsed.TotalSeconds > timerCooldown)
        {
            _timer.Restart();
            _currentPatrolPoint = (_currentPatrolPoint + 1) % _patrolPoints.Count;
            MoveCamera(_patrolPoints[_currentPatrolPoint].position);
        }

        //transition
        if (CheckIsInCone())
        {
            ChangeState(States.Alert, Color.red, _sprites[1]);
        }
    }

    private void Searching()
    {
        FollowSmothly();
        //transition
        if (CheckIsInCone())
        {
            ChangeState(States.Alert, Color.red, _sprites[1]);
        }
        else if (_timer.Elapsed.TotalSeconds > timerCooldown)
        {
            ChangeState(States.Iddle, Color.blue, null);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void DestroyDistraction()
    {
        // transform.LookAt(_playerValues.transform.position + new Vector3(0, offset, 0));
        //shoot
        FollowSmothlyDistraction();
        if (_timer.Elapsed.TotalSeconds > shootCooldown * 3)
        {
            _timer.Restart();
            _shootBullet.Shoot(_targetDistraction.transform.position - transform.position,
                shootSpeed,
                respawn.position);
        }

        if (!InSight(_targetDistraction.transform.position, "Distraction") ||
            Vector3.Distance(_targetDistraction.transform.position, transform.position) > detectionDepth)
        {
            _timer.Restart();
            ChangeState(States.Searching, Color.yellow, _sprites[0]);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Alert()
    {
        FollowSmothly();
        // transform.LookAt(_playerValues.transform.position + new Vector3(0, offset, 0));
        //shoot
        if (_timer.Elapsed.TotalSeconds > shootCooldown)
        {
            _timer.Restart();
            _shootBullet.Shoot((hit.point + new Vector3(0, offset, 0)) - transform.position, shootSpeed,
                respawn.position);
        }

        if (!InSight() || hit.distance > detectionDepth)
        {
            _timer.Restart();
            ChangeState(States.Searching, Color.yellow, _sprites[0]);
        }
    }

    #endregion

    public void EnableEnemy()
    {
        ChangeState(States.Iddle, Color.blue, null);
    }

    #region UTILS

    private void MoveCamera(Vector3 target)
    {
        _updateCamera = true;
        _targetPoint = target;
        _tX = 0f;
        _tY = 0f;
        _tZ = 0f;
    }

    private bool InSight()
    {
        RaycastHit auxHit;
        if (Physics.Raycast(transform.position, _playerValues.transform.position - transform.position, out auxHit,
                detectionDepth, collision))
            if (auxHit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }

        return false;
    }

    private bool InSight(Vector3 distractionPos, string tag)
    {
        RaycastHit auxHit;
        if (Physics.Raycast(transform.position, distractionPos - transform.position, out auxHit,
                detectionDepth, collision))
            if (auxHit.transform.gameObject.CompareTag(tag))
            {
                return true;
            }

        return false;
    }

    private void SmoothllylookAt()
    {
        Vector3 newPoint = new Vector3();
        newPoint.x = Mathf.Lerp(hit.point.x, _targetPoint.x, _tX);
        newPoint.y = Mathf.Lerp(hit.point.y, _targetPoint.y, _tY);
        newPoint.z = Mathf.Lerp(hit.point.z, _targetPoint.z, _tZ);
        transform.LookAt(newPoint);

        _tX += 0.1f * Time.deltaTime;
        _tY += 0.1f * Time.deltaTime;
        _tZ += 0.1f * Time.deltaTime;

        if ((_tX > 1.0f && _tY > 1.0f && _tZ > 1.0f) || _state != States.Iddle)
        {
            _tX = 1.0f;
            _tY = 1.0f;
            _tZ = 1.0f;
            _updateCamera = false;
        }
    }

    private void SetConeColor(Color color)
    {
        cone.material.SetColor(BackgroundColor, color);
    }

    private void ChangeState(States s, Color color, Sprite sprite)
    {
        _state = s;
        SetConeColor(color);
        _light.color = color;
        _spriteRenderer.sprite = sprite;
    }

    private bool CheckIsInCone()
    {
        //transition
        RaycastHit[] auxHit = Physics.SphereCastAll(transform.position, radius,
            transform.TransformDirection(Vector3.forward),
            Vector3.Distance(transform.position, hit.point), collision);
        foreach (var h in auxHit)
        {
            if (h.transform.CompareTag("Player") && InSight()) return true;
        }

        return false;
    }

    private void FollowSmothly()
    {
        Vector3 newPoint = new Vector3();
        var position = _playerValues.transform.position;
        float speed = _state is States.Searching ? 0.5f : 5f;
        newPoint.x = Mathf.Lerp(hit.point.x, position.x, speed * Time.deltaTime);
        newPoint.y = Mathf.Lerp(hit.point.y, position.y, speed * Time.deltaTime);
        newPoint.z = Mathf.Lerp(hit.point.z, position.z, speed * Time.deltaTime);
        transform.LookAt(newPoint);
    }

    private void FollowSmothlyDistraction()
    {
        Vector3 newPoint = new Vector3();
        var position = _targetDistraction.transform.position;
        float speed = 3;
        newPoint.x = Mathf.Lerp(hit.point.x, position.x, speed * Time.deltaTime);
        newPoint.y = Mathf.Lerp(hit.point.y, position.y, speed * Time.deltaTime);
        newPoint.z = Mathf.Lerp(hit.point.z, position.z, speed * Time.deltaTime);
        transform.LookAt(newPoint);
    }

    #endregion
}