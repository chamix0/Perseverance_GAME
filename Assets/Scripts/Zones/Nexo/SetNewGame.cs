using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is used to tell the game data file that the game has been started meaning that the nexus dialog has been readed
/// so it enables the first zone trigger to open the door and to write on the level screen "access granted"
/// </summary>
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