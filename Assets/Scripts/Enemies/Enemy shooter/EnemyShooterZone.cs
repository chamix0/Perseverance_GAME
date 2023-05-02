using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(6)]
public class EnemyShooterZone : MonoBehaviour
{
    // Start is called before the first frame update
    private List<EnemyShooter> enemies;
    private EnemyPath enemyPath;

    private void Awake()
    {
        enemies = new List<EnemyShooter>();
    }

    void Start()
    {
        enemies.AddRange(GetComponentsInChildren<EnemyShooter>());
        enemyPath = GetComponent<EnemyPath>();
        AssignInitialPositions();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void AssignInitialPositions()
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
}