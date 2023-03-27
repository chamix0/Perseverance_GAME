using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PlayerValues _playerValues;
    public bool isPlayer;
    private Vector3 _direction;
    private bool shot;
    private Vector3 respawn;
    
    [SerializeField] private float speed = 1;
    [SerializeField] private Rigidbody _rigidbody;
    public bool ready;

    private void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        ready = true;
    }


    public bool IsReadyToUse()
    {
        return ready;
    }

    public void Shoot(bool player,Vector3 dir, float s,Vector3 checkpoint)
    {
        _rigidbody.useGravity = false;
        isPlayer = player;
        respawn = checkpoint;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        speed = s;
        _direction = dir.normalized;
        var transform1 = transform;
        transform1.position = transform1.parent.position;
        _rigidbody.AddForce(_direction * speed, ForceMode.Impulse);
        ready = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
      
        if (!ready)
        {
            if (isPlayer)
            {
                
            }
            else
            {
                if (collision.transform.CompareTag("Player"))
                {
                    _playerValues.Die(respawn);
                }
            }  
        }

        ready = true;
        _rigidbody.useGravity = true;
      
    }
}