using System;
using System.Collections;
using Mechanics.Shoot.Bullets;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour
{
    private PlayerValues _playerValues;
    [SerializeField] private BulletType _bulletType = BulletType.NormalBullet;
    public bool isPlayer;
    private Vector3 _direction;
    private bool shot;
    private Vector3 respawn;
    [SerializeField] private ParticleSystem hit;
    private float speed = 1;
    [SerializeField] private Rigidbody _rigidbody;
    private bool ready;
    public LayerMask collisionLayers;
    private DecallPool _decallPool;
    private Transform target;
    private bool enemyHit;


    //audio
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _decallPool = FindObjectOfType<DecallPool>();
        _rigidbody.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (_bulletType is BulletType.GuidedBullet && !ready)
        {
            if (target == null)
                target = FindTarget();
            if (target != null)
            {
                _rigidbody.AddForce((target.position - transform.position).normalized * (speed * 8),
                    ForceMode.Acceleration);
                _rigidbody.velocity = _rigidbody.velocity.normalized * (speed * 4);
            }
        }
    }

    public void SetIsReadyToUse(bool val)
    {
        ready = val;
    }

    public bool IsReadyToUse()
    {
        return ready;
    }

    public void SetEnemyHit(bool val)
    {
        enemyHit = val;
    }

    public bool GetEnemyHit()
    {
        return enemyHit;
    }

    public void Shoot(bool player, Vector3 origin, Vector3 dir, float s, Vector3 checkpoint)
    {
        ready = false;
        enemyHit = false;
        isPlayer = player;
        respawn = checkpoint;
        speed = s;
        transform.position = origin;
        _direction = dir.normalized;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = false;
        _rigidbody.drag = 0;
        hit.Stop();
        hit.Clear();
        if (_bulletType is BulletType.GuidedBullet)
            _rigidbody.AddForce(_direction * speed / 2, ForceMode.Impulse);

        else _rigidbody.AddForce(_direction * speed, ForceMode.Impulse);
    }

    public BulletType GetBulletType()
    {
        return _bulletType;
    }

    private Transform FindTarget()
    {
        RaycastHit[] hits =
            Physics.SphereCastAll(transform.position, 12, _direction, 20);

        Transform minDistTransform = null;
        float minDistance = Mathf.Infinity;
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                float newDist = Vector3.Distance(transform.position, hit.transform.position);
                if (newDist < minDistance)
                {
                    minDistTransform = hit.transform;
                    minDistance = newDist;
                }
            }
        }

        return minDistTransform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (MyUtils.IsInLayerMask(collision.gameObject, collisionLayers))
        {
            if (!ready)
            {
                if (isPlayer)
                {
                }
                else
                {
                    if (!_playerValues.dead && collision.transform.CompareTag("Player"))
                    {
                        _playerValues.RecieveDamage(respawn);
                    }
                }

                audioSource.Play();
                hit.Play();
                _decallPool.GetDecal().UseDecal(transform.position, -collision.contacts[0].normal, _bulletType);
            }
            ready = true;
            _rigidbody.useGravity = true;
            _rigidbody.drag = 1;
            target = null;
            if (!collision.transform.CompareTag("Enemy"))
                enemyHit = true;
        }
    }
}