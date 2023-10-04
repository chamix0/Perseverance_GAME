using System;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(12)]
public class EnemyShooterZone : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    private List<Enemy> enemies;
    private EnemyPath enemyPath;
    private PlayerValues _playerValues;

    // [SerializeField] private DoorManager doorManager;
    private bool _minigameFinished = false;
    [SerializeField] private bool startAutomatically = true;

    private void Awake()
    {
        enemies = new List<Enemy>();
    }

    void Start()
    {
        enemies.AddRange(GetComponentsInChildren<Enemy>());
        _playerValues = FindObjectOfType<PlayerValues>();
        _playerValues.AddObserver(this);
        try
        {
            enemyPath = GetComponent<EnemyPath>();
        }
        catch (Exception e)
        {
            enemyPath = null;
            Console.WriteLine(e);
            throw;
        }

        HideAll();
        if (startAutomatically)
            AssignInitialPositions();
    }

    #region Routs

    public int GetNewTarget(int currentNode)
    {
        int target = Random.Range(0, enemyPath.GetNumNodes());
        while (target == currentNode)
        {
            target = Random.Range(0, enemyPath.GetNumNodes());
        }

        return target;
    }

    public EnemyPath GetEnemyPath()
    {
        return enemyPath;
    }

    public void AssignInitialPositions()
    {
        if (enemyPath)
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
        else
        {
            foreach (var enemy in enemies)
            {
                enemy.Spawn(0);
            }
        }
    }

    #endregion


    public void HideAll()
    {
        foreach (var enemy in enemies)
        {
            enemy.Hide();
        }
    }

    public bool AllEnemiesDead()
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
        if (!_minigameFinished && AllEnemiesDead())
            _minigameFinished = true;
    }

    public int GetTotalEnemies()
    {
        return enemies.Count;
    }

    public float GetLiveValue()
    {
        int maxLives = enemies[0].totalLives;
        int count = 0;
        foreach (var enemy in enemies)
        {
            count += enemy.lives;
        }

        return (float)count / (maxLives * enemies.Count);
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    private void ResetEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die && !_minigameFinished)
        {
            ResetEnemies();
        }
    }
}