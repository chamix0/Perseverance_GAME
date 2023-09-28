using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Arcade.Mechanics.Bullets;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GuiManager : Subject
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CanvasGroup canvasGroup;
    private ShowCenterReference showCenterReference;
    private OrbitCameraController _cameraController;

    private SoundManager _soundManager;

    //last move
    [Header("last move")] [SerializeField] private CanvasGroup lastMoveObject;
    [SerializeField] private TMP_Text lastMoveText;

    //machine gun
    [Header("machine gun")] [SerializeField]
    private Image machinegunImage;

    [SerializeField] private TMP_Text machinegunText;
    [SerializeField] private List<Sprite> shootingModeImages;
    [SerializeField] private List<String> shootingModeTexts;

    //race
    [Header("race")] [SerializeField] private Image raceImage;
    [SerializeField] private TMP_Text raceText;

    //dialog
    [Header("Dialog")] [SerializeField] private CanvasGroup dialogObject;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text message;
    [SerializeField] private List<TMP_Text> answers;
    [SerializeField] private Image avatarImage;

    //objetives
    [Header("objetives")] [SerializeField] private CanvasGroup objetiveObject;
    [SerializeField] private TMP_Text objetiveText;

    //pause
    [Header("Pause")] [SerializeField] private CanvasGroup pauseIndicator;
    [SerializeField] private CanvasGroup pausePanel;
    [SerializeField] private TMP_Text pauseMoveText;
    [SerializeField] private Slider pauseProgressSlider;

    //tutorial
    [Header("Tutorial")] [SerializeField] private GameObject tutorialGameObject;
    [SerializeField] private TMP_Text tutorialText;

    //gears
    [Header("Gears")] [SerializeField] private GameObject gearsGameObject;
    [SerializeField] private List<Image> gearImages;
    private int currentGear = 0;

    //crosshair
    [Header("CrossHair")] [SerializeField] private CanvasGroup crosshair;
    [SerializeField] RectTransform crossHairRectTransform;

    // arcade
    // armor wheel
    [Header("Armor wheel")] [SerializeField]
    private CanvasGroup ArmorWheel;

    //message
    [Header("Message")] [SerializeField] private CanvasGroup messageCanvasGroup;
    [SerializeField] private TMP_Text messageText;

    //message
    [Header("Arcade Stats")] [SerializeField]
    private CanvasGroup statsCanvasGroup;

    [SerializeField] private TMP_Text statsText;

    //points
    [Header("Arcade points")] [SerializeField]
    private CanvasGroup pointsCanvasGroup;

    private RectTransform emerginPointsTransform;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private EmerginPointsPool _emergingPoints;

    //lives
    [Header("Lives")] [SerializeField] private CanvasGroup livesCanvas;
    [SerializeField] private TMP_Text livesText;
    private int currentLives;

    void Start()
    {
        _cameraController = FindObjectOfType<OrbitCameraController>();
        _soundManager = FindObjectOfType<SoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        if (_emergingPoints)
            emerginPointsTransform = _emergingPoints.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        showCenterReference = GetComponentInChildren<ShowCenterReference>();
        if (lastMoveText)
            lastMoveText.text = "";
        if (lastMoveObject)
            lastMoveObject.alpha = 0;
        if (machinegunText)
        {
            machinegunImage.sprite = null;
            machinegunImage.color = Color.clear;
            machinegunText.text = "";
        }

        currentLives = 0;
        raceImage.color = Color.clear;
        raceText.text = "";
        HideArmorWheel();
        HideDialog();
        HideObjetives();
        HidePauseIndicator();
        HidePausePanel();
        if (messageCanvasGroup)
            HideMessage();
    }

    private void Update()
    {
        //gears
        if (_playerValues.GetGear() != currentGear)
        {
            currentGear = _playerValues.GetGear();
            HighlightGear(currentGear);
        }

        if (livesText && currentLives != _playerValues.lives)
        {
            currentLives = _playerValues.lives;
            SetLives(currentLives);
        }
    }

    public void ShowGui()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public void HideGui()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    public void SetLastMoveText(Move move)
    {
        lastMoveObject.alpha = 1;
        lastMoveText.text = move.ToString();
        ShowCenters();
    }

    public void SetMachinegun(int index)
    {
        if (machinegunText)
        {
            if (index == -1)
            {
                machinegunImage.color = Color.clear;
                machinegunText.text = "";
                return;
            }

            machinegunImage.color = Color.clear;
            machinegunImage.sprite = shootingModeImages[index];
            machinegunText.text = shootingModeTexts[index];
        }
    }

    #region Race

    public void SetRaceTime(string time, float size)
    {
        raceText.fontSize = size;
        raceImage.color = CopyColorChangAlpha(raceImage.color, 0.2f);
        raceText.text = time;
    }

    public void DisableRace()
    {
        raceImage.color = Color.clear;
        raceText.text = "";
    }

    #endregion

    private void ShowCenters()
    {
        showCenterReference.ShowColors();
    }

    #region Arcade

    #region armor wheel

    public void ShowArmorWheel()
    {
        if (ArmorWheel)
        {
            crosshair.alpha = 0;
            ArmorWheel.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            ArmorWheel.blocksRaycasts = true;
            ArmorWheel.interactable = true;

            Time.timeScale = 0.0125f;
            CursorManager.ShowCursor();
            _cameraController.SlowCameraSpeed();
            _soundManager.SetMuteVFX(true);
            NotifyObservers(PlayerActions.OpenArmorWheel);
        }
    }

    public void HideArmorWheel()
    {
        if (ArmorWheel)
        {
            crosshair.alpha = 1;
            ArmorWheel.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            ArmorWheel.blocksRaycasts = false;
            ArmorWheel.interactable = false;
            Time.timeScale = 1f;
            CursorManager.HideCursor();
            _cameraController.ReturnToNormalCameraSpeed();
            _soundManager.SetMuteVFX(false);
            NotifyObservers(PlayerActions.CloseArmorWheel);
        }
    }

    #endregion

    #endregion

    #region Dialog

    public void ShowDialog()
    {
        dialogObject.alpha = 1;
        crosshair.alpha = 0;
    }

    public void HideDialog()
    {
        dialogObject.alpha = 0;
        crosshair.alpha = 1;
    }

    public void SetDialogName(string cad)
    {
        name.text = cad;
    }

    public void SetDialogMesasge(string cad)
    {
        message.text = cad;
    }

    public void SetDialogAnswers(string[] cad)
    {
        for (int i = 0; i < 2; i++)
        {
            answers[i].text = cad[i];
        }
    }

    public void HighlightAnswer(int index)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (i == index)
            {
                answers[i].color = Color.white;
            }
            else
            {
                answers[i].color = Color.grey;
            }
        }
    }

    public void SetAvatarImage(Sprite sprite)
    {
        avatarImage.sprite = sprite;
    }

    #endregion

    #region Objetives

    public void ShowObjetives()
    {
        objetiveObject.alpha = 1;
    }

    public void HideObjetives()
    {
        objetiveObject.alpha = 0;
    }

    public void SetObjetiveText(string text)
    {
        objetiveText.text = text;
    }

    public string GetObjetiveText()
    {
        return objetiveText.text;
    }

    #endregion

    #region PAUSE

    public void ShowPauseIndicator()
    {
        if (pauseIndicator)
            pauseIndicator.alpha = 1;
    }

    public void HidePauseIndicator()
    {
        if (pauseIndicator)
            pauseIndicator.alpha = 0;
    }

    public void ShowPausePanel()
    {
        if (ArmorWheel)
            HideArmorWheel();
        if (statsCanvasGroup)
            ShowArcadeStats();

        canvasGroup.blocksRaycasts = true;
        pausePanel.blocksRaycasts = true;
        pausePanel.alpha = 1;
        pausePanel.interactable = true;
        crosshair.alpha = 0;
        _cameraController.SlowCameraSpeed();
        _soundManager.SetMuteVFX(true);
        Time.timeScale = 0;
        CursorManager.ShowCursor();
    }

    public void HidePausePanel()
    {
        if (statsCanvasGroup && _playerValues.GetCurrentInput() != CurrentInput.Upgrade)
            HideArcadeStats();
        canvasGroup.blocksRaycasts = false;
        pausePanel.blocksRaycasts = false;
        pausePanel.alpha = 0;
        pausePanel.interactable = false;
        crosshair.alpha = 1;
        Time.timeScale = 1;
        _soundManager.SetMuteVFX(false);
        _cameraController.ReturnToNormalCameraSpeed();
        if (_playerValues.GetCurrentInput() is CurrentInput.Movement or CurrentInput.Conversation or CurrentInput.None
            or CurrentInput.ArcadeMechanics or CurrentInput.AsteroidMinigame or CurrentInput.RaceMovement
            or CurrentInput.RotatingWall or CurrentInput.ShootMovement or CurrentInput.StealthMovement
            or CurrentInput.DontTouchTheWallsMinigame)
        {
            CursorManager.HideCursor();
        }
    }

    public void FillPauseSlider(float value)
    {
        pauseProgressSlider.value = Mathf.Clamp(value, 0, 1);
    }

    public void SetNextMoveText(string value)
    {
        pauseMoveText.text = value;
    }

    #endregion

    #region crosshair

    public void ShowCrosshair()
    {
        crosshair.alpha = 1;
    }

    public void HideCrossHAir()
    {
        crosshair.alpha = 0;
    }

    public RectTransform GetCrossHairPosition()
    {
        return crossHairRectTransform;
    }

    #endregion

    #region Tutorial

    public void SetTutorial(string cad)
    {
        tutorialText.text = cad;
    }

    public void ShowTutorial()
    {
    }

    public void HideTutorial()
    {
    }

    #endregion

    #region Gears

    public void HighlightGear(int gear)
    {
        if (gear == 0)
        {
            gearImages[0].color = CopyColorChangAlpha(gearImages[0].color, 1);
            for (int j = 1; j < gearImages.Count; j++)
                gearImages[j].color = CopyColorChangAlpha(gearImages[j].color, 0.05f);
        }
        else
        {
            gearImages[0].color = CopyColorChangAlpha(gearImages[0].color, 0.05f);
            for (int i = 1; i < gearImages.Count; i++)
            {
                if (i <= gear)
                    gearImages[i].color = CopyColorChangAlpha(gearImages[i].color, 1);
                else
                    gearImages[i].color = CopyColorChangAlpha(gearImages[i].color, 0.05f);
            }
        }
    }

    private Color CopyColorChangAlpha(Color inColor, float alpha)
    {
        return new Color(inColor.r, inColor.g, inColor.b, alpha);
    }

    #endregion

    #region Message

    public void ShowMessage()
    {
        messageCanvasGroup.alpha = 1;
    }

    public void HideMessage()
    {
        messageCanvasGroup.alpha = 0;
    }

    public void SetMessageText_(string text)
    {
        messageText.text = text;
    }

    #endregion

    #region Arcade Stats

    public void ShowArcadeStats()
    {
        NotifyObservers(PlayerActions.ShowArcadeStats);
        statsCanvasGroup.alpha = 1;
    }

    public void HideArcadeStats()
    {
        NotifyObservers(PlayerActions.HideArcadeStats);
        statsCanvasGroup.alpha = 0;
    }

    public void SetArcadeStatsText(string text)
    {
        statsText.text = text;
    }

    #endregion

    #region Emergin points

    public void ShowArcadePoints()
    {
        statsCanvasGroup.alpha = 1;
    }

    public void HideArcadePoints()
    {
        statsCanvasGroup.alpha = 0;
    }

    public void UpdatePointsText(string text)
    {
        pointsText.text = text;
    }

    public void InsertPoints(string text)
    {
        EmergingPoints points = _emergingPoints.GetText();
        points.StartPoints(emerginPointsTransform.position, text);
    }

    public void InsertPoints(string text, Color color)
    {
        EmergingPoints points = _emergingPoints.GetText();
        points.StartPoints(emerginPointsTransform.position, text, color);
    }

    #endregion

    #region Lives

    public void ShowLives()
    {
        livesCanvas.alpha = 1;
        livesCanvas.interactable = false;
        livesCanvas.blocksRaycasts = false;
    }

    public void HideLives()
    {
        livesCanvas.alpha = 0;
        livesCanvas.interactable = false;
        livesCanvas.blocksRaycasts = false;
    }

    public void SetLives(int num)
    {
        livesText.text = num + "";
    }

    #endregion
}