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
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private DoorManager _doorManager;

    //mid live conversation
    [SerializeField] private Conversation conversation;
    private ConversationManager conversationManager;
    private bool conversationShown;
    [SerializeField] private Transform conversationFocus;
    private Objetives objetives;

    [SerializeField] private GameObject initialDialog;

    //shooting
    [SerializeField] private float attackCooldown = 4;
    [SerializeField] private float shootSpeed = 10;
    [SerializeField] private Transform respawn;

    //attacks
    [SerializeField] private float shootSpiralRate = 0.5f, bulletHellRate = 0.5f;
    [SerializeField] private int shootSpirNumShots = 50, bulletHellShots = 10;
    [SerializeField] private List<ShootBullet> spiralShooters;
    [SerializeField] private List<ShootBullet> ArrowShooters;
    [SerializeField] private List<ShootBullet> BulletHellShooters;

    //lasers
    [SerializeField] private MoveTowardsPlayer moveTowardsPlayer;
    [SerializeField] private List<LaserEnemy> laserEnemies;

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
        conversationManager = FindObjectOfType<ConversationManager>();
        playerValues.AddObserver(this);
        _timer.Start();
        minigameStarted = true;
        objetives = FindObjectOfType<Objetives>();
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
            int shootingMode = Random.Range(0, 9);


            if (shootingMode >= 6)
                StartCoroutine(SpiralAttackCoroutine());

            else if (shootingMode >= 3)
                ArrowAttack();

            else
                StartCoroutine(BulletHellAttackCoroutine());
        }
    }

    #endregion

    #region Attacks

    IEnumerator SpiralAttackCoroutine()
    {
        for (int i = 0; i < shootSpirNumShots; i++)
        {
            foreach (var shooter in spiralShooters)
            {
                shooter.Shoot(shootSpeed, respawn.position);
                yield return new WaitForSeconds(shootSpiralRate);
            }
        }

        _timer.Restart();
    }

    private void ArrowAttack()
    {
        foreach (var shooter in ArrowShooters)
        {
            shooter.Shoot(shootSpeed, respawn.position);
        }


        _timer.Restart();
    }

    IEnumerator BulletHellAttackCoroutine()
    {
        for (int i = 0; i < bulletHellShots; i++)
        {
            for (int j = 0; j < BulletHellShooters.Count; j++)
            {
                if (j % 2 == 0)
                    BulletHellShooters[j].Shoot(shootSpeed, respawn.position);
            }

            yield return new WaitForSeconds(bulletHellRate);

            for (int k = 0; k < BulletHellShooters.Count; k++)
            {
                if (k % 2 != 0)
                    BulletHellShooters[k].Shoot(shootSpeed, respawn.position);
            }

            yield return new WaitForSeconds(bulletHellRate);
        }

        _timer.Restart();
    }

    IEnumerator LaserAttackCoroutine()
    {
        moveTowardsPlayer.StartMoving();
        foreach (var laser in laserEnemies)
        {
            laser.ShowBase();
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(5);
        foreach (var laser in laserEnemies)
        {
            laser.TurnOnLaser();
            yield return new WaitForSeconds(0.2f);
        }

        moveTowardsPlayer.StopMoving();
        yield return new WaitForSeconds(4);
        foreach (var laser in laserEnemies)
        {
            laser.TurnOffLaser();
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1);

        foreach (var laser in laserEnemies)
        {
            laser.HideBase();
            yield return new WaitForSeconds(0.2f);
        }

        moveTowardsPlayer.StartMoving();
    }

    #endregion


    public override void Hide()
    {
        // dissolveMaterials.DissolveOut();
        minigameStarted = false;
        conversationShown = false;
        boxCollider.enabled = false;
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

            if (lives < (maxLives / 2))
            {
                //mid conversation stuff
                if (!conversationShown)
                {
                    conversationManager.StartConversation(conversation, conversationFocus);
                    conversationShown = true;
                }

                int shootingMode = Random.Range(0, 9);

                if (shootingMode % 2 == 0)
                    StartCoroutine(LaserAttackCoroutine());
            }

            if (lives <= 0)
            {
                Die();
            }
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
        // dissolveMaterials.DissolveIn();
        conversationShown = false;
        boxCollider.enabled = true;
    }

    private void Die()
    {
        //sonidos
        //efecto de particulas outline.OutlineWidth = 0;

        DisableShooters();
        objetives.RemoveObjetive();
        dissolveMaterials.DissolveOut();
        enemySounds.PlayDieSound();
        dead = true;
        conversationShown = true;
        boxCollider.enabled = false;
        _doorManager.OpenDoor();
    }

    public override bool GetEnemyDead()
    {
        return dead;
    }

    public override void ResetEnemy()
    {
        dissolveMaterials.DissolveIn();
        lives = maxLives;
        minigameStarted = false;
        conversationShown = false;
        StartCoroutine(ResetConversationCoroutine());
    }

    private void DisableShooters()
    {
        StopAllCoroutines();
        foreach (var shooter in spiralShooters)
        {
            shooter.gameObject.SetActive(false);
        }

        foreach (var shooter in ArrowShooters)
        {
            shooter.gameObject.SetActive(false);
        }

        foreach (var shooter in BulletHellShooters)
        {
            shooter.gameObject.SetActive(false);
        }

        foreach (var laser in laserEnemies)
        {
            laser.TurnOffLaser();
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die)
        {
            ResetEnemy();
        }
    }

    IEnumerator ResetConversationCoroutine()
    {
        yield return new WaitForSeconds(6);
        initialDialog.SetActive(true);
    }
}