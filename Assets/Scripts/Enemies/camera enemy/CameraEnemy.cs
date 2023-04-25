using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[DefaultExecutionOrder(5)]
public class CameraEnemy : Enemy
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
    private bool _updateCamera;
    private int _currentPatrolPoint;
    [SerializeField] private float speed = 3f;

    //CONE detector
    [SerializeField] private MeshRenderer cone;
    private Material coneMat;
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
    [SerializeField] private Animator animator;
    private static readonly int Index1 = Animator.StringToHash("index");
    private static readonly int Alpha = Shader.PropertyToID("_alpha");

    private void Awake()
    {
        _patrolPoints = new List<Transform>();
        _timer = new Stopwatch();
    }

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _targetDistraction = FindObjectOfType<Distraction>();
        coneMat = cone.sharedMaterials[0];
        coneMat.SetFloat(Alpha, 1);
        ChangeState(States.Patrol, Color.blue, 0);
        _shootBullet = GetComponentInChildren<ShootBullet>();
        // _light = GetComponentInChildren<Light>();
        foreach (var point in patrolPointContainer.gameObject.GetComponentsInChildren<Transform>())
        {
            if (point != patrolPointContainer.transform)
            {
                _patrolPoints.Add(point);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateCamera)
            SmoothllylookAt();
        //special transitions
        if (lives > 0)
        {
            //if player has the lights on and in sight
            if (_playerValues.GetLights() && InSight())
            {
                ChangeState(States.Alert, Color.red, 2);
            }

            //if its moving too fast and in sight
            if (_playerValues.GetGear() > 2 && InSight())
            {
                ChangeState(States.Alert, Color.red, 2);
            }


            if (_state is States.Patrol)
                Iddle();
            else if (_state is States.Searching)
                Searching();
            else if (_state is States.DestroyDistraction)
                DestroyDistraction();
            else if (_state is States.Alert)
                Alert();
        }

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

        if (_targetDistraction.GetBeingUsed() && InSight(_targetDistraction.transform.position, "Distraction"))
        {
            ChangeState(States.DestroyDistraction, Color.red, 2);
        }

        if (CheckIsInCone())
        {
            ChangeState(States.Alert, Color.red, 2);
        }
    }

    private void Searching()
    {
        FollowSmothly();
        //transition

        if (_targetDistraction.GetBeingUsed() && InSight(_targetDistraction.transform.position, "Distraction"))
        {
            ChangeState(States.DestroyDistraction, Color.red, 2);
        }

        if (CheckIsInCone())
        {
            ChangeState(States.Alert, Color.red, 2);
        }
        else if (_timer.Elapsed.TotalSeconds > timerCooldown)
        {
            ChangeState(States.Patrol, Color.blue, 0);
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
            _shootBullet.Shoot(_targetDistraction.transform.position - _shootBullet.transform.position,
                shootSpeed,
                respawn.position);
        }

        if (!InSight(_targetDistraction.transform.position, "Distraction") ||
            Vector3.Distance(_targetDistraction.transform.position, transform.position) > detectionDepth)
        {
            _timer.Restart();
            ChangeState(States.Searching, Color.yellow, 1);
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
            Ray ray = new Ray(_playerValues.transform.position, _playerValues.transform.forward);
            int gear = _playerValues.GetGear();
            int dist = gear > 2 ? 1 : 0;
            var position = ray.GetPoint(dist);
            _shootBullet.Shoot((position + new Vector3(0, offset, 0)) - transform.position, shootSpeed,
                respawn.position);
        }

        if (!InSight() || hit.distance > detectionDepth)
        {
            _timer.Restart();
            ChangeState(States.Searching, Color.yellow, 1);
        }
    }

    #endregion

    public void EnableEnemy()
    {
        ChangeState(States.Patrol, Color.blue, 0);
    }

    #region UTILS

    private void MoveCamera(Vector3 target)
    {
        _updateCamera = true;
        _targetPoint = target;
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
        Vector3 newPoint;
        newPoint = Vector3.MoveTowards(hit.point, _targetPoint, speed * Time.deltaTime);
        transform.LookAt(newPoint);
        print("AAAAAA");
        if (Vector3.Distance(newPoint, _targetPoint) < 0.01f || (lives > 0 && _state != States.Patrol))
        {
            print("SSSZSDFA");
            _updateCamera = false;
        }
    }

    private void FollowSmothly()
    {
        Vector3 newPoint;

        var position = _playerValues.transform.position;
        float auxSpeed = _state is States.Searching ? speed / 2 : speed;
        newPoint = Vector3.MoveTowards(hit.point, position, auxSpeed * Time.deltaTime);
        transform.LookAt(newPoint);
    }

    private void FollowSmothlyDistraction()
    {
        Vector3 newPoint = new Vector3();
        var position = _targetDistraction.transform.position;
        newPoint = Vector3.MoveTowards(hit.point, position, speed * Time.deltaTime);
        transform.LookAt(newPoint);
    }

    private void SetConeColor(Color color)
    {
        cone.material.SetColor(BackgroundColor, color);
    }

    private void ChangeState(States s, Color color, int index)
    {
        _state = s;
        SetConeColor(color);
        _light.color = color;
        animator.SetInteger(Index1, index);
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

    #endregion

    public override void RecieveDamage()
    {
        if (lives > 0)
        {
            lives--;
            if (lives <= 0)
                Die();
        }
    }

    [SerializeField] private Transform deadPos;

    private void Die()
    {
        //sonidos
        //efecto de particulas
        cone.sharedMaterial.SetFloat(Alpha, 0);
        _light.enabled = false;
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(2f);
        // speed = 100;
        MoveCamera(deadPos.position);
        //apagar la luz y la luz volumetrica
    }
}