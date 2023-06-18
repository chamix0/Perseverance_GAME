using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(12)]
public class EnemyShooterZone : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Enemy> enemies;
    private EnemyPath enemyPath;

    // [SerializeField] private DoorManager doorManager;
    private bool _minigameFinished = false;
    [SerializeField] private bool startAutomatically=true;

    private void Awake()
    {
        enemies = new List<Enemy>();
    }

    void Start()
    {
        enemies.AddRange(GetComponentsInChildren<Enemy>());
        enemyPath = GetComponent<EnemyPath>();
        
        HideAll();
        if (startAutomatically)
            AssignInitialPositions();
    }


    public int GetNewTarget(int currentNode)
    {
        int target = Random.Range(0, enemyPath.GetNumNodes());
        while (target == currentNode)
        {
            target = Random.Range(0, enemyPath.GetNumNodes());
        }

        return target;
    }

    public void AssignInitialPositions()
    {
        int numNodes = enemyPath.GetNumNodes();
        List<int> unusedNodes = new List<int>();
        for (int i = 0; i < numNodes; i++)
            unusedNodes.Add(i);


        foreach (var enemy in enemies)
        {
            int index = Random.Range(0, unusedNodes.Count);
            enemy.Spawn(unusedNodes[index]);

            unusedNodes.Remove(index);
        }
    }

    public void HideAll()
    {
        foreach (var enemy in enemies)
        {
            enemy.Hide();
        }
    }

    bool AllEnemiesDead()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.GetEnemyDead())
                return false;
        }

        return true;
    }

    private void Update()
    {
        if (!_minigameFinished)
        {
            if (GetMissingEnemies() <= 0)
                _minigameFinished = true;
        }
    }

    int GetMissingEnemies()
    {
        int count = 0;
        foreach (var enemy in enemies)
        {
            if (enemy.GetEnemyDead())
                count++;
        }

        return enemies.Count - count;
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}