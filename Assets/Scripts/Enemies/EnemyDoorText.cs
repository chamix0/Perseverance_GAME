using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDoorText : MonoBehaviour
{
    [SerializeField] private List<EnemyShooterZone> shooterZones;
    [SerializeField] private DoorManager doorManager;
    private string enemiesLeftCad;
    [SerializeField] private TMP_Text screenText;
    private bool _minigameFinished;

    // Start is called before the first frame update
    void Start()
    {
        enemiesLeftCad = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (!_minigameFinished && !enemiesLeftCad.Equals(GetMissingTargetsCad()))
        {
            enemiesLeftCad = GetMissingTargetsCad();
            screenText.text = enemiesLeftCad;
            if (GetMissingEnemies() <= 0)
            {
                _minigameFinished = true;
                doorManager.OpenDoor();
            }
        }
    }

    int GetMissingEnemies()
    {
        int count = 0;
        int enemyCount = 0;
        foreach (var shooterZone in shooterZones)
        {
            foreach (var enemy in shooterZone.GetEnemies())
            {
                if (enemy.GetEnemyDead())
                    count++;
                enemyCount++;
            }
        }

        return enemyCount - count;
    }


    string GetMissingTargetsCad()
    {
        return "Enemies Left : " + GetMissingEnemies();
    }
    
}