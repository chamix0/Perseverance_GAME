using System;
using System.Collections;
using System.Collections.Generic;
using Arcade.Mechanics.Doors;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    private ArcadePlayerData _playerData;
    private GameArcadeManager _gameArcadeManager;
    private Dictionary<ZonesArcade, List<Transform>> spawnPoints;
    [SerializeField] private EnemyPool _enemyPool;
    private List<Enemy> currentEnemies;

    //separatedZones lists
    [SerializeField] private List<Transform> lobbySpawns, tubesSpawns, freezerSpawns, salonSpawns, librarySpawns;

    private void Awake()
    {
        currentEnemies = new List<Enemy>();
        spawnPoints = new Dictionary<ZonesArcade, List<Transform>>();
        spawnPoints.Add(ZonesArcade.Lobby, lobbySpawns);
        spawnPoints.Add(ZonesArcade.Tubes, tubesSpawns);
        spawnPoints.Add(ZonesArcade.Freezer, freezerSpawns);
        spawnPoints.Add(ZonesArcade.Salon, salonSpawns);
        spawnPoints.Add(ZonesArcade.Library, librarySpawns);
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _gameArcadeManager = FindObjectOfType<GameArcadeManager>();
    }

    public void StartRound(int minNumEnemies, int maxNumEnemies, int minLives, int maxLives, float minSpeed,
        float maxSpeed,
        int minDamage, int maxDamage, float minSpawnTime, float maxSpawnTime)
    {
        currentEnemies.Clear();
        int numEnemies = Random.Range(minNumEnemies, maxNumEnemies + 1);
        StartCoroutine(StartRoundCoroutine(numEnemies, minLives, maxLives, minSpeed, maxSpeed, minDamage, maxDamage,
            minSpawnTime, maxSpawnTime));
    }


    // Update is called once per frame
    void Update()
    {
    }

    private Vector3 GetSpawnPoint()
    {
        List<Transform> possibleSpawns = new List<Transform>();
        ZonesArcade[] unlockedZones = _playerData.GetUnlockedZonesArray();
        foreach (var zone in unlockedZones)
        {
            if (spawnPoints.ContainsKey(zone))
                possibleSpawns.AddRange(spawnPoints[zone].ToArray());
        }

        int ranIndex = Random.Range(0, possibleSpawns.Count);
        return possibleSpawns[ranIndex].position;
    }

    private bool AllCurrentEnemiesDead()
    {
        foreach (var enemy in currentEnemies)
        {
            if (!enemy.GetEnemyDead())
                return false;
        }

        return true;
    }

    IEnumerator StartRoundCoroutine(int numEnem, int minLives, int maxLives,
        float minSpeed,
        float maxSpeed,
        int minDamage, int maxDamage, float minSpawnTime, float maxSpawnTime)
    {
        for (int i = 0; i < numEnem; i++)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            int lives = Random.Range(minLives, maxLives + 1);
            float speed = Random.Range(minSpeed, maxSpeed);
            int damage = Random.Range(minDamage, maxDamage + 1);
            Vector3 spawnPoint = GetSpawnPoint();
            Enemy enemy = _enemyPool.GetEnemy();
            currentEnemies.Add(enemy);
            enemy.ResetEnemy(lives, speed, damage, spawnPoint);
        }

        yield return new WaitUntil(AllCurrentEnemiesDead);
        _gameArcadeManager.EndRound();
    }
}