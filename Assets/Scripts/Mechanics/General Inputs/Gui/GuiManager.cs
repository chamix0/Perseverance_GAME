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
    [Header("last move")] [SerializeField] private Image lastMoveImage;
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
    [Header("Dialog")] [SerializeField] private GameObject dialogObject;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text message;
    [SerializeField] private List<TMP_Text> answers;
    [SerializeField] private Image avatarImage;

    //objetives
    [Header("Dialog")] [SerializeField] private GameObject objetiveObject;
    [SerializeField] private TMP_Text objetiveText;


    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        canvasGroup = GetComponent<CanvasGroup>();
        showCenterReference = GetComponentInChildren<ShowCenterReference>();
        lastMoveText.text = "";
        lastMoveImage.enabled = false;
        machinegunImage.sprite = null;
        machinegunImage.color = Color.clear;
        machinegunText.text = "";
        raceImage.color = Color.clear;
        raceText.text = "";
        HideDialog();
        HideObjetives();
    }

    public void ShowGui()
    {
        canvasGroup.alpha = 1;
    }

    public void HideGui()
    {
        canvasGroup.alpha = 0;
    }

    public void SetLastMoveText(Move move)
    {
        lastMoveImage.enabled = true;
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

    public void SetRaceTime(string time)
    {
        raceImage.color = Color.white;
        raceText.text = time;
    }

    public void DisableRace()
    {
        raceImage.color = Color.clear;
        raceText.text = "";
    }

    private void ShowCenters()
    {
        showCenterReference.ShowColors();
    }

    #region Dialog

    public void ShowDialog()
    {
        dialogObject.SetActive(true);
    }

    public void HideDialog()
    {
        dialogObject.SetActive(false);
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
        objetiveObject.SetActive(true);
    }

    public void HideObjetives()
    {
        objetiveObject.SetActive(false);
    }

    public void SetObjetiveText(string text)
    {
        objetiveText.text = text;
    }

    #endregion
}