using System.Diagnostics;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class LaserEnemy : MonoBehaviour
{
    private PlayerValues playerValues;
    [SerializeField] private GameObject laser;
    [SerializeField] private ParticleSystem baseLaser;
    [SerializeField] private Transform start, end;
    private bool laserShooting;
    [SerializeField] private LayerMask hitLayer;
    private Stopwatch timer;
    [SerializeField] private float damageCooldown = 3;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        timer = new Stopwatch();
        timer.Start();
    }

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        TurnOffLaser();
        if (baseLaser)
            baseLaser.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (laserShooting && CheckColisionWithPlayer() && timer.Elapsed.TotalSeconds > damageCooldown)
        {
            timer.Restart();
            playerValues.RecieveDamage(spawnPoint.position);
        }

        if (laserShooting)
            CheckColisionWithEnemy();
        
    }

    public void TurnOnLaser()
    {
        laserShooting = true;
        laser.SetActive(true);
    }

    public void TurnOffLaser()
    {
        laserShooting = false;
        laser.SetActive(false);
    }

    public bool CheckColisionWithPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(start.position, end.position - start.position, out hit,
                Mathf.Infinity))
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }

        return false;
    }

    public bool CheckColisionWithEnemy()
    {
        RaycastHit hit;
        if (Physics.Raycast(start.position, end.position - start.position, out hit,
                Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                enemy.RecieveLaserDamage();
                //do stuff
                return true;
            }
        }

        return false;
    }

    public void ShowBase()
    {
        if (baseLaser)
            baseLaser.Play();
    }

    public void HideBase()
    {
        if (baseLaser)
            baseLaser.Stop();
    }
}