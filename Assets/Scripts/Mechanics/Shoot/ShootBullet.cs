using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShootBullet : MonoBehaviour
{
    [SerializeField] private BulletPool _bulletPool;
    public bool isPlayer;
    [SerializeField] private ParticleSystem flash;

    // Start is called before the first frame update


    void Start()
    {
    }


    public void Shoot(float speed)
    {
        PlayParticles();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, transform.forward, speed, Vector3.zero);
    }

    public void Shoot(Vector3 direction, float speed)
    {
        PlayParticles();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, direction, speed, Vector3.zero);
    }

    public void Shoot(float speed, Vector3 respawn)
    {
        PlayParticles();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, transform.forward, speed, respawn);
    }

    public void Shoot(Vector3 direction, float speed, Vector3 respawn)
    {
        PlayParticles();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, direction, speed, respawn);
    }

    private void PlayParticles()
    {
        if (flash)
        {
                    flash.Play();

        }
    }
}