using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    [SerializeField] private Target target;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
            target.Shot();
    }
}