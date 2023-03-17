using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorCollider : MonoBehaviour
{
    //components
    private LaberinthManager _laberinthManager;

    void Start()
    {
        _laberinthManager = transform.parent.GetComponentInChildren<LaberinthManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            _laberinthManager.CloseDoor();
    }


}