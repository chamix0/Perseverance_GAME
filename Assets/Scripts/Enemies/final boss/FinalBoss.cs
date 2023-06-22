using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Player.Observer_pattern;
using UnityEngine;

[DefaultExecutionOrder(11)]
public class FinalBoss : Enemy, IObserver
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private DissolveMaterials dissolveMaterials;
    public int maxLives = 10;

    private bool dead;
    private bool minigameStarted;
    private Stopwatch _timer;

    //shooting
    [SerializeField] private float attackCooldown = 4;
    [SerializeField] private float shootSpeed = 10;
    [SerializeField] private Transform respawn;

    //attacks
    [SerializeField] private float shootSpiralRate = 0.5f;
    [SerializeField] private int shootSpirNumShots = 50;
    [SerializeField] private List<ShootBullet> spiralShooters;
    [SerializeField] private List<ShootBullet> ArrowShooters;
    [SerializeField] private List<ShootBullet> BulletHellShooters;

    //sounds
    private EnemySounds enemySounds;

    private void Awake()
    {
        _timer = new Stopwatch();
        Physics.IgnoreLayerCollision(11, 12);
    }

    void Start()
    {
        lives = maxLives;
        totalLives = maxLives;
        enemySounds = GetComponent<EnemySounds>();
        dissolveMaterials = GetComponent<DissolveMaterials>();
        playerValues = FindObjectOfType<PlayerValues>();
        playerValues.AddObserver(this);
        _timer.Start();
        minigameStarted = true;
    }

    private void Update()
    {
        if (minigameStarted && !dead)
        {
            if (lives > 0)
            {
                Alert();
            }
        }
    }


    #region STATES

    private void Alert()
    {
        if (_timer.Elapsed.TotalSeconds > attackCooldown && !playerValues.dead)
        {
            //final boss attcaks
            _timer.Stop();
            _timer.Reset();
            int shootingMode = Random.Range(0, 10);
            if (shootingMode < 10)
            {
                print("AAAAAAAAAAAAAAAA");
                StartCoroutine(SpiralAttackCoroutine());
            }
        }
    }

    #endregion

    IEnumerator SpiralAttackCoroutine()
    {
        for (int i = 0; i < shootSpirNumShots; i++)
        {
            foreach (var shooter in spiralShooters)
            {
                print("AAAAAAAAAAAAAAAA");
                shooter.Shoot(shootSpeed,respawn.position);
                yield return new WaitForSeconds(shootSpiralRate);
            }
        }

        _timer.Restart();
    }

    // IEnumerator ArrowAttackCoroutine()
    // {
    // }
    //
    // IEnumerator BulletHellAttackCoroutine()
    // {
    // }
    //
    // IEnumerator AsteroidAttackCoroutine()
    // {
    // }


    public override void Hide()
    {
        dissolveMaterials.DissolveOut();
        minigameStarted = false;
    }


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
            lives--;
            if (lives <= 0)
                Die();
            else
            {
                dissolveMaterials.Hit();
            }
        }
    }

    public override void Spawn(int node)
    {
        minigameStarted = true;
        lives = maxLives;
        dissolveMaterials.DissolveIn();
    }

    private void Die()
    {
        //sonidos
        //efecto de particulas outline.OutlineWidth = 0;
        dissolveMaterials.DissolveOut();
        enemySounds.PlayDieSound();
        dead = true;
        //desactivar el collider
    }

    public override bool GetEnemyDead()
    {
        return dead;
    }

    public override void ResetEnemy()
    {
        dissolveMaterials.DissolveIn();
        dead = false;
        lives = maxLives;
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die)
        {
            ResetEnemy();
        }
    }
}