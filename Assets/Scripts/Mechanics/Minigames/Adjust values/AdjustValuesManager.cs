using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

[DefaultExecutionOrder(3)]
public class AdjustValuesManager : Minigame
{
    //text to show on screen before the game
    private readonly string _name = "Adjust the values",
        _tutorial = "Adjust the sliders to get the right values";

    private const string EndMessage = "WELL DONE!";

    //values


    //game variables

    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private int _sliderIndex = 0;
    private MinigameSoundManager soundManager;

    //variables
    private float[] _targetValuees;
    private float _targetValueSlider;
    private float _tX;
    private bool _updateValue;

    private bool minigameStarted;

    //lists
    private List<Slider> _sliders;


    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        soundManager = GetComponent<MinigameSoundManager>();
        _sliders = new List<Slider>();
        _sliders.AddRange(uiObject.GetComponentsInChildren<Slider>());
        foreach (var slider in _sliders)
        {
            slider.onValueChanged.AddListener(delegate(float arg0) { OnValueChanged(); });
        }

        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }


    void Update()
    {
        if (_updateValue)
            UpdatesliderValue();
    }


    // float RoundValue(float input)
    // {
    //     return Mathf.Round(input / angleStep) * angleStep;
    // }
    private void OnValueChanged()
    {
        checkSol();
    }

    public void SelectNextSlider()
    {
        _sliderIndex = (_sliderIndex + 1) % _sliders.Count;
        HighlightSlider();
        print(_sliderIndex);
        _updateValue = false;
    }

    public void SelectPrevSlider()
    {
        _sliderIndex = _sliderIndex - 1 < 0 ? _sliders.Count - 1 : _sliderIndex - 1;
        HighlightSlider();
        print(_sliderIndex);
        _updateValue = false;
    }

    private void HighlightSlider()
    {
        for (var i = 0; i < _sliders.Count; i++)
        {
            var block = _sliders[i].colors;
            if (i == _sliderIndex)
                block.normalColor = Color.green;
            else
                block.normalColor = Color.white;
            _sliders[i].colors = block;
        }
    }

    private void checkSol()
    {
        float sum = 0;
        for (int i = 0; i < _sliders.Count; i++)
        {
            float value = _sliders[i].value;
            float diff = Mathf.Abs(value - _targetValuees[i]);
            if (diff < 0.1f)
            {
                sum += 1;
            }
            else if (diff < 0.3f)
            {
                sum += 0.5f;
            }
        }


        int valueC = (int)(sum * 5 / 3);
        if (valueC >= 5)
        {
            _minigameManager.UpdateCounter(5);
            EndMinigame();
        }
        else
            _minigameManager.UpdateCounter(valueC);
    }

    public override void StartMinigame()
    {
        foreach (var slider in _sliders)
            slider.value = 0;
        _targetValuees = new float[] { Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f) };
        _playerValues.SetCurrentInput(CurrentInput.AdjustValuesMinigame);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(StartGameCoroutine());
    }

    public override void ShowUI()
    {
        _minigameManager.SetCounterVisivility(true);
        uiObject.SetActive(true);
    }

    public override void HideUI()
    {
        _minigameManager.SetCounterVisivility(false);
        uiObject.SetActive(false);
    }

    [SerializeField] private GameObject cubeTutorial, keyTutorial;

    public void ShowCubeTutorial()
    {
        if (minigameStarted)
        {
            if (!cubeTutorial.activeSelf)
                cubeTutorial.SetActive(true);
            if (keyTutorial.activeSelf)
                keyTutorial.SetActive(false);
        }
    }

    public void ShowKeyTutorial()
    {
        if (minigameStarted)
        {
            if (cubeTutorial.activeSelf)
                cubeTutorial.SetActive(false);
            if (!keyTutorial.activeSelf)
                keyTutorial.SetActive(true);
        }
    }

    public void IncreaseValue()
    {
        _tX = 0f;
        _updateValue = true;
        _targetValueSlider = Mathf.Min(1, _sliders[_sliderIndex].value + 0.05f);
    }

    public void DecreaseValue()
    {
        _tX = 0f;
        _updateValue = true;
        _targetValueSlider = Mathf.Max(0, _sliders[_sliderIndex].value - 0.05f);
    }

    private void UpdatesliderValue()
    {
        _sliders[_sliderIndex].value = Mathf.Lerp(_sliders[_sliderIndex].value, _targetValueSlider, _tX);
        _tX += 0.5f * Time.deltaTime;
        if (_tX > 1.0f)
        {
            _tX = 1.0f;
            _updateValue = false;
        }
    }

    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        _minigameManager.UpdateCounter(0);
        HideUI();
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
        _playerValues.SetInputsEnabled(true);
        minigameStarted = true;
        ShowKeyTutorial();


        //empezar minijuego
    }

    IEnumerator EndGameCoroutine()
    {
        minigameStarted = false;
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