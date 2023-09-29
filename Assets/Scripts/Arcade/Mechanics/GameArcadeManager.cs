using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Player.Observer_pattern;
using UnityEditor.SceneManagement;
using UnityEngine;
using UTILS;

[DefaultExecutionOrder(6)]
public class GameArcadeManager : MonoBehaviour,IObserver
{
    private PlayerValues _playerValues;
    private EnemyManager _enemyManager;
    private ArcadePlayerData _playerData;
    private GuiManager _guiManager;

    [SerializeField] private MyStopWatch _betweenRoundsTimer;
    [SerializeField] private float timeBetweenRounds = 30f;

    //shops
    private List<BulletShopBase> _bulletShopBases;
    private List<UpgradeBase> _upgradeBases;
    private UpgradeManager _upgradeManager;
    private BulletShopManager _bulletShopManager;

    //enemy parameters
    [SerializeField] private int minNumEnemies = 3, maxNumEnemies = 6, numEnemyCap = 50;
    [SerializeField] private int minLives = 6, maxLives = 10;
    [SerializeField] float minSpeed = 1, maxSpeed = 2, speedCap = 4;
    [SerializeField] private int minDamage = 1, maxDamage = 1, _damageCap = 2;
    [SerializeField] private float minSpawnTime = 7, maxSpawnTime = 10, _spawnTimeCap = 1;

    //variables
    private bool displayRace, roundStarted;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _enemyManager = FindObjectOfType<EnemyManager>();
        _guiManager = FindObjectOfType<GuiManager>();
        _bulletShopBases = new List<BulletShopBase>(FindObjectsOfType<BulletShopBase>());
        _upgradeBases = new List<UpgradeBase>(FindObjectsOfType<UpgradeBase>());
        _bulletShopManager = FindObjectOfType<BulletShopManager>();
        _upgradeManager = FindObjectOfType<UpgradeManager>();
        StartInBetweenRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayRace)
            _guiManager.SetRaceTime(GetRemainingTime(), 60.7f);
        if (!roundStarted && _betweenRoundsTimer.GetElapsedSeconds() > timeBetweenRounds)
            StartRound();
    }

    #region rounds

    public void StartInBetweenRound()
    {
        roundStarted = false;
        EnableShops();
        //timer
        _betweenRoundsTimer.Restart();
        displayRace = true;
    }

    public void StartRound()
    {
        roundStarted = true;
        //show the round counter

        //hide timer
        displayRace = false;
        _guiManager.DisableRace();
        _betweenRoundsTimer.Stop();
        _betweenRoundsTimer.ResetStopwatch();
        //dissable shops
        ExitFromShops();
        DisableShops();
        //spawn enemies
        int round = _playerData.GetRound();
        if (round % 5 == 0 && round > 0)
            IncreaseDifficulty(round);
        _enemyManager.StartRound(minNumEnemies, maxNumEnemies, minLives,
            maxLives, minSpeed, maxSpeed, minDamage,
            maxDamage, minSpawnTime, maxSpawnTime);
    }

    public void EndRound()
    {
        _playerData.IncreaseRound();
        StartInBetweenRound();
    }

    #endregion

    private void ExitFromShops()
    {
        if (_upgradeManager.isIn)
            _upgradeManager.EndShop();
        if (_bulletShopManager.isIn)
            _bulletShopManager.EndShop();
    }

    private void DisableShops()
    {
        foreach (var bulletShop in _bulletShopBases)
            bulletShop.DeactivateBase();
        foreach (var upgradeBase in _upgradeBases)
            upgradeBase.DeactivateBase();
    }

    private void EnableShops()
    {
        foreach (var bulletShop in _bulletShopBases)
            bulletShop.ActivateBase();
        foreach (var upgradeBase in _upgradeBases)
            upgradeBase.ActivateBase();
    }

    private void IncreaseDifficulty(int numRound)
    {
        //num enemies
        minNumEnemies = Mathf.Min(numEnemyCap / 2, minNumEnemies + 1);
        maxNumEnemies = Mathf.Min(numEnemyCap, minNumEnemies + 2);
        //lives
        minLives += 2;
        maxLives += 2;
        //damage
        if (numRound >= 15)
            maxDamage = 2;
        //speed
        if (numRound >= 10)
            maxSpeed = Mathf.Min(speedCap, maxSpeed + 0.5f);
        //spawn time

        minSpawnTime = Mathf.Max(_spawnTimeCap, minSpawnTime - 1);
        maxSpawnTime = Mathf.Max(_spawnTimeCap * 2, maxSpawnTime - 1);
    }

    private string GetRemainingTime()
    {
        var aux = timeBetweenRounds * 1000 - _betweenRoundsTimer.GetElapsedMiliseconds();
        return MyUtils.GetCountdownTimeString(aux);
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die)
        {
            
        }
    }
}