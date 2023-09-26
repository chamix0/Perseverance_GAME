using Arcade.Mechanics.Bullets;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootBullet : MonoBehaviour
{
    [SerializeField] private BulletPool _bulletPool;
    public bool isPlayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem flash;

    public void Shoot(float speed)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, transform.forward, speed, Vector3.zero);
    }

    public void Shoot(BulletType bulletType, float speed)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet(bulletType);
        aux.Shoot(isPlayer, transform.position, transform.forward, speed, Vector3.zero);
    }

    public void ShootRandom(BulletType bulletType, float speed)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet(bulletType);
        aux.Shoot(isPlayer, transform.position, transform.forward +
                                                new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0),
            speed, Vector3.zero);
    }

    public void Shoot(BulletType bulletType, float speed, Vector3 direction)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet(bulletType);
        aux.Shoot(isPlayer, transform.position, direction, speed, Vector3.zero);
    }

    public void Shoot(Vector3 direction, float speed)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, direction, speed, Vector3.zero);
    }

    //enemy shoooting
    public void Shoot(float speed, Vector3 respawn)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, transform.forward, speed, respawn);
    }

    public void Shoot(Vector3 direction, float speed, Vector3 respawn)
    {
        PlayParticlesAndSound();
        Bullet aux = _bulletPool.GetBullet();
        aux.Shoot(isPlayer, transform.position, direction, speed, respawn);
    }

    private void PlayParticlesAndSound()
    {
        if (flash)
        {
            flash.Play();
        }

        if (audioSource)
        {
            audioSource.Play();
        }
    }
}