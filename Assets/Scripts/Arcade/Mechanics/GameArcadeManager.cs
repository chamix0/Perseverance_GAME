using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;
using UTILS;

[DefaultExecutionOrder(6)]
public class GameArcadeManager : MonoBehaviour
{
    private PlayerValues _playerValues;
    private EnemyManager _enemyManager;
    private ArcadePlayerData _playerData;
    private GuiManager _guiManager;

    [SerializeField] private MyStopWatch _betweenRoundsTimer;
    [SerializeField] private float timeBetweenRounds = 20f;

    //shops
    private List<BulletShopBase> _bulletShopBases;
    private List<UpgradeBase> _upgradeBases;
    private UpgradeManager _upgradeManager;
    private BulletShopManager _bulletShopManager;

    //enemy parameters
    [SerializeField] private int minNumEnemies, maxNumEnemies = 1;
    [SerializeField] private int minLives = 6, maxLives = 10;
    [SerializeField] float minSpeed, maxSpeed = 3f, speedCap = 7f;
    [SerializeField] private int minDamage = 1, maxDamage = 1;
    private float spawnTime;

    //variables
    private bool displayRace, roundStarted;


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
        {
            _guiManager.SetRaceTime(GetRemainingTime(), 60.7f);
        }

        if (!roundStarted && _betweenRoundsTimer.GetElapsedSeconds() > timeBetweenRounds)
        {
            StartRound();
        }
    }

    #region rounds

    public void StartInBetweenRound()
    {
        StartCoroutine(ShowRoundCoroutine());
        roundStarted = false;
        // EnableShops();
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
        // ExitFromShops();
        // DisableShops();
        //spawn enemies
        int round = _playerData.GetRound();
        if (round % 3 == 0 && round > 0)
            IncreaseDifficulty(round);
        _enemyManager.StartRound(minNumEnemies, maxNumEnemies, minLives,
            maxLives, minSpeed, maxSpeed, minDamage,
            maxDamage, spawnTime);
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
        if (numRound < 25)
        {
            maxNumEnemies += 1;
        }
        else if (numRound < 35)
        {
            maxNumEnemies += 2;
        }
        else
        {
            maxNumEnemies += 4;
        }

        minNumEnemies = maxNumEnemies;

        //lives
        if (numRound < 35)
        {
            minLives += 1;
            maxLives += 2;
        }
        else
        {
            minLives += 2;
            maxLives += 6;
        }


        //damage
        if (numRound >= 20 && numRound < 30)
            maxDamage = 2;
        else if (numRound > 30)
            maxDamage = 3;

        //speed
        if (numRound >= 12 && numRound < 22)
            maxSpeed = Mathf.Min(speedCap, maxSpeed + 0.5f);
        else if (numRound >= 22)
        {
            maxSpeed = Mathf.Min(speedCap, maxSpeed + 1f);
        }

        minSpeed = maxSpeed / 2;

        //spawn time
        spawnTime = Random.Range(0.5f, 3f);
    }

    private string GetRemainingTime()
    {
        var aux = timeBetweenRounds * 1000 - _betweenRoundsTimer.GetElapsedMiliseconds();
        return MyUtils.GetCountdownTimeString(aux);
    }

    IEnumerator ShowRoundCoroutine()
    {
        _guiManager.ShowRound();
        _guiManager.SetRound(_playerData.GetRound() + "");
        yield return new WaitForSeconds(3);
        _guiManager.HideRound();
    }
}