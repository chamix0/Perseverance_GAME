using System;
using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(14)]
public class BeeFight : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider slider;
    [SerializeField] private List<DoorManager> doorManagers;
    [SerializeField] private DoorManager optionalEnterDoor;
    [SerializeField] private GameObject conversation;
    private PlayerValues playerValues;
    [SerializeField] private EnemyShooterZone enemyShooterZone;
    private bool fightStarted;
    private float sliderVal = 0;


    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        playerValues.AddObserver(this);
        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float newVal = enemyShooterZone.GetLiveValue();
        if (Math.Abs(sliderVal - newVal) > 0)
        {
            sliderVal = enemyShooterZone.GetLiveValue();
            slider.value = newVal;
        }

        if (fightStarted && enemyShooterZone.AllEnemiesDead())
        {
            fightStarted = false;
            EndFight();
        }
    }

    public void StartBattle()
    {
        fightStarted = true;
        canvasGroup.alpha = 1;
        enemyShooterZone.AssignInitialPositions();
    }

    public void EndFight()
    {
        canvasGroup.alpha = 0;
        foreach (var doorManager in doorManagers)
        {
            doorManager.OpenDoor();
        }
    }

    public void ResetFight()
    {
        canvasGroup.alpha = 0;
        enemyShooterZone.HideAll();
        conversation.SetActive(true);
        if (optionalEnterDoor)
        {
            optionalEnterDoor.OpenDoor();
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die)
        {
            ResetFight();
        }
    }
}