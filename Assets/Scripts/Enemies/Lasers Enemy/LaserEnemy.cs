using System.Diagnostics;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    private PlayerValues playerValues;
    [SerializeField] private GameObject laser;
    [SerializeField] private ParticleSystem baseLaser;
    private bool laserShooting;
    [SerializeField] private LayerMask playerLayer;
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
        return Physics.Raycast(transform.position, baseLaser.transform.position - laser.transform.position,
            Mathf.Infinity,
            playerLayer);
    }

    public void ShowBase()
    {
        baseLaser.Play();
    }

    public void HideBase()
    {
        baseLaser.Stop();
    }
}