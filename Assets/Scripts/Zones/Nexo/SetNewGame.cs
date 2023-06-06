using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNewGame : MonoBehaviour
{
    private PlayerValues playerValues;
    [SerializeField] private GameObject trigger;
    [SerializeField] private ScoreBoard scoreBoard;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            trigger.SetActive(true);
            playerValues.gameData.SetNewGame();
            playerValues.gameData.enableLevel(0);
            scoreBoard.UpdateText();
            playerValues.SaveGame();
        }
    }
}