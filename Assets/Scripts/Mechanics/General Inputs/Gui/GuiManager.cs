using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CanvasGroup canvasGroup;
    private ShowCenterReference showCenterReference;

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

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        canvasGroup = GetComponent<CanvasGroup>();
        showCenterReference = GetComponentInChildren<ShowCenterReference>();
        lastMoveText.text = "";
        lastMoveObject.alpha = 0;
        machinegunImage.sprite = null;
        machinegunImage.color = Color.clear;
        machinegunText.text = "";
        raceImage.color = Color.clear;
        raceText.text = "";

        HideDialog();
        HideObjetives();
        HidePauseIndicator();
        HidePausePanel();
    }

    private void Update()
    {
        //gears
        if (_playerValues.GetGear() != currentGear)
        {
            currentGear = _playerValues.GetGear();
            HighlightGear(currentGear);
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
        if (index == -1)
        {
            machinegunImage.color = Color.clear;
            machinegunText.text = "";
            return;
        }

        machinegunImage.color = Color.white;
        machinegunImage.sprite = shootingModeImages[index];
        machinegunText.text = shootingModeTexts[index];
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

    #region Dialog

    public void ShowDialog()
    {
        dialogObject.alpha = 1;
    }

    public void HideDialog()
    {
        dialogObject.alpha = 0;
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

    #endregion

    #region PAUSE

    public void ShowPauseIndicator()
    {
        pauseIndicator.alpha = 1;
    }

    public void HidePauseIndicator()
    {
        pauseIndicator.alpha = 0;
    }

    public void ShowPausePanel()
    {
        canvasGroup.blocksRaycasts = true;
        pausePanel.blocksRaycasts = true;
        pausePanel.alpha = 1;
        pausePanel.interactable = true;
    }

    public void HidePausePanel()
    {
        canvasGroup.blocksRaycasts = false;
        pausePanel.blocksRaycasts = false;
        pausePanel.alpha = 0;
        pausePanel.interactable = false;
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
}