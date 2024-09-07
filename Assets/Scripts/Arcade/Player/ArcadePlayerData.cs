using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arcade.Mechanics.Doors;
using Arcade.Mechanics.Granades;
using Mechanics.General_Inputs.Machine_gun_mode;
using Mechanics.Shoot.Bullets;
using Player.Observer_pattern;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(2)]
public class ArcadePlayerData : Subject, IObserver
{
    #region DATA

    [SerializeField] private int points;
    float pointMultiplier = 1, maxPointMultiplier = 6, consumibleMultiplier = 1;
    private int totalPoints;
    private int rounds = 0;

    private PlayerValues _playerValues;
    public ShootingMode currentShootingMode = ShootingMode.Manual;
    private int level = 0;
    public int unlockedGear = 3;

    //bullets
    private BulletType currentBulletType = BulletType.NormalBullet;
    private Dictionary<BulletType, int> bulletsAmmo, bulletsCurrentMaxAmmo, bulletsMaxAmmo;
    private Dictionary<ShootingMode, int> currentShootCooldown, minShootCooldown;
    private List<BulletType> bulletSlots;
    [SerializeField] private int unlockedBulletSlots = 1;

    //grenades
    private GrenadeType currentGrenadeType = GrenadeType.None;
    private Dictionary<GrenadeType, int> grenadesAmmo, GrenadesCurrentMaxAmmo, grenadesMaxAmmo;
    [SerializeField] private int unlockedGrenadeSlots = 0;
    private List<GrenadeType> grenadeSlots;

    //zones
    private Dictionary<ZonesArcade, bool> zonesUnlocked;

    //enemies
    private int enemiesKilled = 0;

    //auxiliar
    public bool isArmorWheelDisplayed;
    private GuiManager _guiManager;

    //power 
    [SerializeField] private bool power;

    //save data
    private JSONsaving _jsoNsaving;
    private SaveData _saveData;
    private LoadScreen _loadScreen;

    private NavMeshSurface _navMeshSurface;
    //navmesh

    #endregion

    private void Awake()
    {
        //save data
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _saveData = _jsoNsaving._saveData;

        //bullets
        InitBullets();
        //grenades
        InitGrenades();
        //shooting cooldowns
        InitShootingCooldowns();
        //zones unlocked
        InitZonesUnlocked();
    }

    private void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _guiManager = FindObjectOfType<GuiManager>();
        _loadScreen = FindObjectOfType<LoadScreen>();
        _playerValues.AddObserver(this);
        totalPoints = points;
        _guiManager.UpdatePointsText(points + "");
    }

    private void Update()
    {
    }


    public void SaveGame()
    {
        _jsoNsaving.SaveTheData();
    }


    #region Bullets

    private void InitBullets()
    {
        //ammo
        bulletsAmmo = new Dictionary<BulletType, int>();
        bulletsAmmo.Add(BulletType.NormalBullet, 100);
        bulletsAmmo.Add(BulletType.FreezeBullet, 0);
        bulletsAmmo.Add(BulletType.InstaKillBullet, 0);
        bulletsAmmo.Add(BulletType.BurnBullet, 0);
        bulletsAmmo.Add(BulletType.GuidedBullet, 0);
        bulletsAmmo.Add(BulletType.ShotgunBullet, 0);
        bulletsAmmo.Add(BulletType.ExplosiveBullet, 0);
        //max ammo
        bulletsCurrentMaxAmmo = new Dictionary<BulletType, int>();
        bulletsCurrentMaxAmmo.Add(BulletType.NormalBullet, 100);
        bulletsCurrentMaxAmmo.Add(BulletType.FreezeBullet, 20);
        bulletsCurrentMaxAmmo.Add(BulletType.InstaKillBullet, 1);
        bulletsCurrentMaxAmmo.Add(BulletType.BurnBullet, 20);
        bulletsCurrentMaxAmmo.Add(BulletType.GuidedBullet, 30);
        bulletsCurrentMaxAmmo.Add(BulletType.ShotgunBullet, 10);
        bulletsCurrentMaxAmmo.Add(BulletType.ExplosiveBullet, 5);
        //max ammo
        bulletsMaxAmmo = new Dictionary<BulletType, int>();
        bulletsMaxAmmo.Add(BulletType.NormalBullet, 1000);
        bulletsMaxAmmo.Add(BulletType.FreezeBullet, 200);
        bulletsMaxAmmo.Add(BulletType.InstaKillBullet, 10);
        bulletsMaxAmmo.Add(BulletType.BurnBullet, 200);
        bulletsMaxAmmo.Add(BulletType.GuidedBullet, 300);
        bulletsMaxAmmo.Add(BulletType.ShotgunBullet, 150);
        bulletsMaxAmmo.Add(BulletType.ExplosiveBullet, 25);
        //BulletSlots
        bulletSlots = new List<BulletType>(4)
            { BulletType.NormalBullet, BulletType.None, BulletType.None, BulletType.None };
    }

    private void InitShootingCooldowns()
    {
        currentShootCooldown = new Dictionary<ShootingMode, int>();
        currentShootCooldown.Add(ShootingMode.Manual, 1000);
        currentShootCooldown.Add(ShootingMode.Automatic, 1250);
        currentShootCooldown.Add(ShootingMode.Burst, 3000);
        minShootCooldown = new Dictionary<ShootingMode, int>();
        minShootCooldown.Add(ShootingMode.Manual, 150);
        minShootCooldown.Add(ShootingMode.Automatic, 175);
        minShootCooldown.Add(ShootingMode.Burst, 400);
    }

    public void SetBulletSlot(BulletType bulletType, int index)
    {
        if (index > 0 && index < bulletSlots.Capacity && index <= unlockedBulletSlots)
            bulletSlots[index] = bulletType;
    }

    public BulletType GetBulletSlot(int index)
    {
        return bulletSlots[index];
    }

    public BulletType[] GetBulletSlotsArray()
    {
        return bulletSlots.ToArray();
    }

    public int GetUnlockedSlot()
    {
        return unlockedBulletSlots;
    }

    public int GetBulletSlotsFilled()
    {
        for (int i = 0; i < bulletSlots.Count; i++)
        {
            if (bulletSlots[i] is BulletType.None)
                return i;
        }

        return bulletSlots.Count;
    }

    public int GetEmptySlot()
    {
        for (int i = 0; i < bulletSlots.Count; i++)
        {
            if (bulletSlots[i] is BulletType.None)
                return i;
        }

        return -1;
    }

    public bool ContainsBulletInArmor(BulletType type)
    {
        for (int i = 0; i < bulletSlots.Count; i++)
        {
            if (bulletSlots[i] == type)
                return true;
        }

        return false;
    }

    public void AddBullets(BulletType bulletType, int quantity)
    {
        bulletsAmmo[bulletType] += quantity;
    }

    public void SetCurrentBulletType(BulletType bulletType)
    {
        currentBulletType = bulletType;
    }

    public BulletType GetCurrentBulletType()
    {
        return currentBulletType;
    }

    public int GetNumCurrentBulletMaxAmmo(BulletType bulletType)
    {
        if (bulletType is BulletType.None)
            return 0;
        return bulletsCurrentMaxAmmo[bulletType];
    }

    public void IncreaseNumBulletSlots()
    {
        unlockedBulletSlots = Mathf.Min(unlockedBulletSlots + 1, bulletSlots.Capacity - 1);
    }

    public void IncreaseNumCurrentBulletMaxAmmo()
    {
        List<BulletType> keys = new List<BulletType>(bulletsCurrentMaxAmmo.Keys);
        foreach (var key in keys)
        {
            bulletsCurrentMaxAmmo[key] = Mathf.Min((int)(bulletsCurrentMaxAmmo[key] + bulletsMaxAmmo[key] * 0.2f),
                bulletsMaxAmmo[key]);
        }
    }

    public int GetNumBullets(BulletType bulletType)
    {
        if (bulletType is BulletType.None)
            return 0;
        return bulletsAmmo[bulletType];
    }

    public bool isFullAmmo(BulletType type)
    {
        return GetNumBullets(type) == GetNumCurrentBulletMaxAmmo(type);
    }

    public bool ShootBullet(BulletType bulletType)
    {
        if (bulletsAmmo[bulletType] - 1 < 0) return false;
        bulletsAmmo[bulletType] -= 1;
        return true;
    }

    #endregion

    #region Grenades

    private void InitGrenades()
    {
        grenadesAmmo = new Dictionary<GrenadeType, int>();
        grenadesAmmo.Add(GrenadeType.NormalGrenade, 0);
        grenadesAmmo.Add(GrenadeType.SmokeGrenade, 0);
        grenadesAmmo.Add(GrenadeType.FreezeGrenade, 0);

        GrenadesCurrentMaxAmmo = new Dictionary<GrenadeType, int>();
        GrenadesCurrentMaxAmmo.Add(GrenadeType.NormalGrenade, 2);
        GrenadesCurrentMaxAmmo.Add(GrenadeType.FreezeGrenade, 2);
        GrenadesCurrentMaxAmmo.Add(GrenadeType.SmokeGrenade, 2);

        grenadesMaxAmmo = new Dictionary<GrenadeType, int>();
        grenadesMaxAmmo.Add(GrenadeType.NormalGrenade, 10);
        grenadesMaxAmmo.Add(GrenadeType.SmokeGrenade, 5);
        grenadesMaxAmmo.Add(GrenadeType.FreezeGrenade, 10);
        grenadeSlots = new List<GrenadeType>(2) { GrenadeType.None, GrenadeType.None };
    }

    public void SetGrenadeSlot(GrenadeType grenadeType, int index)
    {
        if (index >= 0 && index < grenadeSlots.Capacity && index <= unlockedGrenadeSlots)
            grenadeSlots[index] = grenadeType;
    }


    public GrenadeType GetGrenadeSlot(int index)
    {
        if (index <= unlockedGrenadeSlots)
            return grenadeSlots[index];
        return GrenadeType.None;
    }

    public GrenadeType[] GetGrenadeSlotsArray()
    {
        return grenadeSlots.ToArray();
    }

    public int GetUnlockedGrenadeSlot()
    {
        return unlockedGrenadeSlots;
    }

    public int GetEmptyGrenadeSlot()
    {
        for (int i = 0; i < grenadeSlots.Count; i++)
        {
            if (grenadeSlots[i] is GrenadeType.None)
                return i;
        }

        return -1;
    }

    public int GetGrenadeSlotsFilled()
    {
        for (int i = 0; i < grenadeSlots.Count; i++)
        {
            if (grenadeSlots[i] is GrenadeType.None)
                return i;
        }

        return grenadeSlots.Count;
    }

    public bool ContainsGrenadeInArmor(GrenadeType type)
    {
        for (int i = 0; i < grenadeSlots.Count; i++)
        {
            if (grenadeSlots[i] == type)
                return true;
        }

        return false;
    }

    public void AddGrenades(GrenadeType grenadeType, int quantity)
    {
        grenadesAmmo[grenadeType] += quantity;
    }

    public void IncreaseNumGrenadeSlots()
    {
        unlockedGrenadeSlots = Mathf.Min(unlockedGrenadeSlots + 1, grenadeSlots.Capacity - 1);
    }

    public void SetCurrentGrenadeType(GrenadeType grenadeType)
    {
        currentGrenadeType = grenadeType;
    }

    public GrenadeType GetCurrentGrenadeType()
    {
        return currentGrenadeType;
    }

    public int GetNumCurrentGrenadeMaxAmmo(GrenadeType grenadeType)
    {
        if (grenadeType is GrenadeType.None)
            return 0;
        return GrenadesCurrentMaxAmmo[grenadeType];
    }

    public void IncreaseNumCurrentGrenadesMaxAmmo()
    {
        List<GrenadeType> keys = new List<GrenadeType>(GrenadesCurrentMaxAmmo.Keys);
        foreach (var key in keys)
            GrenadesCurrentMaxAmmo[key] = Mathf.Min((int)(GrenadesCurrentMaxAmmo[key] + grenadesMaxAmmo[key] * 0.2f),
                grenadesMaxAmmo[key]);
    }

    public int GetNumGrenades(GrenadeType grenadeType)
    {
        if (grenadeType is GrenadeType.None)
            return 0;
        return grenadesAmmo[grenadeType];
    }

    public bool isFullGrenadeAmmo(GrenadeType type)
    {
        return GetNumGrenades(type) == GetNumCurrentGrenadeMaxAmmo(type);
    }

    public bool throwGrenade(GrenadeType grenadeType)
    {
        if (grenadesAmmo[grenadeType] - 1 < 0) return false;
        grenadesAmmo[grenadeType] -= 1;
        return true;
    }

    #endregion

    #region Shooting mode

    public void SetShootingMode(ShootingMode shootingMode)
    {
        currentShootingMode = shootingMode;
    }

    public ShootingMode GetShootingMode()
    {
        return currentShootingMode;
    }

    public int GetShootingCooldown()
    {
        return currentShootCooldown[currentShootingMode];
    }

    public void IncreaseShootSpeed()
    {
        List<ShootingMode> keys = new List<ShootingMode>(currentShootCooldown.Keys);
        foreach (var key in keys)
            currentShootCooldown[key] = Mathf.Max(currentShootCooldown[key] - 100, minShootCooldown[key]);
    }

    #endregion

    #region points

    public void SetPoints(int val)
    {
        points = val;
    }

    public int GetPoints()
    {
        return points;
    }

    public void AddPoints(int val)
    {
        int updatedVal = (int)(val * pointMultiplier * consumibleMultiplier);
        totalPoints += updatedVal;
        points += updatedVal;
        _guiManager.InsertPoints("+" + GetPointsReduced(updatedVal));
        _guiManager.UpdatePointsText(points + "");
    }

    public void RemovePoints(int val)
    {
        points = Mathf.Max(0, points - val);
        _guiManager.InsertPoints("-" + GetPointsReduced(val), new Color(0.859f, 0.318f, 0.227f, 1));
        _guiManager.UpdatePointsText(points + "");
    }

    public void IncreasePointMultiplier()
    {
        pointMultiplier = Mathf.Min(maxPointMultiplier, pointMultiplier + 0.25f);
    }

    #endregion

    #region Consumibles

    public void FillAmmo()
    {
        foreach (var slot in bulletSlots.Where(slot => slot is not BulletType.None))
            bulletsAmmo[slot] = bulletsCurrentMaxAmmo[slot];
        foreach (var slot in grenadeSlots.Where(slot => slot is not GrenadeType.None))
            grenadesAmmo[slot] = GrenadesCurrentMaxAmmo[slot];
    }

    public void SetConsumibleMultipler(float val)
    {
        consumibleMultiplier = val;
    }

    #endregion

    #region zones

    private void InitZonesUnlocked()
    {
        zonesUnlocked = new Dictionary<ZonesArcade, bool>();
        zonesUnlocked.Add(ZonesArcade.Lobby, true);
        zonesUnlocked.Add(ZonesArcade.Trap, false);
        zonesUnlocked.Add(ZonesArcade.Library, false);
        zonesUnlocked.Add(ZonesArcade.Salon, false);
        zonesUnlocked.Add(ZonesArcade.Freezer, false);
        zonesUnlocked.Add(ZonesArcade.Tubes, false);
        zonesUnlocked.Add(ZonesArcade.Storage, false);
    }

    public void UnlockZone(ZonesArcade zone)
    {
        zonesUnlocked[zone] = true;
    }

    public bool GetZone(ZonesArcade zone)
    {
        return zonesUnlocked[zone];
    }

    public ZonesArcade[] GetUnlockedZonesArray()
    {
        List<ZonesArcade> zones = new List<ZonesArcade>();
        foreach (var zone in zonesUnlocked)
            if (zone.Value)
                zones.Add(zone.Key);
        return zones.ToArray();
    }

    public ZonesArcade[] GetUnlockedZonesArrayForMap()
    {
        List<ZonesArcade> zones = new List<ZonesArcade>();
        foreach (var zone in zonesUnlocked)
            if (zone.Value && zone.Key != ZonesArcade.Trap)
                zones.Add(zone.Key);
        return zones.ToArray();
    }

    public int GetUnlockedZones()
    {
        int count = 0;
        foreach (var zone in zonesUnlocked)
            if (zone.Value)
                count++;
        return count;
    }

    #endregion

    #region Lives

    public void AddLive()
    {
        _playerValues.AddMaxLives(1);
    }

    #endregion

    #region Gears

    public void AddGear()
    {
        unlockedGear = Mathf.Min(unlockedGear + 1, 4);
    }

    #endregion

    #region Power

    public void SetPower(bool val)
    {
        power = val;
    }

    public bool GetPower()
    {
        return power;
    }

    #endregion

    #region rounds

    public void IncreaseRound()
    {
        rounds++;
    }

    public int GetRound()
    {
        return rounds;
    }

    #endregion

    public void AddEnemiesKilled()
    {
        enemiesKilled++;
    }

    public void UpgradeLevel()
    {
        level++;
        if (level % 5 == 0 && GetUnlockedZonesArrayForMap().Length > 1)
            NotifyObservers(PlayerActions.ChangeUpgradeLocation);
    }

    public ArcadeStats GetArcadeStats()
    {
        return new ArcadeStats(GetPointsReduced(totalPoints), rounds, level, enemiesKilled,
            GetUnlockedZones() + "/" + zonesUnlocked.Count, (unlockedGear + 1) + "/" + 5,
            power ? "yes" : "no");
    }

    public string GetPointsReduced(int num)
    {
        if (num < 1000)
            return num + "";

        float result = (float)num / 1000;
        string cad = $"{result:F1}";
        return cad + "k";
    }

    public string GetStatsText()
    {
        string livesText = _playerValues.lives + "/" + _playerValues.GetMaxLives();
        float shootSpeed = 1 / ((float)currentShootCooldown[ShootingMode.Automatic] / 1000);
        string shootSpeedText = $"{shootSpeed:F2}dps";
        string pMul = $"{pointMultiplier:F2}";
        return "\n" + livesText + "\n" + level + "\n" + rounds + "\n" + GetPointsReduced(points) + "\n" +
               GetPointsReduced(totalPoints) + "\n" + "x" +
               pMul + "\n" + unlockedGear + "/4" + "\n" + shootSpeedText + "\n" + (unlockedBulletSlots + 1) +
               "/4" + "\n" + (unlockedGrenadeSlots + 1) + "/2" + "\n\n\n" +
               GetPointsReduced(bulletsAmmo[BulletType.NormalBullet]) + "/" +
               GetPointsReduced(bulletsCurrentMaxAmmo[BulletType.NormalBullet]) + "\n" +
               GetPointsReduced(bulletsAmmo[BulletType.FreezeBullet]) + "/" +
               GetPointsReduced(bulletsCurrentMaxAmmo[BulletType.FreezeBullet]) + "\n" +
               GetPointsReduced(bulletsAmmo[BulletType.ExplosiveBullet]) + "/" +
               GetPointsReduced(bulletsCurrentMaxAmmo[BulletType.ExplosiveBullet]) + "\n" +
               GetPointsReduced(bulletsAmmo[BulletType.GuidedBullet]) + "/" +
               GetPointsReduced(bulletsCurrentMaxAmmo[BulletType.GuidedBullet]) +
               "\n" + GetPointsReduced(bulletsAmmo[BulletType.ShotgunBullet]) + "/" +
               GetPointsReduced(bulletsCurrentMaxAmmo[BulletType.ShotgunBullet]) +
               "\n" + GetPointsReduced(bulletsAmmo[BulletType.BurnBullet]) + "/" +
               GetPointsReduced(bulletsCurrentMaxAmmo[BulletType.BurnBullet]) +
               "\n" + GetPointsReduced(bulletsAmmo[BulletType.InstaKillBullet]) +
               "/" + GetPointsReduced(bulletsCurrentMaxAmmo
                   [BulletType.InstaKillBullet]) + "\n\n\n" +
               grenadesAmmo[GrenadeType.NormalGrenade] +
               "/" +
               GrenadesCurrentMaxAmmo
                   [GrenadeType.NormalGrenade] + "\n" +
               grenadesAmmo[GrenadeType.FreezeGrenade] +
               "/" +
               GrenadesCurrentMaxAmmo
                   [GrenadeType.FreezeGrenade] + "\n" +
               grenadesAmmo[GrenadeType.SmokeGrenade] + "/" +
               GrenadesCurrentMaxAmmo[GrenadeType.SmokeGrenade] +
               "\n\n\n" + GetUnlockedZones() + "/" +
               zonesUnlocked.Count + "\n" + enemiesKilled;
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.TurnOnPower)
        {
            SetPower(true);
        }

        if (playerAction is PlayerActions.Die)
        {
            _saveData.AddScoreToSortedLeaderboard(new Score(_saveData.GetArcadeName(), rounds, totalPoints));
            _saveData.SetArcadeStats(GetArcadeStats());
            _jsoNsaving.SaveTheData();
            StartCoroutine(changeSceneCoroutine());
        }
    }

    IEnumerator changeSceneCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _loadScreen.LoadArcadeStatsScreen();
    }
}