using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Arcade.Mechanics.Bullets;
using Mechanics.General_Inputs.Machine_gun_mode;
using UnityEngine;

public class PlayerMechanicsArcadeManager : MonoBehaviour
{
    // Start is called before the first frame update

    //timers
    private Stopwatch automaticShootTimer;
    //cooldowns

    //variables
    private bool isAutomaticShooting;
    [SerializeField] private Transform grenadeHolder;
    private GrenadePool _grenadePool;
    private Grenade currentGrenade;

    // public BulletType selectedBullet = BulletType.NormalBullet;
    [SerializeField] private List<MachineGun> _machineGuns;
    private ArcadePlayerData _playerData;

    private void Awake()
    {
        automaticShootTimer = new Stopwatch();
    }

    void Start()
    {
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _grenadePool = FindObjectOfType<GrenadePool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAutomaticShooting)
            automaticShooting();
    }

    public void Shoot()
    {
        switch (_playerData.GetShootingMode())
        {
            case ShootingMode.Manual:
                if (_playerData.GetCurrentBulletType() is BulletType.NormalBullet)
                    ActualShot(_playerData.GetCurrentBulletType());
                else if (_playerData.ShootBullet(_playerData.GetCurrentBulletType()))
                    ActualShot(_playerData.GetCurrentBulletType());
                break;
            case ShootingMode.Automatic:
                isAutomaticShooting = true;
                break;
            case ShootingMode.Burst:
                StartCoroutine(BurstCoroutine());
                break;
        }
    }

    public void ThrowGrenade()
    {
        if (currentGrenade == null)
            PrepareGrenade();
        else
        {
            if (_playerData.throwGrenade(_playerData.GetCurrentGrenadeType()))
            {
                currentGrenade.ThrowGrenade();
                PrepareGrenade();
            }
        }
    }

    public void PrepareGrenade()
    {
        if (_playerData.GetNumGrenades(_playerData.GetCurrentGrenadeType()) > 0 &&
            (currentGrenade == null || currentGrenade.BeingUsed()))
        {
            currentGrenade = _grenadePool.GetGrenade();
            UpdateGrenade();
        }
        else if (currentGrenade != null && !currentGrenade.BeingUsed())
        {
            UpdateGrenade();
        }
        else
        {
            currentGrenade = null;
        }
    }

    private void UpdateGrenade()
    {
        currentGrenade.InitGrenade(grenadeHolder, _playerData.GetCurrentGrenadeType());
    }

    public bool IsThereGrenade()
    {
        return currentGrenade != null;
    }

    public void Aim()
    {
        foreach (var machineGun in _machineGuns)
            machineGun.Aim();
    }

    public void StopAim()
    {
        foreach (var machineGun in _machineGuns)
            machineGun.StopAim();
    }

    public void StopAutomaticShooting()
    {
        isAutomaticShooting = false;
        automaticShootTimer.Stop();
        automaticShootTimer.Reset();
    }

    public void ShowMachineGuns()
    {
        foreach (var machine in _machineGuns)
            machine.ShowMachineGun();
    }

    public void HideMachineGuns()
    {
        foreach (var machine in _machineGuns)
            machine.HideMachinegun();
    }

    #region private

    private void automaticShooting()
    {
        if (!automaticShootTimer.IsRunning)
        {
            automaticShootTimer.Start();
            tryShoot();
        }

        if (automaticShootTimer.Elapsed.TotalMilliseconds > _playerData.GetShootingCooldown())
        {
            automaticShootTimer.Restart();
            tryShoot();
        }
    }


    private void ActualShot(BulletType bulletType)
    {
        foreach (var machine in _machineGuns)
        {
            if (bulletType is BulletType.ShotgunBullet)
            {
                for (int i = 0; i < 4; i++)
                {
                    machine.ActualShotArcadeRandom(BulletType.NormalBullet);
                }
            }
            else machine.ActualShotArcade(bulletType);
        }
    }

    private bool tryShoot()
    {
        if (_playerData.ShootBullet(_playerData.GetCurrentBulletType()))
        {
            ActualShot(_playerData.GetCurrentBulletType());
            return true;
        }

        StopAutomaticShooting();
        return false;
    }

    #endregion

    #region coroutines

    IEnumerator BurstCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            if (tryShoot())
                yield return new WaitForSeconds(0.25f);
            else
                break;
        }
    }

    #endregion
}