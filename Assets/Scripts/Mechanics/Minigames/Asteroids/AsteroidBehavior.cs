using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(3)]
public class AsteroidBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public AsteroidsManager _asteroidsManager;
    private Vector2 direction;
    private float speed;
    private int bounceCount;
    private int maxBounces;
    private bool active = false;
    public Image _image;
    private Color initColor;
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");


    void Start()
    {
        initColor = _image.color;
        _asteroidsManager = FindObjectOfType<AsteroidsManager>();
        audioSource = GetComponent<AudioSource>();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Movement();
            CheckIfKillPlayer();
        }
    }

    public void resetAsteroid()
    {
        _image.color = Color.clear;
        bounceCount = 0;
        active = false;
    }

    public void Spawn()
    {
        FindSpawnPos();
        direction = -_asteroidsManager.VectorTowardsPlayer(transform.localPosition).normalized;
        _image.color = initColor;
        speed = Random.Range(0.25f, 0.3f);
        maxBounces = Random.Range(3, 8);
        bounceCount = 0;
        active = true;
    }

    public void Movement()
    {
        if (transform.localPosition.x >= _asteroidsManager.GetArenaTransform()[1] / 2 ||
            transform.localPosition.x <= -_asteroidsManager.GetArenaTransform()[1] / 2)
        {
            if (Random.Range(0, 10) < 2)
                AttackPlayer();
            else
                ChangeDirectionX();
            AddBounce();
        }
        else if (transform.localPosition.y >= _asteroidsManager.GetArenaTransform()[0] / 2 ||
                 transform.localPosition.y <= -_asteroidsManager.GetArenaTransform()[0] / 2)
        {
            if (Random.Range(0, 10) < 1)
                AttackPlayer();
            else
                ChangeDirectionY();
            AddBounce();
        }

        SelfRotation();
        transform.localPosition +=
            new Vector3(speed * direction.x * Time.deltaTime, speed * direction.y * Time.deltaTime, 0);
    }

    private void AddBounce()
    {
        bounceCount++;
        audioSource.clip = clips[0];
        audioSource.Play();
        ColorIterpolation();
        if (bounceCount >= maxBounces)
            Die();
    }


    private void ColorIterpolation()
    {
        _image.color = Color.Lerp(_image.color, Color.red, (float)bounceCount / maxBounces);
    }

    private void FindSpawnPos()
    {
        bool valid = false;
        float width = _asteroidsManager.GetArenaTransform()[1];
        float height = _asteroidsManager.GetArenaTransform()[0];
        Vector3 pos = Vector3.zero;
        while (!valid)
        {
            pos = new Vector2(Random.Range(-width / 2, width / 2),
                Random.Range(-height / 2, height / 2));
            if (_asteroidsManager.DistanceToPlayer(pos) > 0.1f)
                valid = true;
        }

        transform.localPosition = pos;
    }

    private void ChangeDirectionX()
    {
        direction = new Vector2(direction.x * -1, direction.x + Random.Range(-0.5f, 0.5f)).normalized;
    }

    private void AttackPlayer()
    {
        direction = _asteroidsManager.VectorTowardsPlayer(transform.localPosition).normalized;
    }

    private void ChangeDirectionY()
    {
        direction = new Vector2(direction.x + Random.Range(-0.5f, 0.5f), direction.y * -1).normalized;
    }

    private void CheckIfKillPlayer()
    {
        if (_asteroidsManager.DistanceToPlayer(transform.localPosition) < _asteroidsManager.killingRange)
        {
            _asteroidsManager.KillPlayer();
        }
    }

    private void SelfRotation()
    {
        if (direction.x > 0)
            transform.Rotate(0, 0, MyUtils.Clamp0360(-speed * 100 * Time.deltaTime));
        else
            transform.Rotate(0, 0, MyUtils.Clamp0360(speed * 100 * Time.deltaTime));
    }

    public void Die()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
        active = false;
        _image.color = Color.clear;
        bounceCount = 0;
        _asteroidsManager.TransferAsteroid(this);
        _asteroidsManager.EndRound();
    }
}