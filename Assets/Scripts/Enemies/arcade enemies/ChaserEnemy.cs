using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Arcade.Mechanics.Bullets;
using Mechanics.General_Inputs.Machine_gun_mode;
using Player.Observer_pattern;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ChaserEnemy : Enemy
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private ArcadePlayerData _playerData;
    private Rigidbody rigidbody;
    private DissolveMaterials dissolveMaterials;

    [SerializeField] private ParticleSystem flash;

    //navmesh 
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [Header("Enemy values")] [SerializeField]
    private float speed = 0;

    [SerializeField] private int damage = 1;
    [SerializeField] private int maxLives;
    private Stopwatch damageTimer;
    [SerializeField] private float damageColdown = 2;
    private Stopwatch slowTimer, freezeTimer, burnTimer;
    [SerializeField] private float slowColdown = 0.5f, freezeCooldown = 5, burnCooldown = 0.5f;
    private int burnCount = 0, BurnMaxCount = 5;
    private bool slow, freeze, burn, decoy;

    private Stopwatch laserTimer;
    private float laserDamageCooldown = 0.5f;

    //outline
    private Outline outline;
    private Stopwatch outlineTimer;
    private float outlineCooldown = 10f;


    //sounds
    private EnemySounds enemySounds;

    //health bar
    [SerializeField] private CanvasGroup healthBarCanvas;
    [SerializeField] private Slider healthBar;

    private void Awake()
    {
        outlineTimer = new Stopwatch();
        damageTimer = new Stopwatch();
        slowTimer = new Stopwatch();
        freezeTimer = new Stopwatch();
        burnTimer = new Stopwatch();
        laserTimer = new Stopwatch();
        damageTimer.Start();
    }

    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        enemySounds = GetComponent<EnemySounds>();
        dissolveMaterials = GetComponent<DissolveMaterials>();
        playerValues = FindObjectOfType<PlayerValues>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
        rigidbody = GetComponent<Rigidbody>();
        outline.OutlineColor = Color.clear;
        lives = maxLives;
        totalLives = lives;
        _navMeshAgent.speed = speed;
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

        if (slow && slowTimer.Elapsed.TotalSeconds > slowColdown)
        {
            slow = false;
            slowTimer.Stop();
            slowTimer.Reset();
            if (!freeze)
                _navMeshAgent.speed = speed;
        }

        if (freeze && freezeTimer.Elapsed.TotalSeconds > freezeCooldown)
        {
            freeze = false;
            freezeTimer.Stop();
            freezeTimer.Reset();
            dissolveMaterials.UnFreeze();
            _navMeshAgent.speed = speed;
        }

        if (burnCount > BurnMaxCount)
        {
            burn = false;
            burnCount = 0;
            burnTimer.Stop();
            burnTimer.Reset();
        }
        else if (burn && burnTimer.Elapsed.TotalSeconds > burnCooldown)
        {
            burnTimer.Restart();
            burnCount++;
            RecieveDamage(1);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!decoy)
            MoveToTarget();
    }


    public void SetPos(Vector3 pos)
    {
        rigidbody.MovePosition(pos);
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

    private void OnCollisionStay(Collision collisionInfo)
    {
        _navMeshAgent.speed = 0;
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            if (damageTimer.Elapsed.TotalSeconds > damageColdown)
            {
                playerValues.RecieveDamage(playerValues.GetPos(), damage);
                damageTimer.Restart();
            }
        }
    }

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
        if (!laserTimer.IsRunning)
        {
            laserTimer.Restart();
            RecieveDamage(15);
        }

        if (laserTimer.Elapsed.TotalSeconds > laserDamageCooldown)
        {
            laserTimer.Stop();
            laserTimer.Reset();
        }
    }

    public override void RecieveDamage(int damage)
    {
        UpdateHealthBar();
        if (lives > 0)
        {
            _playerData.AddPoints(+10);
            lives -= damage;
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
        outlineTimer.Start();
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
        outlineTimer.Start();
        rigidbody.detectCollisions = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        //lives
        lives = maxLives;

        UpdateHealthBar();
    }

    public override void ResetEnemy(int maxLivesAux, float speed, int damage, Vector3 pos)
    {
        SetPos(pos);
        isDead = false;
        healthBarCanvas.alpha = 1;
        dissolveMaterials.DissolveIn();
        outlineTimer.Start();
        rigidbody.detectCollisions = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        //lives
        maxLives = maxLivesAux;
        lives = maxLives;
        //speed
        this.speed = speed;
        ContinueWander();
        //damage
        this.damage = damage;
        UpdateHealthBar();
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