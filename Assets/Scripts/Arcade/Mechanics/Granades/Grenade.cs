using System;
using System.Collections;
using System.Diagnostics;
using Arcade.Mechanics.Granades;
using UnityEngine;

public class Grenade : Subject
{
    // Start is called before the first frame update
    private GrenadeType _grenadeType;

    private PlayerValues _playerValues;
    private Rigidbody _rigidbody;
    private bool isVisible, beingUsed, readyToUse;
    private Transform iddlePos;
    [SerializeField] private float force, throwForce;
    private Renderer _renderer;
    private static readonly int Alpha = Shader.PropertyToID("_alpha");
    [SerializeField] private float throwDrag = 1, recoverDrag = 10, explotionRadius = 5;

    private SphereCollider _collider;

    //materials 
    [SerializeField] private ParticleSystem smokeParticles, normalParticles, FreezeParticles;

    //outline


    //cooldown
    private Stopwatch _timer;
    [SerializeField] private float explosionCooldown = 5;
    private float _targetAlpha, _tA;
    private bool _updateAlpha, _lockPos;
    private CameraChanger cameraChanger;


    //ready sound
    private AudioSource explosionSound;
    private ParticleSystem particles;

    private bool playedEffects;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");

    private void Awake()
    {
        _timer = new Stopwatch();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        cameraChanger = FindObjectOfType<CameraChanger>();
        explosionSound = GetComponent<AudioSource>();
    }


    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        AddObserver(_playerValues.GetComponent<PlayerSounds>());
        AddObserver(FindObjectOfType<RumbleObserver>());
        Physics.IgnoreCollision(_playerValues.GetComponent<BoxCollider>(), GetComponent<BoxCollider>());
        _renderer.material.SetFloat(Alpha, 0);
    }

    private void Update()
    {
        if (_updateAlpha)
            SmoothAlpha();

        Explode();
    }

    private void FixedUpdate()
    {
        if (!beingUsed)
            HoldObject();
    }

    public void InitGrenade(Transform newIdlePos, GrenadeType type)
    {
        _grenadeType = type;
        UpdateColor();
        UpdateParticles();
        iddlePos = newIdlePos;
        beingUsed = false;
        playedEffects = false;
        SetVisible(true);
        _rigidbody.drag = recoverDrag;
        transform.position = newIdlePos.position;
        _timer.Stop();
        _timer.Reset();
    }

    public void ThrowGrenade()
    {
        if (!beingUsed)
        {
            NotifyObservers(PlayerActions.ThrowDistraction);
            beingUsed = true;
            _rigidbody.drag = throwDrag;
            _rigidbody.AddForce(cameraChanger.GetActiveCam().transform.forward * throwForce, ForceMode.Impulse);
            _timer.Start();
        }
    }

    public void SetAlpha(float targetAlpha)
    {
        _targetAlpha = targetAlpha;
        _tA = 0;
        _updateAlpha = true;
    }

    private void SmoothAlpha()
    {
        _renderer.material.SetFloat(Alpha, Mathf.Lerp(_renderer.material.GetFloat(Alpha), _targetAlpha, _tA));
        _tA += 0.5f * Time.deltaTime;
        if (_tA > 1.0f)
        {
            _tA = 1.0f;
            _updateAlpha = false;
        }
    }

    public void SetVisible(bool val)
    {
        isVisible = val;

        if (val)
        {
            SetAlpha(0.75f);
        }
        else
        {
            SetAlpha(0);
        }
    }

    public bool GetIsVisible()
    {
        return isVisible;
    }

    void HoldObject()
    {
        if (!_lockPos)
        {
            if (Vector3.Distance(transform.position, iddlePos.position) > 0.01f)
            {
                Vector3 moveDir = (iddlePos.position - transform.position);
                _rigidbody.AddForce(moveDir * force, ForceMode.Acceleration);
            }
            else
                _lockPos = true;
        }
        else
        {
            transform.position = iddlePos.position;
            _rigidbody.angularVelocity = new Vector3(1, 1, 1);
        }
    }

    public bool GetIsReadyToUse()
    {
        return readyToUse;
    }

    public void SetIsReadyToUse(bool val)
    {
        readyToUse = val;
    }

    private void Explode()
    {
        if (!playedEffects && _timer.Elapsed.TotalSeconds > explosionCooldown)
        {
            playedEffects = true;
            _timer.Stop();
            explosionSound.Play();
            particles.transform.up = Vector3.up;
            particles.Play();
            SetVisible(false);
            StartCoroutine(ReadyGrenadeCoroutine());
            //do the damage
            GetEnemiesExplotion();
            NotifyObservers(PlayerActions.GrenadeExplode);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explotionRadius);
    }

    private void GetEnemiesExplotion()
    {
        RaycastHit[] hits =
            Physics.SphereCastAll(transform.position, explotionRadius, Vector3.up, explotionRadius);

        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                switch (_grenadeType)
                {
                    case GrenadeType.NormalGrenade:
                        enemy.GrenadeDamage();
                        break;
                    case GrenadeType.FreezeGrenade:
                        enemy.GrenadeFreezeDamage();
                        break;
                    case GrenadeType.SmokeGrenade:
                        enemy.GrenadeSmoke(transform.position, 4.5f);
                        break;
                    case GrenadeType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public bool BeingUsed()
    {
        return beingUsed;
    }

    private void UpdateColor()
    {
        Color col = _grenadeType switch
        {
            GrenadeType.NormalGrenade => new Color(0, 0.9803922f, 0.6595296f),
            GrenadeType.FreezeGrenade => new Color(0.3259999f, 0.9514491f, 1f),
            GrenadeType.SmokeGrenade => new Color(0.844f, 0.844f, 0.844f)
        };
        _renderer.material.SetColor(BackgroundColor, col);
    }

    private void UpdateParticles()
    {
        ParticleSystem particle = _grenadeType switch
        {
            GrenadeType.NormalGrenade => normalParticles,
            GrenadeType.FreezeGrenade => FreezeParticles,
            GrenadeType.SmokeGrenade => smokeParticles
        };
        particles = particle;
    }

    IEnumerator ReadyGrenadeCoroutine()
    {
        if (_grenadeType is GrenadeType.NormalGrenade or GrenadeType.FreezeGrenade)
            yield return new WaitForSeconds(1);
        else if (_grenadeType is GrenadeType.SmokeGrenade)
            yield return new WaitForSeconds(5);
        SetIsReadyToUse(true);
    }
}