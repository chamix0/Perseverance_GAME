using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsFall : MonoBehaviour
{
    private PlayerValues _playerValues;
    public float fact;
    public Vector3 forceDirection;

    private void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerValues._rigidbody.AddForce(forceDirection * fact,ForceMode.Impulse);
        }
    }
}