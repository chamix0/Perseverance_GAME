using System.Collections;
using System.Collections.Generic;
using Arcade.Mechanics.Bullets;
using Arcade.Mechanics.Granades;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BulletShopManager : MonoBehaviour
{
    //values
    private BulletType currentBulletType;
    private GrenadeType currentGrenadeType;
    private int prize;
    public bool isIn;

    private int navegationIndex;

    //components
    [SerializeField] private CanvasGroup uiCanvas;
    private PlayerValues _playerValues;
    private ArcadePlayerData _playerData;
    private CameraChanger _cameraChanger;
    private GenericScreenUi _genericScreenUi;
    private BulletShopBase shopBase;
    private ArmorWheel armorWheel;
    private MinigameSoundManager minigameSoundManager;

    //canvas groups
    [SerializeField] private CanvasGroup slotScreen;
    [SerializeField] private CanvasGroup buyScreen;

    //Buttons
    [SerializeField] private Button exitButton, plusOne, plusTen, plusMax;
    [SerializeField] private List<Button> slots;

    //texts
    [SerializeField] private TMP_Text yourPointsText;

    [FormerlySerializedAs("yourBulletsText")] [SerializeField]
    private TMP_Text yourUnitsText;

    [SerializeField] private TMP_Text plusOnePrizeText, plusTenPrizeText, plusMaxPrizeText;
    [SerializeField] private List<TMP_Text> slotTexts;


    void Start()
    {
        minigameSoundManager = FindObjectOfType<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
        armorWheel = FindObjectOfType<ArmorWheel>();
        //button listeners
        //slots
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            slots[i].onClick.AddListener(() => SlotButtonAction(index));
        }

        //buy
        plusOne.onClick.AddListener(() => buyButtonAction(1));
        plusTen.onClick.AddListener(() => buyButtonAction(10));
        plusMax.onClick.AddListener(buyMaxButtonAction);
        //exit
        exitButton.onClick.AddListener(ExitButtonAction);

        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }

    #region Button actions

    private void SlotButtonAction(int index)
    {
        if (shopBase.isGrenade)
        {
            _playerData.SetGrenadeSlot(currentGrenadeType, index);
            _playerData.SetCurrentGrenadeType(currentGrenadeType);
        }
        else
        {
            _playerData.SetBulletSlot(currentBulletType, index + 1);
            _playerData.SetCurrentBulletType(currentBulletType);
        }

        minigameSoundManager.PlayClickSound();
        HideSlotScreen();
        ShowBuyScreen();
    }

    private void buyButtonAction(int quantity)
    {
        //points
        int points = _playerData.GetPoints();
        //units
        int maxUnits = shopBase.isGrenade
            ? _playerData.GetNumCurrentGrenadeMaxAmmo(currentGrenadeType)
            : _playerData.GetNumCurrentBulletMaxAmmo(currentBulletType);
        int units = shopBase.isGrenade
            ? _playerData.GetNumGrenades(currentGrenadeType)
            : _playerData.GetNumBullets(currentBulletType);

        int prizeAux = quantity * prize;
        int desiredUnitsNumber = units + quantity;

        if (points > prizeAux && desiredUnitsNumber <= maxUnits)
        {
            minigameSoundManager.PlayClickSound();
            _playerData.RemovePoints(prizeAux);
            if (shopBase.isGrenade)
                _playerData.AddGrenades(currentGrenadeType, quantity);
            else
                _playerData.AddBullets(currentBulletType, quantity);
        }

        //update Texts
        UpdateTexts();

        //update buttons
        UpdateButtons();
    }


    private void buyMaxButtonAction()
    {
        //points
        int points = _playerData.GetPoints();
        //units
        int maxUnits = shopBase.isGrenade
            ? _playerData.GetNumCurrentGrenadeMaxAmmo(currentGrenadeType)
            : _playerData.GetNumCurrentBulletMaxAmmo(currentBulletType);
        int units = shopBase.isGrenade
            ? _playerData.GetNumGrenades(currentGrenadeType)
            : _playerData.GetNumBullets(currentBulletType);

        int unitsPlayerNeeds = maxUnits - units;
        int prizeMax = unitsPlayerNeeds * prize;

        if (points > prizeMax)
        {
            minigameSoundManager.PlayClickSound();
            _playerData.RemovePoints(prizeMax);
            if (shopBase.isGrenade)
                _playerData.AddGrenades(currentGrenadeType, unitsPlayerNeeds);
            else
                _playerData.AddBullets(currentBulletType, unitsPlayerNeeds);
        }

        //Update Texts
        UpdateTexts();

        //updateButtons
        UpdateButtons();
    }

    public void ExitButtonAction()
    {
        minigameSoundManager.PlayClickSound();
        EndShop();
        exitButton.interactable = false;
    }

    #endregion

    #region Update text and Buttons

    private void UpdateTexts()
    {
        //points
        int points = _playerData.GetPoints();
        //units
        int maxUnits = shopBase.isGrenade
            ? _playerData.GetNumCurrentGrenadeMaxAmmo(currentGrenadeType)
            : _playerData.GetNumCurrentBulletMaxAmmo(currentBulletType);
        int units = shopBase.isGrenade
            ? _playerData.GetNumGrenades(currentGrenadeType)
            : _playerData.GetNumBullets(currentBulletType);

        int unitsPlayerNeeds = maxUnits - units;

        //set texts
        yourPointsText.text = points + " pts";
        yourUnitsText.text = units + "/" + maxUnits;

        plusOnePrizeText.text = prize + " pts";
        plusTenPrizeText.text = (prize * 10) + " pts";
        plusMaxPrizeText.text = (prize * unitsPlayerNeeds) + " pts";

        if (prize > _playerData.GetPoints())
            plusOnePrizeText.color = Color.red;
        else
            plusOnePrizeText.color = Color.green;


        if (prize * unitsPlayerNeeds > _playerData.GetPoints())
            plusMaxPrizeText.color = Color.red;
        else
            plusMaxPrizeText.color = Color.green;

        if (_playerData.GetPoints() > prize * 10)
            plusTenPrizeText.color = Color.green;
        else
            plusTenPrizeText.color = Color.red;

          

        for (int i = 0; i < slotTexts.Count; i++)
        {
            if (shopBase.isGrenade)
                slotTexts[i].text = ArmorWheel.GrenadeTypeToString(_playerData.GetGrenadeSlot(i));
            else
                slotTexts[i].text = ArmorWheel.BulletTypeToString(_playerData.GetBulletSlot(i + 1));
        }
    }

    private void UpdateButtons()
    {
        //units
        int maxUnits = shopBase.isGrenade
            ? _playerData.GetNumCurrentGrenadeMaxAmmo(currentGrenadeType)
            : _playerData.GetNumCurrentBulletMaxAmmo(currentBulletType);
        int units = shopBase.isGrenade
            ? _playerData.GetNumGrenades(currentGrenadeType)
            : _playerData.GetNumBullets(currentBulletType);

        bool playerIsFullAmmo = shopBase.isGrenade
            ? _playerData.isFullGrenadeAmmo(currentGrenadeType)
            : _playerData.isFullAmmo(currentBulletType);
        int unitsPlayerNeeds = maxUnits - units;
        int prizeMax = unitsPlayerNeeds * prize;


        if (prize > _playerData.GetPoints() || playerIsFullAmmo)
            plusOne.interactable = false;
        else
            plusOne.interactable = true;
        

        if (prizeMax > _playerData.GetPoints() || playerIsFullAmmo)
            plusMax.interactable = false;
        else
            plusMax.interactable = true;
        
        if (prize * 10 > _playerData.GetPoints() || playerIsFullAmmo)
            plusTen.interactable = false;
        else
            plusTen.interactable = true;

        if (units + 10 > maxUnits)
            plusTen.interactable = false;
        else if (_playerData.GetPoints() > prize * 10 && !playerIsFullAmmo && units + 10 <= maxUnits)
            plusTen.interactable = true;

        for (int i = 0; i < slots.Count; i++)
        {
            if (shopBase.isGrenade)
                slots[i].interactable = i <= _playerData.GetUnlockedGrenadeSlot();
            else
                slots[i].interactable = i < _playerData.GetUnlockedSlot();
        }
    }

    #endregion


    public void StartShop(BulletType type, GrenadeType grenadeType, BulletShopBase actualBase, int prize)
    {
        isIn = true;
        this.prize = prize;
        currentBulletType = type;
        currentGrenadeType = grenadeType;
        shopBase = actualBase;
        exitButton.interactable = true;
        UpdateTexts();
        UpdateButtons();
        _genericScreenUi.FadeOutText();
        HideBuyScreen();
        HideSlotScreen();
        //check if player has the bullet in armot
        if (shopBase.isGrenade)
        {
            if (_playerData.ContainsGrenadeInArmor(currentGrenadeType))
            {
                ShowBuyScreen();
            }
            else
            {
                //check if there is a empty slot
                int emptySlot = _playerData.GetEmptyGrenadeSlot();
                if (emptySlot >= 0 && emptySlot <= _playerData.GetUnlockedGrenadeSlot())
                {
                    _playerData.SetGrenadeSlot(currentGrenadeType, emptySlot);
                    ShowBuyScreen();
                }
                else
                {
                    ShowSlotScreen();
                }
            }
        }
        else
        {
            if (_playerData.ContainsBulletInArmor(currentBulletType))
            {
                ShowBuyScreen();
            }
            else
            {
                //check if there is a empty slot
                int emptySlot = _playerData.GetEmptySlot();
                if (emptySlot > 0 && emptySlot <= _playerData.GetUnlockedSlot())
                {
                    _playerData.SetBulletSlot(currentBulletType, emptySlot);
                    ShowBuyScreen();
                }
                else
                {
                    ShowSlotScreen();
                }
            }
        }

        _playerValues.SetCurrentInput(CurrentInput.Shop);
        _playerValues.SetInputsEnabled(true);
        ShowUI();
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

    private void ShowBuyScreen()
    {
        buyScreen.interactable = true;
        buyScreen.alpha = 1;
        buyScreen.blocksRaycasts = true;
    }

    private void HideBuyScreen()
    {
        buyScreen.interactable = false;
        buyScreen.alpha = 0;
        buyScreen.blocksRaycasts = false;
    }

    private void ShowSlotScreen()
    {
        slotScreen.interactable = true;
        slotScreen.alpha = 1;
        slotScreen.blocksRaycasts = true;
    }

    private void HideSlotScreen()
    {
        slotScreen.interactable = false;
        slotScreen.alpha = 0;
        slotScreen.blocksRaycasts = false;
    }

    public void SelectNext()
    {
        minigameSoundManager.PlayTapSound();

        navegationIndex = (navegationIndex + 1) % 3;
        if (slotScreen.alpha >= 1)
        {
            HighlightButton(slots, navegationIndex);
        }
        else
        {
            List<Button> priceButtons = new List<Button>(new[] { plusOne, plusTen, plusMax });
            HighlightButton(priceButtons, navegationIndex);
        }
    }

    public void SelectPrev()
    {
        minigameSoundManager.PlayTapSound();
        navegationIndex = navegationIndex - 1 < 0 ? 2 : navegationIndex - 1;
        if (slotScreen.alpha >= 1)
        {
            HighlightButton(slots, navegationIndex);
        }
        else
        {
            List<Button> priceButtons = new List<Button>(new[] { plusOne, plusTen, plusMax });
            HighlightButton(priceButtons, navegationIndex);
        }
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
        Button button;
        if (slotScreen.alpha >= 1)
        {
            button = slots[navegationIndex];
        }
        else
        {
            List<Button> priceButtons = new List<Button>(new[] { plusOne, plusTen, plusMax });
            button = priceButtons[navegationIndex];
        }

        if (button.IsInteractable())
            button.onClick.Invoke();
    }


    public void EndShop()
    {
        isIn = false;
        StartCoroutine(EndShopCoroutine());
    }

    IEnumerator EndShopCoroutine()
    {
        shopBase.ExitBase();
        yield return new WaitForSeconds(3f);
        HideUI();
        _playerValues.SetInputsEnabled(true);
        _cameraChanger.SetOrbitCamera();
        _genericScreenUi.FadeInText();
        CursorManager.HideCursor();
    }
}