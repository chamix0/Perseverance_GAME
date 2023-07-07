using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(3)]
public class JustWaitManager : Minigame
{
    //text to show on screen before the game
    private readonly string _name = "Just Wait",
        _tutorial = "Wait to the timer to end.";

    private const string EndMessage = "WELL DONE!";

    //values


    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private MinigameSoundManager soundManager;
    private Stopwatch _timer;
    [SerializeField] private Slider timeSlider;

    //variables
    private int _maxTime;
    private bool minigameActive = false;


    //lists


    void Start()
    {
        soundManager = GetComponent<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _timer = new Stopwatch();
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }


    void Update()
    {
        if (_playerValues.GetPaused() && _timer.IsRunning && minigameActive)
        {
            _timer.Stop();
        }
        else if (!_playerValues.GetPaused() && !_timer.IsRunning && minigameActive)
        {
            _timer.Start();
        }

        if (minigameActive)
        {
            UpdateTimeSlider();
            CheckSol();
        }
    }


    private void UpdateTimeSlider()
    {
        float aux = (float)(_maxTime - _timer.Elapsed.TotalSeconds) / _maxTime;
        timeSlider.value = aux;
    }

    private void CheckSol()
    {
        if (_timer.Elapsed.TotalSeconds >= _maxTime)
            EndMinigame();
    }


    public override void StartMinigame()
    {
        minigameActive = true;
        _maxTime = Random.Range(40, 60);
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
        _timer.Reset();
        HideUI();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
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
        _minigameManager.EndMinigame();
    }
}