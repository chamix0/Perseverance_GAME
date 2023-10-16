using System;
using System.Collections.Generic;
using Arcade.Mechanics.Granades;
using Mechanics.General_Inputs.Machine_gun_mode;
using Mechanics.Shoot.Bullets;
using Player.Observer_pattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(5)]
public class ArmorWheel : MonoBehaviour, IObserver
{
    private ArcadePlayerData _playerData;

    private GuiManager _guiManager;

    //gamepad control
    private int selectedBulletIndex, selectedShootingModeIndex, selectedGrenadeIndex;
    private int verticalIndex;

    [SerializeField] private List<Image> framesBullets, framesShotingMode, framesGrenades;

    //bullets
    [SerializeField] private List<Button> bulletSlots;
    private List<Image> bulletSlotImages;
    [SerializeField] private List<TMP_Text> bulletSlotsText;
    [SerializeField] private List<TMP_Text> bulletSlotsAmmoText;


    //shootingMode
    [SerializeField] private List<Button> shootModeSlots;
    private List<Image> shootingModeImages;

    //grenades
    [SerializeField] private List<Button> grenadeSlots;
    [SerializeField] private List<TMP_Text> grenadeSlotsText;
    [SerializeField] private List<TMP_Text> grenadeSlotsAmmoText;
    private List<Image> grenadeSlotImages;
    private PlayerMechanicsArcadeManager _mechanicsArcadeManager;

    [SerializeField] private Color highlightedColor, normalColor;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");

    [SerializeField] private Shader _shader;
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    private static readonly int ScrollSpeed = Shader.PropertyToID("_scroll_speed");
    private static readonly int TilingStripes = Shader.PropertyToID("_tiling_stripes");
    private static readonly int FresnelColor = Shader.PropertyToID("_fresnel_color");

    private void Awake()
    {
        bulletSlotImages = new List<Image>();
        shootingModeImages = new List<Image>();
        grenadeSlotImages = new List<Image>();
    }

    void Start()
    {
        _guiManager = FindObjectOfType<GuiManager>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _mechanicsArcadeManager = FindObjectOfType<PlayerMechanicsArcadeManager>();
        _guiManager.AddObserver(this);

        for (int i = 0; i < bulletSlots.Count; i++)
        {
            Image image = bulletSlots[i].GetComponent<Image>();
            image.material = new Material(_shader);
            image.material.SetFloat(Alpha, 1);
            image.material.SetFloat(ScrollSpeed, 0.1f);
            image.material.SetVector(TilingStripes, new Vector4(1, 1.32f, 0, 0));
            image.material.SetColor(FresnelColor, Color.white);

            bulletSlotImages.Add(image);
        }

        for (int i = 0; i < shootModeSlots.Count; i++)
        {
            Image image = shootModeSlots[i].GetComponent<Image>();
            image.material = new Material(_shader);
            image.material.SetFloat(Alpha, 1);
            image.material.SetFloat(ScrollSpeed, 0.1f);
            image.material.SetVector(TilingStripes, new Vector4(1, 1.32f, 0, 0));
            image.material.SetColor(FresnelColor, Color.white);

            shootingModeImages.Add(image);
        }

        for (int i = 0; i < grenadeSlots.Count; i++)
        {
            Image image = grenadeSlots[i].GetComponent<Image>();
            image.material = new Material(_shader);
            image.material.SetFloat(Alpha, 1);
            image.material.SetFloat(ScrollSpeed, 0.1f);
            image.material.SetVector(TilingStripes, new Vector4(1, 1.32f, 0, 0));
            image.material.SetColor(FresnelColor, Color.white);
            grenadeSlotImages.Add(image);
        }

        SetButtons();
        UpdateBulletButtons();
        UpdateGrenadesButtons();
        HighlightButtons(0, bulletSlotImages);
        HighlightButtons(0, shootingModeImages);
        HighlightButtons(-1, grenadeSlotImages);
        _guiManager.SetBulletTypeIcon(_playerData.GetCurrentBulletType());
        _guiManager.SetShootingModeIcon((int)_playerData.GetShootingMode());
        _guiManager.SetGrenadeIcon((int)_playerData.GetCurrentGrenadeType());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetButtons()
    {
        BulletType[] slotsBullets = _playerData.GetBulletSlotsArray();
        for (int i = 0; i < slotsBullets.Length; i++)
        {
            int aux = i;
            bulletSlots[i].onClick.AddListener(() =>
            {
                OnClickBullets(aux);
                selectedBulletIndex = aux;
                verticalIndex = 0;
            });
        }

        GrenadeType[] slotsGrenades = _playerData.GetGrenadeSlotsArray();
        for (int i = 0; i < slotsGrenades.Length; i++)
        {
            int aux = i;
            grenadeSlots[i].onClick.AddListener(() =>
            {
                OnClickGrenades(aux);
                selectedBulletIndex = aux;
                verticalIndex = 2;
            });
        }

        shootModeSlots[0].onClick.AddListener(() =>
        {
            OnClickShootingMode(ShootingMode.Manual);
            selectedShootingModeIndex = 0;
            verticalIndex = 1;
        });
        shootModeSlots[1].onClick.AddListener(() =>
        {
            OnClickShootingMode(ShootingMode.Automatic);
            selectedShootingModeIndex = 1;
            verticalIndex = 1;
        });
        shootModeSlots[2].onClick.AddListener(() =>
        {
            OnClickShootingMode(ShootingMode.Burst);
            selectedShootingModeIndex = 2;
            verticalIndex = 1;
        });
    }


    private void UpdateBulletButtons()
    {
        BulletType[] slots = _playerData.GetBulletSlotsArray();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == BulletType.None)
                bulletSlots[i].interactable = false;
            else
                bulletSlots[i].interactable = true;

            bulletSlotsText[i].text = BulletTypeToString(slots[i]);
            if (slots[i] is BulletType.NormalBullet && _playerData.GetShootingMode() is ShootingMode.Manual)
                bulletSlotsAmmoText[i].text = "inf";
            else
                bulletSlotsAmmoText[i].text = "" + _playerData.GetNumBullets(slots[i]);
        }
    }

    private void UpdateGrenadesButtons()
    {
        GrenadeType[] slots = _playerData.GetGrenadeSlotsArray();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == GrenadeType.None)
                grenadeSlots[i].interactable = false;
            else
                grenadeSlots[i].interactable = true;

            grenadeSlotsText[i].text = GrenadeTypeToString(slots[i]);
            grenadeSlotsAmmoText[i].text = "" + _playerData.GetNumGrenades(slots[i]);
        }
    }


    private void OnClickBullets(int index)
    {
        _playerData.SetCurrentBulletType(_playerData.GetBulletSlot(index));
        HighlightButtons(index, bulletSlotImages);
        HighlightFrame(index, 0);
        _guiManager.SetBulletTypeIcon(_playerData.GetCurrentBulletType());
    }

    private void OnClickShootingMode(ShootingMode shootingMode)
    {
        _playerData.SetShootingMode(shootingMode);
        UpdateBulletButtons();
        HighlightButtons(ShootingModeToIndex(shootingMode), shootingModeImages);
        HighlightFrame(ShootingModeToIndex(shootingMode), 1);
        //select 
        _guiManager.SetShootingModeIcon((int)_playerData.GetShootingMode());
    }

    private void OnClickGrenades(int index)
    {
        _playerData.SetCurrentGrenadeType(_playerData.GetGrenadeSlot(index));
        UpdateGrenadesButtons();
        HighlightButtons(index, grenadeSlotImages);
        HighlightFrame(index, 2);
        _mechanicsArcadeManager.PrepareGrenade();
        _guiManager.SetGrenadeIcon((int)_playerData.GetCurrentGrenadeType());
    }

    public void SelectNext()
    {
        switch (verticalIndex)
        {
            case 0:
                selectedBulletIndex = (selectedBulletIndex + 1) % _playerData.GetBulletSlotsFilled();
                OnClickBullets(selectedBulletIndex);
                HighlightButtons(selectedBulletIndex, bulletSlotImages);
                break;
            case 1:
                selectedShootingModeIndex = (selectedShootingModeIndex + 1) % 3;
                OnClickShootingMode(IndexToShootingMode(selectedShootingModeIndex));
                HighlightButtons(selectedShootingModeIndex, shootingModeImages);
                break;
            case 2:
                if (_playerData.GetGrenadeSlotsFilled() > 0)
                {
                    selectedGrenadeIndex = (selectedGrenadeIndex + 1) % _playerData.GetGrenadeSlotsFilled();
                    OnClickGrenades(selectedGrenadeIndex);
                    HighlightButtons(selectedGrenadeIndex, grenadeSlotImages);
                }

                break;
        }
    }

    public void SelectNextBullets()
    {
        selectedBulletIndex = (selectedBulletIndex + 1) % _playerData.GetBulletSlotsFilled();
        OnClickBullets(selectedBulletIndex);
        HighlightButtons(selectedBulletIndex, bulletSlotImages);
    }

    public void SelectPrevBullets()
    {
        selectedBulletIndex =
            selectedBulletIndex - 1 < 0 ? _playerData.GetBulletSlotsFilled() - 1 : selectedBulletIndex - 1;
        OnClickBullets(selectedBulletIndex);
        HighlightButtons(selectedBulletIndex, bulletSlotImages);
    }

    public void SelectNextShootingMode()
    {
        selectedShootingModeIndex = (selectedShootingModeIndex + 1) % 3;
        OnClickShootingMode(IndexToShootingMode(selectedShootingModeIndex));
        HighlightButtons(selectedShootingModeIndex, shootingModeImages);
    }

    public void SelectNextGrenade()
    {
        if (_playerData.GetGrenadeSlotsFilled() > 0)
        {
            selectedGrenadeIndex = (selectedGrenadeIndex + 1) % _playerData.GetGrenadeSlotsFilled();
            OnClickGrenades(selectedGrenadeIndex);
            HighlightButtons(selectedGrenadeIndex, grenadeSlotImages);
        }
    }


    public void SelectPrev()
    {
        switch (verticalIndex)
        {
            case 0:
                selectedBulletIndex =
                    selectedBulletIndex - 1 < 0 ? _playerData.GetBulletSlotsFilled() - 1 : selectedBulletIndex - 1;
                OnClickBullets(selectedBulletIndex);
                HighlightButtons(selectedBulletIndex, bulletSlotImages);
                break;
            case 1:
                selectedShootingModeIndex = selectedShootingModeIndex - 1 < 0
                    ? 2
                    : selectedShootingModeIndex - 1;
                OnClickShootingMode(IndexToShootingMode(selectedShootingModeIndex));
                HighlightButtons(selectedShootingModeIndex, shootingModeImages);
                break;
            case 2:
                if (_playerData.GetGrenadeSlotsFilled() > 0)
                {
                    selectedGrenadeIndex = (selectedGrenadeIndex + 1) % _playerData.GetGrenadeSlotsFilled();
                    OnClickGrenades(selectedGrenadeIndex);
                    HighlightButtons(selectedGrenadeIndex, grenadeSlotImages);
                }

                break;
        }
    }

    public void SelectUp()
    {
        verticalIndex = verticalIndex - 1 < 0 ? 2 : verticalIndex - 1;
        if (verticalIndex == 0)
            HighlightFrame(selectedBulletIndex, 0);
        else if (verticalIndex == 1)
            HighlightFrame(selectedShootingModeIndex, 1);
        else if (verticalIndex == 2)
            HighlightFrame(selectedGrenadeIndex, 2);
    }

    public void SelectDown()
    {
        verticalIndex = (verticalIndex + 1) % 3;

        if (verticalIndex == 0)
            HighlightFrame(selectedBulletIndex, 0);
        else if (verticalIndex == 1)
            HighlightFrame(selectedShootingModeIndex, 1);
        else if (verticalIndex == 2)
            HighlightFrame(selectedGrenadeIndex, 2);
    }

    private void HighlightFrame(int mode, int vertical)
    {
        foreach (var frame in framesBullets)
            frame.color = Color.white;


        foreach (var frame in framesShotingMode)
            frame.color = Color.white;


        foreach (var frame in framesGrenades)
            frame.color = Color.white;


        if (vertical == 0)
            framesBullets[mode].color = new Color(0.2364276f, 0.4422438f, 0.7264151f, 1);
        else if (vertical == 1)
            framesShotingMode[mode].color = new Color(0.2364276f, 0.4422438f, 0.7264151f, 1);
        else if (vertical == 2)
            framesGrenades[mode].color = new Color(0.2364276f, 0.4422438f, 0.7264151f, 1);
    }

    private void HighlightButtons(int index, List<Image> images)
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (i == index)
                images[i].material.SetColor(BackgroundColor, highlightedColor);
            else
                images[i].material.SetColor(BackgroundColor, normalColor);
        }
    }

    public static string ShootingTypeToString(ShootingMode shootingMode)
    {
        return shootingMode switch
        {
            ShootingMode.Automatic => "Automatic",
            ShootingMode.Manual => "Manual",
            ShootingMode.Burst => "Burst",
            _ => throw new ArgumentOutOfRangeException(nameof(shootingMode), shootingMode, null)
        };
    }

    public static string BulletTypeToString(BulletType bulletType)
    {
        return bulletType switch
        {
            BulletType.None => "-",
            BulletType.NormalBullet => "Regular Bullet",
            BulletType.BurnBullet => "Burn Bullet",
            BulletType.ExplosiveBullet => "Explosive Bullet",
            BulletType.FreezeBullet => "Freeze Bullet",
            BulletType.GuidedBullet => "Guided Bullet",
            BulletType.ShotgunBullet => "shotGun Bullet",
            BulletType.InstaKillBullet => "Insta Kill Bullet"
        };
    }

    public static string GrenadeTypeToString(GrenadeType grenadeType)
    {
        return grenadeType switch
        {
            GrenadeType.None => "-",
            GrenadeType.NormalGrenade => "Normal Grenade",
            GrenadeType.FreezeGrenade => "Freeze Grenade",
            GrenadeType.SmokeGrenade => "Smoke Grenade"
        };
    }

    private int ShootingModeToIndex(ShootingMode shootingMode)
    {
        return shootingMode switch
        {
            ShootingMode.Automatic => 1,
            ShootingMode.Manual => 0,
            ShootingMode.Burst => 2
        };
    }

    private ShootingMode IndexToShootingMode(int index)
    {
        return index switch
        {
            0 => ShootingMode.Manual,
            1 => ShootingMode.Automatic,
            2 => ShootingMode.Burst,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.OpenArmorWheel)
        {
            UpdateBulletButtons();
            UpdateGrenadesButtons();
        }
        else if (playerAction is PlayerActions.ShowArcadeStats)
        {
            _guiManager.SetArcadeStatsText(_playerData.GetStatsText());
        }
    }
}