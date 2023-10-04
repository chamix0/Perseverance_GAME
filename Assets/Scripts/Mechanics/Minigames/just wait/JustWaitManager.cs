using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UTILS;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(3)]
public class JustWaitManager : Minigame
{
    //text to show on screen before the game
    private readonly string _name = "Just Wait",
        _tutorial = "Wait to the timer to end.";


    //values


    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private MinigameSoundManager soundManager;
    private MyStopWatch _timer;
    [SerializeField] private Slider timeSlider;

    //variables
    private int _maxTime;
    private bool minigameActive = false;


    //lists
    private void Awake()
    {
        _timer = gameObject.AddComponent<MyStopWatch>();
    }

    void Start()
    {
        soundManager = GetComponent<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }


    void Update()
    {
        if (minigameActive)
        {
            UpdateTimeSlider();
            CheckSol();
        }
    }


    private void UpdateTimeSlider()
    {
        float aux = (float)(_maxTime - _timer.GetElapsedSeconds()) / _maxTime;
        timeSlider.value = aux;
    }

    private void CheckSol()
    {
        if (_timer.GetElapsedSeconds() >= _maxTime)
        {
            EndMinigame();
            _timer.Stop();
        }
    }


    public override void StartMinigame()
    {
        minigameActive = true;
        _maxTime = Random.Range(10, 20);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(StartGameCoroutine());
    }

    public override void ShowUI()
    {
        uiObject.SetActive(true);
    }

    public override void HideUI()
    {
        uiObject.SetActive(false);
    }


    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        minigameActive = false;
        _timer.Stop();
        _timer.ResetStopwatch();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        HideUI();
        StartCoroutine(_minigameManager.EndMinigame());
    }

    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name, 35);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        //enseñar tutorial del minijuego
        _genericScreenUi.SetText(_tutorial, 10);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(4f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(1f);
        ShowUI();
        //empezar minijuego
        _timer.Restart();
        _playerValues.NotifyAction(PlayerActions.JustWaitMinigame);
    }
}