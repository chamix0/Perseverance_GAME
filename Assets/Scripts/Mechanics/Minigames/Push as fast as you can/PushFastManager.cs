using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(3)]
public class PushFastManager : Minigame
{
    //text to show on screen before the game
    private readonly string _name = "Push fast",
        _tutorial = "Push as fast as you can \n or \n turn any face as fast as you can.";

    private const string EndMessage = "WELL DONE!";

    //values


    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private MinigameSoundManager soundManager;

    [SerializeField] private Button button;
    private Stopwatch _timer;
    [SerializeField] private Slider timeSlider;

    //variables
    private int _clickCount;
    private int _targetClicks;
    private int _maxTime;
    private bool minigameActive = false;


    //lists


    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    void Start()
    {
        soundManager = GetComponent<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _timer = new Stopwatch();
        button.onClick.AddListener(Click);
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }


    void Update()
    {
        if (minigameActive)
        {
            if (_timer.Elapsed.TotalSeconds > _maxTime)
            {
                ResetMinigame();
            }

            UpdateTimeSlider();
        }
    }

    public void Click()
    {
        _clickCount++;
        soundManager.PlayClickSound();
        _minigameManager.UpdateCounter(CounterVal());
        CheckSol();
    }


    private void UpdateTimeSlider()
    {
        float aux = (float)(_maxTime - _timer.Elapsed.TotalSeconds) / _maxTime;
        timeSlider.value = aux;
    }

    private void CheckSol()
    {
        if (_clickCount >= _targetClicks)
            EndMinigame();
    }

    private void ResetMinigame()
    {
        _timer.Restart();
        _clickCount = 0;
        _minigameManager.UpdateCounter(0);
    }

    private int CounterVal()
    {
        float aux = (float)_clickCount / _targetClicks;
        return (int)(aux * 5);
    }

    public override void StartMinigame()
    {
        minigameActive = true;
        _clickCount = 0;
        _targetClicks = Random.Range(75, 101);
        _maxTime = Random.Range(20, 40);
        _playerValues.SetCurrentInput(CurrentInput.ClickFastMinigame);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(StartGameCoroutine());
    }

    public override void ShowUI()
    {
        _minigameManager.SetCounterVisivility(true);
        uiObject.SetActive(true);
        ShowKeyTutorial();
    }

    public override void HideUI()
    {
        _minigameManager.SetCounterVisivility(false);
        uiObject.SetActive(false);
    }

    [SerializeField] private GameObject cubeTutorial, keyTutorial;

    public void ShowCubeTutorial()
    {
        if (minigameActive)
        {
            if (!cubeTutorial.activeSelf)
                cubeTutorial.SetActive(true);
            if (keyTutorial.activeSelf)
                keyTutorial.SetActive(false);
        }
    }

    public void ShowKeyTutorial()
    {
        if (minigameActive)
        {
            if (cubeTutorial.activeSelf)
                cubeTutorial.SetActive(false);
            if (!keyTutorial.activeSelf)
                keyTutorial.SetActive(true);
        }
    }

    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        minigameActive = false;
        _timer.Stop();
        HideUI();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name, 10);
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
        _playerValues.SetInputsEnabled(true);
        _timer.Start();
        //empezar minijuego
        _timer.Restart();
    }

    IEnumerator EndGameCoroutine()
    {
        _genericScreenUi.SetText(EndMessage, 10);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        _cameraChanger.SetOrbitCamera();
        yield return new WaitForSeconds(2f);
        _playerValues.StandUp(true, 3);
    }
}