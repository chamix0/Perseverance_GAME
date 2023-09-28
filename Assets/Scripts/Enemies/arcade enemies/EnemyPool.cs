using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private Queue<Enemy> usedEnemies;

    private Transform container;

    [SerializeField] private GameObject enemyTemplate;


    private void Awake()
    {
        usedEnemies = new Queue<Enemy>();
    }


    public Enemy GetEnemy()
    {
        Enemy enemy = FindEnemy();
        if (enemy != null)
            return enemy;
        GameObject newEnemy = Instantiate(enemyTemplate, transform);
        newEnemy.transform.position = new Vector3(1000, 1000, 0);
        Enemy enemyComp = newEnemy.GetComponent<Enemy>();
        InsertNewEnemy(enemyComp);
        return enemyComp;
    }

    private Enemy FindEnemy()
    {
        int enemyCount = usedEnemies.Count;
        for (int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = usedEnemies.Dequeue();
            usedEnemies.Enqueue(enemy);
            if (enemy.GetEnemyDead())
            {
                enemy.Hide();
                return enemy;
            }
        }

        return null;
    }

    private void InsertNewEnemy(Enemy newEnemy) =>
        usedEnemies.Enqueue(newEnemy);
}