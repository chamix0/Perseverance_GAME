using System.Diagnostics;
using Codice.Client.Commands;
using Enemies;
using Player.Observer_pattern;
using UnityEngine;
using UnityEngine.UI;

///comportamineto de este enemigo
///
/// Estado Patrol: el enemigo eleige un nodo al azar y se moverá llendo de nodo  a nodo por el camino mas corto a dicho punto
/// Estado Alerta: el personaje ha sido descubierto por el enemigo por lo que el enemigo está disparando al personaje desde el punto en el que esté y no dejará de estar en este estadoo
/// hasta que el personaje no muera quede fuera del campo de visión del enemigo.
/// Estado busqueda: el enemigo ha perdido de vista al personaje y durante cierto tiempo intentará buscar al personaje
/// Estado distraido: El enemigo destruirá el señuelo pasando totalmente del personaje hasta que lo haya destruido
///
[DefaultExecutionOrder(11)]
public class EnemyShooter : Enemy, IObserver
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private MachinegunMovementInputs machinegunMovementInputs;
    private Rigidbody rigidbody;
    [SerializeField] private EnemyShooterZone enemyShooterZone;
    [SerializeField] private EnemyPath enemyPath;
    private LookAtPlayer lookAtPlayer;
    private int targetNode, nextNode, actualNode;
    public float turnSpeed = 1f;
    private float force = 10, normalForce = 10, slowForce = 3;
    [SerializeField] private float enemyCollisionForce = 1;
    public ForceMode forceMode;
    private DissolveMaterials dissolveMaterials;
    [SerializeField] private States currentState = States.Patrol;
    [SerializeField] private ParticleSystem flash;

    private int maxLives;

    //outline
    private Outline outline;
    private Stopwatch outlineTimer;
    private float outlineCooldown = 10f;

    //distractions
    private Distraction _targetDistraction;

    //animator
    private Animator animator;
    private static readonly int Index = Animator.StringToHash("index");

    //detecction
    [SerializeField] private float radius = 2;
    [SerializeField] private LayerMask collision;
    [SerializeField] private float detectionDistance = 8;
    [SerializeField] private float searchingDistance = 12;

    private Stopwatch _timer;
    [SerializeField] private int timerCooldown = 5;

    //shooting
    private ShootBullet _shootBullet;
    [SerializeField] private float shootCooldown = 4;
    [SerializeField] private float shootSpeed = 10;
    [SerializeField] private Transform respawn;
    [SerializeField] private Vector3 shootOffset;

    [SerializeField] private bool isSearcher;
    private bool moveToPlayer;
    [SerializeField] private float minDistanceFromPlayer = 5;
    private bool isDead;

    //sounds
    private EnemySounds enemySounds;

    //health bar
    [SerializeField] private CanvasGroup healthBarCanvas;
    [SerializeField] private Slider healthBar;

    private void Awake()
    {
        _timer = new Stopwatch();
        outlineTimer = new Stopwatch();
        _timer.Start();
        Physics.IgnoreLayerCollision(11, 12);
    }

    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        enemySounds = GetComponent<EnemySounds>();
        dissolveMaterials = GetComponent<DissolveMaterials>();
        _targetDistraction = FindObjectOfType<Distraction>();
        playerValues = FindObjectOfType<PlayerValues>();
        machinegunMovementInputs = FindObjectOfType<MachinegunMovementInputs>();
        machinegunMovementInputs.AddObservers(this);
        playerValues.AddObserver(this);
        rigidbody = GetComponent<Rigidbody>();
        lookAtPlayer = GetComponentInChildren<LookAtPlayer>();
        animator = GetComponentInChildren<Animator>();
        _shootBullet = GetComponentInChildren<ShootBullet>();

        outline.OutlineColor = Color.clear;
        ChangeState(States.Patrol, 0);
        maxLives = lives;
        totalLives = lives;
    }

    private void Update()
    {
        if (outlineTimer.IsRunning)
        {
            if (outlineTimer.Elapsed.TotalSeconds > outlineCooldown)
            {
                outlineTimer.Stop();
                outlineTimer.Reset();
                outline.OutlineColor = Color.clear;
            }
        }

        if (lives > 0)
        {
            switch (currentState)
            {
                case States.Patrol:
                    Patrol();
                    break;
                case States.Searching:
                    Searching();
                    break;
                case States.DestroyDistraction:
                    DestroyDistraction();
                    break;
                case States.Alert:
                    Alert();
                    break;
            }
        }
        else
        {
            if (animator.GetInteger(Index) != 0)
                animator.SetInteger(Index, 0);
            
        }
    }

    [SerializeField] Transform head;

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, detectionDistance);
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, searchingDistance);
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position, radius);
        // Gizmos.DrawRay(transform.position, -head.forward);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToTarget();
        if (moveToPlayer)
            MoveToPlayer();
    }

    #region STATES

    private void Patrol()
    {
        Wander();
        lookAtPlayer.LookNode(enemyPath.GetNodeTransform(nextNode));


        if (CheckIsInCone())
            ChangeState(States.Alert, 2);


        if (InSight() && Vector3.Distance(playerValues.GetPos(), transform.position) < searchingDistance)
        {
            _timer.Restart();
            ChangeState(States.Searching, 1);
        }

        if (_targetDistraction.GetBeingUsed() && InSight(_targetDistraction.transform.position, "Distraction"))
        {
            ChangeState(States.DestroyDistraction, 2);
            _timer.Restart();
        }
    }

    private void Searching()
    {
        SlowDown();
        lookAtPlayer.LookPlayerSlow();

        if (_timer.Elapsed.TotalSeconds > timerCooldown)
        {
            _timer.Restart();
            ChangeState(States.Patrol, 0);
        }

        if (CheckIsInCone())
        {
            ChangeState(States.Alert, 2);
        }

        if (_targetDistraction.GetBeingUsed() && InSight(_targetDistraction.transform.position, "Distraction"))
        {
            ChangeState(States.DestroyDistraction, 2);
            _timer.Restart();
        }
    }

    private void DestroyDistraction()
    {
        StopWander();
        lookAtPlayer.LookNode(_targetDistraction.transform);
        if (_timer.Elapsed.TotalSeconds > shootCooldown * 3)
        {
            _timer.Restart();
            _shootBullet.Shoot(_targetDistraction.transform.position - _shootBullet.transform.position,
                shootSpeed);
        }

        if (!InSight(_targetDistraction.transform.position, "Distraction") ||
            Vector3.Distance(_targetDistraction.transform.position, transform.position) > searchingDistance)
        {
            _timer.Restart();
            ChangeState(States.Patrol, 0);
        }
    }

    private void Alert()
    {
        StopWander();
        if (isSearcher)
            moveToPlayer = true;

        lookAtPlayer.LookPlayer();

        if (_timer.Elapsed.TotalSeconds > shootCooldown && !playerValues.dead)
        {
            _timer.Restart();
            _shootBullet.Shoot((playerValues.GetPos() + shootOffset) - transform.position, shootSpeed,
                respawn.position);
        }

        if (!InSight())
        {
            if (moveToPlayer)
            {
                moveToPlayer = false;
                nextNode = enemyPath.GetClosestNode(transform.position, nextNode);
            }

            _timer.Restart();
            ChangeState(States.Searching, 1);
        }

        if (_targetDistraction.GetBeingUsed() && InSight(_targetDistraction.transform.position, "Distraction"))
        {
            if (moveToPlayer)
            {
                moveToPlayer = false;
                nextNode = enemyPath.GetClosestNode(transform.position, nextNode);
            }

            moveToPlayer = false;
            ChangeState(States.DestroyDistraction, 2);
            _timer.Restart();
        }
    }

    #endregion


    public void SetPos(Vector3 pos)
    {
        rigidbody.MovePosition(pos);
    }


    private void StopWander()
    {
        force = 0;
    }

    private void Wander()
    {
        force = normalForce;
    }

    private void SlowDown()
    {
        force = slowForce;
    }

    private void ChangeState(States s, int index)
    {
        currentState = s;
        if (s is States.Alert)
            enemySounds.PlayAlertSound();
        else if (s is States.Searching)
            enemySounds.PlaySearchingSound();
        animator.SetInteger(Index, index);
    }

    private void MoveToTarget()
    {
        Vector3 dest = new Vector3(enemyPath.GetNodeTransform(nextNode).position.x, 0,
            enemyPath.GetNodeTransform(nextNode).position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = dest - pos;
        rigidbody.AddForce(direction.normalized * force, forceMode);
        BodyRotation();
        // transform.forward = rigidbody.velocity.normalized;
        if (Vector3.Distance(pos, dest) < 0.7f)
        {
            if (nextNode == targetNode)
            {
                //assign new target
                targetNode = enemyShooterZone.GetNewTarget(targetNode);
                nextNode = enemyPath.GetNextNode(nextNode, targetNode);
            }
            else
            {
                nextNode = enemyPath.GetNextNode(nextNode, targetNode);
            }
        }
    }

    private void MoveToPlayer()
    {
        Vector3 dest = playerValues.GetPos();
        Vector3 direction = dest - transform.position;
        float distance = Vector3.Distance(dest, transform.position);
        if (distance > minDistanceFromPlayer)
        {
            rigidbody.AddForce(direction.normalized * normalForce, forceMode);
            BodyRotation();
        }
    }

    private void BodyRotation()
    {
        Quaternion lookAtRotation = Quaternion.identity;

        lookAtRotation = Quaternion.LookRotation(transform.position - enemyPath.GetNodeTransform(nextNode).position);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(0, lookAtRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, LookAtRotationOnly_Y, Time.deltaTime * turnSpeed);
    }

    #region Detection

    private bool CheckIsInCone()
    {
        //transition
        RaycastHit[] auxHit = Physics.SphereCastAll(transform.position, radius,
            -head.forward,
            searchingDistance, collision);
        foreach (var h in auxHit)
        {
            if (h.transform.CompareTag("Player") && InSight()) return true;
        }

        return false;
    }

    private bool InSight()
    {
        RaycastHit auxHit;
        if (Physics.Raycast(transform.position, playerValues.GetPos() - transform.position, out auxHit,
                searchingDistance, collision))
        {
            if (auxHit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }


        return false;
    }

    private bool InSight(Vector3 distractionPos, string tag)
    {
        RaycastHit auxHit;
        if (Physics.Raycast(transform.position, distractionPos - transform.position, out auxHit,
                searchingDistance, collision))
            if (auxHit.transform.gameObject.CompareTag(tag))
            {
                return true;
            }

        return false;
    }

    #endregion

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Shoot)
        {
            if (Vector3.Distance(playerValues.GetPos(), transform.position) < searchingDistance)
            {
                if (currentState != States.Alert)
                    ChangeState(States.Alert, 2);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            RecieveDamage();
        }
        else if (collision.gameObject.layer == 11)
        {
            rigidbody.AddForce(rigidbody.transform.right * enemyCollisionForce, ForceMode.Impulse);
        }
    }

    public override void RecieveDamage()
    {
        UpdateHealthBar();
        if (lives > 0)
        {
            lives--;
            if (lives <= 0)
            {
                Die();
            }
            else
            {
                outlineTimer.Restart();
                outline.OutlineColor = Color.red;
                dissolveMaterials.Hit();
                if (currentState != States.Alert)
                    ChangeState(States.Alert, 2);
            }
        }
    }

    public override void Hide()
    {
        healthBarCanvas.alpha = 0;
        outlineTimer.Stop();
        outline.OutlineMode = Outline.Mode.OutlineHidden;
        flash.Stop();
        ChangeState(States.Off, 0);
        rigidbody.detectCollisions = false;
        StopWander();
        dissolveMaterials.DissolveOut();
    }

    public override void Spawn(int node)
    {
        healthBarCanvas.alpha = 1;
        dissolveMaterials.DissolveIn();
        outlineTimer.Start();
        rigidbody.detectCollisions = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        nextNode = node;
        rigidbody.MovePosition(enemyPath.GetNodeTransform(node).transform.position);
        healthBar.value = 1;
        currentState = States.Patrol;
    }


    public override bool GetEnemyDead()
    {
        return isDead;
    }

    public override void ResetEnemy()
    {
        healthBarCanvas.alpha = 1;
        dissolveMaterials.DissolveIn();
        outlineTimer.Start();
        rigidbody.detectCollisions = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        lives = maxLives;
        UpdateHealthBar();
        isDead = false;
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)lives / maxLives;
    }

    private void Die()
    {
        healthBarCanvas.alpha = 0;
        outlineTimer.Stop();
        outline.OutlineMode = Outline.Mode.OutlineHidden;
        flash.Stop();
        ChangeState(States.Off, 0);
        enemySounds.PlayDieSound();
        rigidbody.detectCollisions = false;
        StopWander();
        dissolveMaterials.DissolveOut();
        isDead = true;
    }
}