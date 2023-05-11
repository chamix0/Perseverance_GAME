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
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.";

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

    //variables
    private float _targetValue;
    private float _targetValueSlider;
    private float _tX;
    private bool _updateValue;

    //lists
    [SerializeField] private List<Slider> _sliders;


    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
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
        foreach (var slider in _sliders)
        {
            sum += slider.value;
        }

        float diff = Mathf.Abs(_targetValue - sum);
        if (diff < 0.01f)
        {
            _minigameManager.UpdateCounter(5);
            EndMinigame();
        }
        else if (diff < 0.20f)
            _minigameManager.UpdateCounter(4);
        else if (diff < 0.40f)
            _minigameManager.UpdateCounter(3);
        else if (diff < 0.60f)
            _minigameManager.UpdateCounter(2);
        else if (diff < 0.85f)
            _minigameManager.UpdateCounter(1);
        else
            _minigameManager.UpdateCounter(0);
    }

    public override void StartMinigame()
    {
        foreach (var slider in _sliders)
            slider.value = 0;

        _targetValue = Random.Range(1f, 3f);
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

    public void IncreaseValue()
    {
        _tX = 0f;
        _updateValue = true;
        _targetValueSlider = Mathf.Min(1, _sliders[_sliderIndex].value + 0.1f);
    }

    public void DecreaseValue()
    {
        _tX = 0f;
        _updateValue = true;
        _targetValueSlider = Mathf.Max(0, _sliders[_sliderIndex].value - 0.1f);
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
        _minigameManager.UpdateCounter(0);
        HideUI();
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name,10);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        //enseñar tutorial del minijuego
        _genericScreenUi.SetText(_tutorial,10);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(4f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(1f);
        ShowUI();
        _playerValues.SetInputsEnabled(true);


        //empezar minijuego
    }

    IEnumerator EndGameCoroutine()
    {
        _genericScreenUi.SetText(EndMessage,10);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        _cameraChanger.SetOrbitCamera();
        yield return new WaitForSeconds(2f);
        _playerValues.StandUp(true, 3);
    }
}