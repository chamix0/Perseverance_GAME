using System;
using System.Collections;
using System.Diagnostics;
using Player.Observer_pattern;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(11)]
public class EnemyBee : Enemy, IObserver
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private Rigidbody rigidbody;
    [SerializeField] private EnemyPath enemyPath;
    private LookAtPlayer lookAtPlayer;
    private int targetNode = 0, nextNode = 0;
    public float turnSpeed = 1f;
    [SerializeField] private float force = 10, oldForce;
    public ForceMode forceMode;
    private DissolveMaterials dissolveMaterials;

    public int maxLives = 10;
    //outline

    private Outline outline;
    private Stopwatch outlineTimer;
    private float outlineCooldown = 10f;
    private bool dead;

    private bool minigameStarted;

    //detecction
    [SerializeField] private LayerMask collision;


    private Stopwatch _timer;

    //shooting
    private ShootBullet _shootBullet;
    [SerializeField] private float shootCooldown = 4;
    [SerializeField] private float shootSpeed = 10;
    private Transform respawn;
    [SerializeField] private Vector3 shootOffset;

    private bool moveToPlayer;

    //sounds
    private EnemySounds enemySounds;

    private void Awake()
    {
        _timer = new Stopwatch();
        outlineTimer = new Stopwatch();
        _timer.Start();
        Physics.IgnoreLayerCollision(11, 12);
    }

    void Start()
    {
        lives = maxLives;
        outline = GetComponentInChildren<Outline>();
        enemySounds = GetComponent<EnemySounds>();
        dissolveMaterials = GetComponent<DissolveMaterials>();
        playerValues = FindObjectOfType<PlayerValues>();
        rigidbody = GetComponent<Rigidbody>();
        lookAtPlayer = GetComponentInChildren<LookAtPlayer>();
        _shootBullet = GetComponentInChildren<ShootBullet>();
        playerValues.AddObserver(this);
        outline.OutlineColor = Color.clear;
        respawn = transform.parent;
        oldForce = force;
    }

    private void Update()
    {
        if (minigameStarted && !dead)
        {
            if (outlineTimer.IsRunning)
            {
                if (outlineTimer.Elapsed.TotalSeconds > outlineCooldown)
                {
                    outlineTimer.Stop();
                    outline.OutlineColor = Color.clear;
                }
            }

            if (lives > 0)
            {
                Alert();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToTarget();
    }

    #region STATES

    private void Alert()
    {
        GoToFurtherPos();
        lookAtPlayer.LookPlayer();

        if (_timer.Elapsed.TotalSeconds > shootCooldown && !playerValues.dead)
        {
            _timer.Restart();
            int shootingMode = Random.Range(0, 10);
            if (shootingMode < 8)
            {
                _shootBullet.Shoot((playerValues.GetPos() + shootOffset) - transform.position, shootSpeed,
                    respawn.position);
            }
            else
            {
                StartCoroutine(TripleShotCoroutine());
            }
        }
    }

    #endregion

    IEnumerator TripleShotCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            _shootBullet.Shoot((playerValues.GetPos() + shootOffset) - transform.position, shootSpeed,
                respawn.position);
            yield return new WaitForSeconds(0.5f);
        }
    }


    public override void Hide()
    {
        outlineTimer.Stop();
        dissolveMaterials.DissolveOut();
        rigidbody.detectCollisions = false;
        StopWander();
    }

    private void GoToFurtherPos()
    {
        targetNode = enemyPath.GetFurthestNode(playerValues.GetPos(), targetNode);
        nextNode = targetNode;
    }

    private void StopWander()
    {
        force = 0;
    }


    private void MoveToTarget()
    {
        Vector3 dest = enemyPath.GetNodeTransform(nextNode).position;
        Vector3 direction = dest - transform.position;
        rigidbody.AddForce(direction.normalized * force, forceMode);
        BodyRotation();
        // transform.forward = rigidbody.velocity.normalized;
        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            nextNode = enemyPath.GetNextNode(nextNode, targetNode);
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

    private bool InSight()
    {
        RaycastHit auxHit;
        if (Physics.Raycast(transform.position, playerValues.GetPos() - transform.position, out auxHit,
                Mathf.Infinity, collision))
        {
            if (auxHit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }


        return false;
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            RecieveDamage();
        }
    }

    public override void RecieveDamage()
    {
        if (lives > 0)
        {
            outlineTimer.Restart();
            outline.OutlineColor = Color.red;
            lives--;
            if (lives <= 0)
                Die();
            else
            {
                _timer.Restart();
                dissolveMaterials.Hit();
            }
        }
    }

    public override void Spawn(int node)
    {
        dissolveMaterials.DissolveIn();
        nextNode = node;
        outlineTimer.Start();
        rigidbody.MovePosition(enemyPath.GetNodeTransform(node).transform.position);
        rigidbody.detectCollisions = true;
        force = oldForce;
    }

    private void Die()
    {
        //sonidos
        //efecto de particulas outline.OutlineWidth = 0;
        outlineTimer.Stop();
        dissolveMaterials.DissolveOut();
        enemySounds.PlayDieSound();
        rigidbody.detectCollisions = false;
        StopWander();
        dead = true;
    }

    public override bool GetEnemyDead()
    {
        return dead;
    }

    public override void ResetEnemy()
    {
        dissolveMaterials.DissolveIn();
        outlineTimer.Start();
        rigidbody.detectCollisions = true;
        force = oldForce;
        dead = false;
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die)
        {
            ResetEnemy();
        }
    }
}