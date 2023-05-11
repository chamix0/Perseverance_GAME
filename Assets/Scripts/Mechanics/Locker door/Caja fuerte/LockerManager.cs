using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockerManager : MonoBehaviour
{
    //text to show on screen before the game
    private readonly string _name = "code",
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.";

    private const string EndMessage = "WELL DONE!";

    //values


    //game variables

    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private GenericScreenUi _genericScreenUi;
    private LockerBase _lockerBase;
    private int _sliderIndex = 0;
    [SerializeField] private Button checkButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text statusText;


    //variables
    private float _targetValue;
    private float _targetValueSlider;
    private float _tX;
    private bool _updateValue;
    private int[] _code = { 0, 0, 0, 0 };

    //lists
    [SerializeField] private List<Button> upButtons;
    [SerializeField] private List<Button> downButtons;
    [SerializeField] private List<TMP_Text> digits;


    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();

        for (int i = 0; i < upButtons.Count; i++)
        {
            int aux = i;
            upButtons[i].onClick.AddListener(delegate { IncreaseValue(aux); });
            downButtons[i].onClick.AddListener(delegate { DecreaseValue(aux); });
            digits[i].text = "" + _code[i];
        }

        checkButton.onClick.AddListener(CheckButton);
        exitButton.onClick.AddListener(ExitButton);
        statusText.text = "";
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }

    public void SetLockerBase(LockerBase compo)
    {
        _lockerBase = compo;
    }


    // float RoundValue(float input)
    // {
    //     return Mathf.Round(input / angleStep) * angleStep;
    // }

    public void SelectNextNumber()
    {
        // _sliderIndex = (_sliderIndex + 1) % _sliders.Count;
        // HighlightSlider();
        // print(_sliderIndex);
        // _updateValue = false;
    }

    public void SelectPrevNumber()
    {
        // _sliderIndex = _sliderIndex - 1 < 0 ? _sliders.Count - 1 : _sliderIndex - 1;
        // HighlightSlider();
        // print(_sliderIndex);
        // _updateValue = false;
    }

    private void HighlightNumber()
    {
        // for (var i = 0; i < _sliders.Count; i++)
        // {
        //     var block = _sliders[i].colors;
        //     if (i == _sliderIndex)
        //         block.normalColor = Color.green;
        //     else
        //         block.normalColor = Color.white;
        //     _sliders[i].colors = block;
        // }
    }

    private void CheckButton()
    {
        if (checkSol())
        {
            //que ponga correct 
            statusText.text = "CORRECT CODE";
            EndMinigame();
        }
        else
        {
            statusText.text = "INCORRECT CODE";
        }
    }

    private void ExitButton()
    {
        EndMinigame();
    }

    private bool checkSol()
    {
        for (int i = 0; i < _code.Length; i++)
            if (!_lockerBase.CheckGetDigit(i, _code[i]))
                return false;

        return true;
    }

    public void StartMinigame()
    {
        _playerValues.SetCurrentInput(CurrentInput.LockerMinigame);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(StartGameCoroutine());
    }

    public void ShowUI()
    {
        uiObject.SetActive(true);
    }

    public void HideUI()
    {
        uiObject.SetActive(false);
    }

    public void IncreaseValue(int index)
    {
        int oldVal = _code[index];
        _code[index] = oldVal + 1 > 9 ? 0 : oldVal + 1;
        digits[index].text = "" + _code[index];
    }

    public void DecreaseValue(int index)
    {
        int oldVal = _code[index];
        _code[index] = oldVal - 1 < 0 ? 9 : oldVal - 1;
        digits[index].text = "" + _code[index];
    }


    private void EndMinigame()
    {
        HideUI();
        _playerValues.SetInputsEnabled(false);
        _lockerBase.EndLocker();
        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name,55);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        //enseñar tutorial del minijuego
        _genericScreenUi.SetText(_tutorial,20);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(4f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(1f);
        ShowUI();
        _playerValues.SetInputsEnabled(true);
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        _cameraChanger.SetOrbitCamera();
        yield return new WaitForSeconds(2f);
        _lockerBase.ExitBase();
    }
}