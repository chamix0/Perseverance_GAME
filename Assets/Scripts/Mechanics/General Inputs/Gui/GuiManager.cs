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
            machinegunImage = null;
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
}