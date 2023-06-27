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
    [SerializeField] private GameObject conversation;
    private PlayerValues playerValues;
    [SerializeField] private EnemyShooterZone enemyShooterZone;
    private bool fightEnded;
    private float sliderVal = 0;


    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        playerValues.AddObserver(this);
        canvasGroup.alpha = 0;

        StartCoroutine(WaitForFightToEnd());
    }

    // Update is called once per frame
    void Update()
    {
        float newVal = enemyShooterZone.GetLiveValue();
        if (Math.Abs(sliderVal - newVal) > 0.01)
        {
            sliderVal = enemyShooterZone.GetLiveValue();
            slider.value = newVal;
        }
    }

    public void StartBattle()
    {
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
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Die)
        {
            ResetFight();
        }
    }

    IEnumerator WaitForFightToEnd()
    {
        yield return new WaitUntil(enemyShooterZone.AllEnemiesDead);
        EndFight();
    }
}