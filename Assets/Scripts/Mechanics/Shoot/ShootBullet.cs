using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShootBullet : MonoBehaviour
{
    private BulletPool _bulletPool;
    public bool isPlayer;

    // Start is called before the first frame update


    void Start()
    {
        _bulletPool = GetComponentInChildren<BulletPool>();
    }


    public void Shoot(float speed)
    {
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.forward, speed, Vector3.zero);
    }

    public void Shoot(Vector3 direction, float speed)
    {
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, direction, speed, Vector3.zero);
    }

    public void Shoot(float speed, Vector3 respawn)
    {
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.forward, speed, respawn);
    }

    public void Shoot(Vector3 direction, float speed, Vector3 respawn)
    {
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, direction, speed, respawn);
    }
}