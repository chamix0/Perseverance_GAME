using System;
using System.Collections.Generic;
using UnityEngine;

public class FallTriggerLaberinth : MonoBehaviour
{
    private PlayerValues playerValues;
    public List<GameObject> spawnPoints;
 [NonSerialized]   public bool secondFase = false;

    private void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!secondFase)
                playerValues.Die(spawnPoints[0].transform.position);
            else
                playerValues.Die(spawnPoints[1].transform.position);
        }
    }
}