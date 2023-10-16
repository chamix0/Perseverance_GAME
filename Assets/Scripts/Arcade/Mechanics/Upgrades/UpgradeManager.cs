using System;
using System.Collections;
using System.Collections.Generic;
using Arcade.Mechanics.Upgrades;
using Mechanics.General_Inputs;
using Player.Observer_pattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour, IObserver
{
    //values
    public bool isIn;
    private int prize = 100, prizeIncrease = 100, reRollPrize = 500;
    private bool limitReached;

    private int navegationIndex;

    //components
    [SerializeField] private CanvasGroup uiCanvas, limitReachedCanvasGroup;
    private GuiManager _guiManager;
    private PlayerValues _playerValues;
    private ArcadePlayerData _playerData;
    private CameraChanger _cameraChanger;
    private GenericScreenUi _genericScreenUi;
    private UpgradeBase upgradeBase;
    private MinigameSoundManager minigameSoundManager;

    //Buttons
    [SerializeField] private Button exitButton, reRoll;
    [SerializeField] private List<Button> slots;

    //texts
    [SerializeField] private TMP_Text yourPointsText, upGradeCostText, reRollCostText;
    private List<TMP_Text> slotTexts;

    //upgrades
    private List<Upgrade> _upgrades;
    private List<Upgrade> _selectedUpgrades;

    private void Awake()
    {
        _selectedUpgrades = new List<Upgrade>();
        slotTexts = new List<TMP_Text>();
    }

    void Start()
    {
        minigameSoundManager = FindObjectOfType<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerData.AddObserver(this);

        //button listeners
        //slots
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            slots[index].onClick.AddListener(() => SlotButtonAction(index));
            slotTexts.Add(slots[index].GetComponentInChildren<TMP_Text>());
        }

        InitUpgrades();

        //exit
        exitButton.onClick.AddListener(ExitButtonAction);
        //reroll
        reRoll.onClick.AddListener(ReRollButtonAction);
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
        _selectedUpgrades.AddRange(SelectThreeRandomUpgrades());
        navegationIndex = 0;
    }

    private void Update()
    {
    }

    #region Button actions

    private void SlotButtonAction(int index)
    {
        if (_playerData.GetPoints() >= prize)
        {
            _selectedUpgrades[index].ApplyUpgrade();
            _playerData.RemovePoints(prize);
            prize += prizeIncrease;
            _upgrades.Remove(_selectedUpgrades[index]);
            _playerData.UpgradeLevel();
            _guiManager.SetArcadeStatsText(_playerData.GetStatsText());
            minigameSoundManager.PlayClickSound();
            navegationIndex = index;
            if (AllCurrentUpgradesUsed())
            {
                _selectedUpgrades.Clear();
                _selectedUpgrades.AddRange(SelectThreeRandomUpgrades());
            }
        }

        if (!limitReached)
            UpdateButtons();
        UpdateTexts();
    }

    private void ReRollButtonAction()
    {
        if (_playerData.GetPoints() >= reRollPrize)
        {
            _selectedUpgrades.Clear();
            _selectedUpgrades.AddRange(SelectThreeRandomUpgrades());
            _playerData.RemovePoints(reRollPrize);
            minigameSoundManager.PlayClickSound();
        }

        UpdateButtons();
        UpdateTexts();
    }

    private void ExitButtonAction()
    {
        EndShop();
        exitButton.interactable = false;
        minigameSoundManager.PlayClickSound();
    }

    #endregion

    #region Update text and Buttons

    private void UpdateTexts()
    {
        //points
        int points = _playerData.GetPoints();

        //set texts
        yourPointsText.text = points + " pts";
        upGradeCostText.text = prize + " pts";
        reRollCostText.text = reRollPrize + " pts";
        if (prize > points)
            upGradeCostText.color = Color.red;
        else
            upGradeCostText.color = Color.green;

        if (reRollPrize > points)
            reRollCostText.color = Color.red;
        else
            reRollCostText.color = Color.green;

        for (int i = 0; i < slotTexts.Count; i++)
            slotTexts[i].text = i < _selectedUpgrades.Count ? _selectedUpgrades[i].GetText() : "-";
    }

    private void UpdateButtons()
    {
        //points
        int points = _playerData.GetPoints();

        if (prize > points)
            foreach (var slot in slots)
                slot.interactable = false;
        else
            foreach (var slot in slots)
                slot.interactable = true;

        for (var i = 0; i < slots.Count; i++)
            if (i >= _selectedUpgrades.Count || _selectedUpgrades[i].Used)
                slots[i].interactable = false;

        reRoll.interactable = reRollPrize <= points;
    }

    #endregion

    private void InitUpgrades()
    {
        _upgrades = new List<Upgrade>();
        _upgrades.Add(new Upgrade(UpgradeType.MoreGears, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreGears, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreLives, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreLives, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreLives, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreLives, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreLives, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreLives, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.IncreaseShootSpeed, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxBullets, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxBullets, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxBullets, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxBullets, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxBullets, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxBullets, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxGrenades, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxGrenades, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxGrenades, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxGrenades, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxGrenades, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MaxGrenades, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreGrenadeSlots, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreGrenadeSlots, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreBulletSlots, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.MoreBulletSlots, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
        _upgrades.Add(new Upgrade(UpgradeType.PointMultiplier, _playerData));
    }

    private Upgrade[] SelectThreeRandomUpgrades()
    {
        // GetRidOfUsedUpgrades();
        List<int> usedIndex = new List<int>();
        List<UpgradeType> differentUpgrades = new List<UpgradeType>();
        List<Upgrade> selectedItems = new List<Upgrade>();
        if (_upgrades.Count <= 3)
            return _upgrades.ToArray();

        if (HasToBeDuplicate())
        {
            do
            {
                int auxIndex = Random.Range(0, _upgrades.Count);
                Upgrade auxUpgrade = _upgrades[auxIndex];

                if (!usedIndex.Contains(auxIndex))
                {
                    usedIndex.Add(auxIndex);
                    selectedItems.Add(auxUpgrade);
                }
            } while (usedIndex.Count < 3);


            return selectedItems.ToArray();
        }

        do
        {
            int auxIndex = Random.Range(0, _upgrades.Count);
            Upgrade auxUpgrade = _upgrades[auxIndex];
            UpgradeType auxUpgradeType = auxUpgrade.UpgradeType;

            if (!usedIndex.Contains(auxIndex) && !differentUpgrades.Contains(auxUpgradeType))
            {
                usedIndex.Add(auxIndex);
                differentUpgrades.Add(auxUpgradeType);
                selectedItems.Add(auxUpgrade);
            }
        } while (usedIndex.Count < 3);

        return selectedItems.ToArray();
    }


    private bool HasToBeDuplicate()
    {
        List<UpgradeType> differentUpgrades = new List<UpgradeType>();
        for (int i = 0; i < _upgrades.Count; i++)
        {
            if (!differentUpgrades.Contains(_upgrades[i].UpgradeType))
            {
                differentUpgrades.Add(_upgrades[i].UpgradeType);
                if (differentUpgrades.Count >= 3)
                    return false;
            }
        }

        return true;
    }

    public void StartUpgrades(UpgradeBase newBase)
    {
        isIn = true;
        limitReachedCanvasGroup.alpha = 0;
        limitReached = false;
        upgradeBase = newBase;
        exitButton.interactable = true;
        // _selectedUpgrades.Clear();
        // _selectedUpgrades.AddRange(SelectThreeRandomUpgrades());

        UpdateTexts();
        UpdateButtons();

        _genericScreenUi.FadeOutText();
        _playerValues.SetCurrentInput(CurrentInput.Upgrade);
        _playerValues.SetInputsEnabled(true);
        ShowUI();
        //_playerValues.SetInputsEnabled(true);
        _guiManager.ShowArcadeStats();
        CursorManager.ShowCursor();
    }

    public void ShowUI()
    {
        uiCanvas.alpha = 1;
        uiCanvas.interactable = true;
        uiCanvas.blocksRaycasts = true;
    }

    public void HideUI()
    {
        uiCanvas.alpha = 0;
        uiCanvas.interactable = false;
        uiCanvas.blocksRaycasts = false;
    }

    private void BlockAllButtons()
    {
        foreach (var slot in slots)
            slot.interactable = false;
        reRoll.interactable = false;
    }

    public void SelectNext()
    {
        minigameSoundManager.PlayTapSound();
        navegationIndex = (navegationIndex + 1) % 3;
        HighlightButton(slots, navegationIndex);
    }

    public void SelectPrev()
    {
        minigameSoundManager.PlayTapSound();
        navegationIndex = navegationIndex - 1 < 0 ? 2 : navegationIndex - 1;
        HighlightButton(slots, navegationIndex);
    }

    public void HighlightButton(List<Button> buttons, int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == index)
                buttons[i].image.color = Color.black;
            else
                buttons[i].image.color = Color.white;
        }
    }

    public void SelectButton()
    {
        Button button = slots[navegationIndex];
        if (button.IsInteractable())
            button.onClick.Invoke();
    }

    public void TryReroll()
    {
        if (reRoll.IsInteractable())
            reRoll.onClick.Invoke();
    }

    private bool AllCurrentUpgradesUsed()
    {
        foreach (var upgrade in _selectedUpgrades)
        {
            if (!upgrade.Used)
                return false;
        }

        return true;
    }

    public void EndShop()
    {
        isIn = false;
        StartCoroutine(EndShopCoroutine());
    }

    IEnumerator EndShopCoroutine()
    {
        upgradeBase.ExitBase();
        yield return new WaitForSeconds(3f);
        HideUI();
        _playerValues.SetInputsEnabled(true);
        _cameraChanger.SetOrbitCamera();
        _genericScreenUi.FadeInText();
        CursorManager.HideCursor();
        _guiManager.HideArcadeStats();
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeUpgradeLocation)
        {
            limitReachedCanvasGroup.alpha = 1;
            limitReached = true;
            BlockAllButtons();
            minigameSoundManager.PlayInCorrectSound();
        }
    }
}