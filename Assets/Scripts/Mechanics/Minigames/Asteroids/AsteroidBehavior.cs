using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
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
    public Shader shader;
    private Material _material;

    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");


    void Start()
    {
        _material = new Material(shader);
        _image.material = _material;
        _material.SetColor(BackgroundColor, Color.red);
        _asteroidsManager = FindObjectOfType<AsteroidsManager>();
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
        _material.SetFloat(MyAlpha, 0);
        bounceCount = 0;
        active = false;
    }

    public void Spawn()
    {
        FindSpawnPos();
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        _image.material.SetFloat(MyAlpha, 1);
        speed = Random.Range(0.1f, 0.3f);
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

        transform.localPosition +=
            new Vector3(speed * direction.x * Time.deltaTime, speed * direction.y * Time.deltaTime, 0);
    }

    private void AddBounce()
    {
        bounceCount++;
        {
            if (bounceCount >= maxBounces)
                Die();
        }
    }

    private void FindSpawnPos()
    {
        bool valid = false;
        float width = _asteroidsManager.GetArenaTransform()[1];
        float height = _asteroidsManager.GetArenaTransform()[0];
        Vector3 pos = Vector3.zero;
        while (!valid)
        {
            pos = new Vector2(Random.Range(-width  / 2, width  / 2),
                Random.Range(-height  / 2, height  / 2));
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

    public void Die()
    {
        active = false;
        _material.SetFloat(MyAlpha, 0);
        _asteroidsManager.TransferAsteroid(this);
        _asteroidsManager.EndRound();
    }
}