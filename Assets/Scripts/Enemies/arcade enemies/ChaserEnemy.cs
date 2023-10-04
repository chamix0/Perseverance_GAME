using System;
using System.Collections;
using Arcade.Mechanics.Bullets;
using UnityEngine;
using UnityEngine.AI;
using UTILS;
using Slider = UnityEngine.UI.Slider;

public class ChaserEnemy : Enemy
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private ArcadePlayerData _playerData;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private DissolveMaterials dissolveMaterials;
    [SerializeField] private ParticleSystem flash;

    //navmesh 
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [Header("Enemy values")] [SerializeField]
    private float speed = 0;

    [SerializeField] private int damage = 1;
    [SerializeField] private int maxLives;
    private MyStopWatch damageTimer;
    [SerializeField] private float damageColdown = 2;
    private MyStopWatch slowTimer, freezeTimer, burnTimer;
    [SerializeField] private float slowColdown = 0.5f, freezeCooldown = 5, burnCooldown = 0.5f;
    private int burnCount = 0, BurnMaxCount = 5;
    private bool slow, freeze, burn, decoy;
    private MyStopWatch laserTimer;
    private float laserDamageCooldown = 0.5f;

    [SerializeField] private float minDistToPlayer = 1;

    //outline
    [SerializeField] private Outline outline;
    private MyStopWatch outlineTimer;
    private float outlineCooldown = 10f;


    //sounds
    [SerializeField] private EnemySounds enemySounds;

    //health bar
    [SerializeField] private CanvasGroup healthBarCanvas;
    [SerializeField] private Slider healthBar;

    private void Awake()
    {
        outlineTimer = gameObject.AddComponent<MyStopWatch>();
        damageTimer = gameObject.AddComponent<MyStopWatch>();
        slowTimer = gameObject.AddComponent<MyStopWatch>();
        freezeTimer = gameObject.AddComponent<MyStopWatch>();
        burnTimer = gameObject.AddComponent<MyStopWatch>();
        laserTimer = gameObject.AddComponent<MyStopWatch>();
        damageTimer.StartStopwatch();
    }

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
        outline.OutlineColor = Color.clear;
        lives = maxLives;
        totalLives = lives;
        _navMeshAgent.speed = speed;
        isDead = true;
    }


    private void Update()
    {
        if (outlineTimer.IsRunning())
        {
            if (outlineTimer.GetElapsedSeconds() > outlineCooldown)
            {
                outlineTimer.Stop();
                outlineTimer.ResetStopwatch();
                outline.OutlineColor = Color.clear;
            }
        }

        if (slow && slowTimer.GetElapsedSeconds() > slowColdown)
        {
            slow = false;
            slowTimer.Stop();
            slowTimer.ResetStopwatch();
            if (!freeze)
                _navMeshAgent.speed = speed;
        }

        if (freeze && freezeTimer.GetElapsedSeconds() > freezeCooldown)
        {
            freeze = false;
            freezeTimer.Stop();
            freezeTimer.ResetStopwatch();
            dissolveMaterials.UnFreeze();
            _navMeshAgent.speed = speed;
        }

        if (burnCount > BurnMaxCount)
        {
            burn = false;
            burnCount = 0;
            burnTimer.Stop();
            burnTimer.ResetStopwatch();
        }
        else if (burn && burnTimer.GetElapsedSeconds() > burnCooldown)
        {
            burnTimer.Restart();
            burnCount++;
            RecieveDamage(1);
        }

        if (Vector3.Distance(transform.position, playerValues.GetPos()) < minDistToPlayer && !isDead)
        {
            _navMeshAgent.speed = 0;
            if (damageTimer.GetElapsedSeconds() > damageColdown)
            {
                playerValues.RecieveDamage(playerValues.GetPos(), damage);
                damageTimer.Restart();
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!decoy && !isDead)
            MoveToTarget();
    }


    public void SetPos(Vector3 pos)
    {
        _navMeshAgent.Warp(pos);
    }

    private void MoveToTarget()
    {
        Vector3 dest = playerValues.GetPos();
        _navMeshAgent.SetDestination(dest);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.transform.GetComponent<Bullet>();
            if (!bullet.GetEnemyHit())
            {
                bullet.SetEnemyHit(true);
                switch (bullet.GetBulletType())
                {
                    case BulletType.NormalBullet:
                        RecieveDamage(1);
                        HitSlow();
                        break;
                    case BulletType.FreezeBullet:
                        RecieveDamage(1);
                        Freeze();
                        break;
                    case BulletType.BurnBullet:
                        Burn();
                        RecieveDamage(1);
                        HitSlow();
                        break;
                    case BulletType.GuidedBullet:
                        RecieveDamage(1);
                        HitSlow();
                        break;
                    case BulletType.InstaKillBullet:
                        RecieveDamage(maxLives);
                        break;
                    case BulletType.ShotgunBullet:
                        RecieveDamage(1);
                        HitSlow();
                        break;
                    case BulletType.ExplosiveBullet:
                        RecieveDamage(5);
                        HitSlow();
                        break;
                    case BulletType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // if (collision.gameObject.CompareTag("Player"))
        // {
        //     _navMeshAgent.speed = 0;
        //     if (damageTimer.GetElapsedSeconds() > damageColdown)
        //     {
        //         playerValues.RecieveDamage(playerValues.GetPos(), damage);
        //         damageTimer.Restart();
        //     }
        // }
    }

    // private void OnCollisionStay(Collision collisionInfo)
    // {
    //     if (collisionInfo.gameObject.CompareTag("Player"))
    //     {
    //         _navMeshAgent.speed = 0;
    //         if (damageTimer.GetElapsedSeconds() > damageColdown)
    //         {
    //             playerValues.RecieveDamage(playerValues.GetPos(), damage);
    //             damageTimer.Restart();
    //         }
    //     }
    // }

    private void OnCollisionExit(Collision other)
    {
        if (!slow && !freeze && !isDead)
        {
            _navMeshAgent.speed = speed;
        }
    }

    private void StopWander()
    {
        _navMeshAgent.speed = 0;
    }

    private void ContinueWander()
    {
        _navMeshAgent.speed = speed;
    }

    public override void HitSlow()
    {
        slow = true;
        slowTimer.Restart();
        _navMeshAgent.speed = Mathf.Min(speed / 2, _navMeshAgent.speed);
    }

    public override void Freeze()
    {
        dissolveMaterials.Freeze();
        freeze = true;
        freezeTimer.Restart();
        _navMeshAgent.speed = 0;
    }

    public override void Burn()
    {
        burnCount = 0;
        burn = true;
        burnTimer.Restart();
    }

    public override void GrenadeFreezeDamage()
    {
        RecieveDamage(6);
        Freeze();
    }

    public override void GrenadeDamage()
    {
        RecieveDamage(10);
    }

    public override void GrenadeSmoke(Vector3 decoyPos, float time)
    {
        RecieveDamage(3);
        decoy = true;
        _navMeshAgent.SetDestination(decoyPos);
        StartCoroutine(DisableDecoyCoroutine(time));
    }

    public override void RecieveLaserDamage()
    {
        if (!laserTimer.IsRunning())
        {
            laserTimer.Restart();
            RecieveDamage(15);
        }

        if (laserTimer.GetElapsedSeconds() > laserDamageCooldown)
        {
            laserTimer.Stop();
            laserTimer.ResetStopwatch();
        }
    }

    public override void RecieveDamage(int damage)
    {
        if (lives > 0)
        {
            _playerData.AddPoints(+10);
            lives -= damage;
            UpdateHealthBar();
            if (lives <= 0)
            {
                _playerData.AddPoints(+100);
                Die();
            }
            else
            {
                enemySounds.PlayHurtSound();
                outlineTimer.Restart();
                outline.OutlineColor = Color.red;
                dissolveMaterials.Hit(burn);
            }
        }
    }

    public override void Hide()
    {
        isDead = true;
        healthBarCanvas.alpha = 0;
        outlineTimer.Stop();
        outline.OutlineMode = Outline.Mode.OutlineHidden;
        flash.Stop();
        rigidbody.detectCollisions = false;
        StopWander();
        dissolveMaterials.DissolveOut();
    }

    public override void Spawn(int node)
    {
        healthBarCanvas.alpha = 1;
        dissolveMaterials.DissolveIn();
        outlineTimer.StartStopwatch();
        rigidbody.detectCollisions = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        healthBar.value = 1;
    }

    public override bool GetEnemyDead()
    {
        return isDead;
    }

    public override void ResetEnemy()
    {
        isDead = false;
        healthBarCanvas.alpha = 1;
        dissolveMaterials.DissolveIn();
        outlineTimer.StartStopwatch();
        rigidbody.detectCollisions = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        //lives
        lives = maxLives;

        UpdateHealthBar();
    }

    public override void ResetEnemy(int maxLivesAux, float speedVal, int damageVal, Vector3 pos)
    {
        transform.position = new Vector3(0, -1000, 0);
        SetPos(pos);
        healthBarCanvas.alpha = 1;
        dissolveMaterials.DissolveIn();
        outlineTimer.Restart();
        rigidbody.detectCollisions = true;
        //outline
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.clear;
        flash.Play();
        //lives
        maxLives = maxLivesAux;
        lives = maxLives;
        //speed
        speed = speedVal;
        ContinueWander();
        //damage
        this.damage = damageVal;
        UpdateHealthBar();
        isDead = false;
        StartCoroutine(DelayDissolveInCoroutine());
    }

    IEnumerator DelayDissolveInCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        dissolveMaterials.DissolveIn();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)lives / maxLives;
    }

    private void Die()
    {
        _playerData.AddEnemiesKilled();
        healthBarCanvas.alpha = 0;
        outlineTimer.Stop();
        outline.OutlineMode = Outline.Mode.OutlineHidden;
        flash.Stop();
        enemySounds.PlayDieSound();
        rigidbody.detectCollisions = false;
        StopWander();
        dissolveMaterials.DissolveOut();
        isDead = true;
    }

    IEnumerator DisableDecoyCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        decoy = false;
    }
}