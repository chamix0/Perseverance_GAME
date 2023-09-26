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
        _tutorial = "Introduce the access code";

    private const string EndMessage = "WELL DONE!";

    //values


    //game variables

    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private GenericScreenUi _genericScreenUi;
    private LockerBase _lockerBase;
    [SerializeField] private Button checkButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text statusText;
    private MinigameSoundManager minigameSoundManager;

    //variables
    private float _targetValue;
    private float _targetValueSlider;
    private float _tX;
    private bool _updateValue;
    private int[] _code = { 0, 0, 0, 0 };

    //lists
    private int index = 0;
    [SerializeField] private Color initColor, highlightColor;
    [SerializeField] private List<Button> upButtons;
    [SerializeField] private List<Button> downButtons;
    [SerializeField] private List<TMP_Text> digits;


    //shader names


    void Start()
    {
        minigameSoundManager = FindObjectOfType<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();

        for (int i = 0; i < upButtons.Count; i++)
        {
            int aux = i;
            upButtons[i].onClick.AddListener(delegate
            {
                IncreaseValue(aux);
                KeyboardInputs();
            });
            downButtons[i].onClick.AddListener(delegate
            {
                DecreaseValue(aux);
                KeyboardInputs();
            });
            digits[i].text = "" + _code[i];
        }

        cubeTutorial.SetActive(false);
        keyTutorial.SetActive(false);
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

    private void KeyboardInputs()
    {
        ShowKeyTutorial();
        UnHighlighlightNumber();
    }
    

    public void SelectNextNumber()
    {
        index = (index + 1) % digits.Count;
        HighlightNumber();
    }

    public void SelectPrevNumber()
    {
        index = index - 1 < 0 ? digits.Count - 1 : index - 1;
        HighlightNumber();
    }

    private void HighlightNumber()
    {
        for (var i = 0; i < digits.Count; i++)
        {
            if (i == index)
                digits[i].color = highlightColor;
            else
                digits[i].color = initColor;
        }
    }

    private void UnHighlighlightNumber()
    {
        foreach (var digit in digits)
        {
            digit.color = initColor;
        }
    }

    public void CheckButton()
    {
        if (checkSol())
        {
            //que ponga correct 
            statusText.text = "CORRECT CODE";
            statusText.color = Color.green;
            EndMinigame();
            minigameSoundManager.PlayFinishedSound();
        }
        else
        {
            minigameSoundManager.PlayInCorrectSound();
            statusText.color = Color.red;
            statusText.text = "INCORRECT CODE";
        }
    }

    public void ExitButton()
    {
        minigameSoundManager.PlayClickSound();
        _lockerBase.ExitBase();
        HideUI();
        CursorManager.HideCursor();
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
        CursorManager.ShowCursor();
    }

    public void ShowUI()
    {
        uiObject.SetActive(true);
        ShowKeyTutorial();
    }

    public void HideUI()
    {
        uiObject.SetActive(false);
        cubeTutorial.SetActive(false);
        keyTutorial.SetActive(false);
    }

    public void IncreaseValue(int index)
    {
        minigameSoundManager.PlayClickSound();
        int oldVal = _code[index];
        _code[index] = oldVal + 1 > 9 ? 0 : oldVal + 1;
        digits[index].text = "" + _code[index];
    }

    public void DecreaseValue(int index)
    {
        minigameSoundManager.PlayClickSound();
        int oldVal = _code[index];
        _code[index] = oldVal - 1 < 0 ? 9 : oldVal - 1;
        digits[index].text = "" + _code[index];
    }

    public void IncreaseValueCube()
    {
        IncreaseValue(index);
        HighlightNumber();
    }

    public void DecreaseValueCube()
    {
        DecreaseValue(index);
        HighlightNumber();
    }

    [SerializeField] private GameObject cubeTutorial, keyTutorial;

    public void ShowCubeTutorial()
    {
        if (!cubeTutorial.activeSelf)
            cubeTutorial.SetActive(true);
        if (keyTutorial.activeSelf)
            keyTutorial.SetActive(false);
    }

    public void ShowKeyTutorial()
    {
        if (cubeTutorial.activeSelf)
            cubeTutorial.SetActive(false);
        if (!keyTutorial.activeSelf)
            keyTutorial.SetActive(true);
    }

    private void EndMinigame()
    {
        StartCoroutine(EndMinigameCoroutine());
    }

    IEnumerator EndMinigameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        HideUI();
        _playerValues.SetInputsEnabled(false);
        _lockerBase.EndLocker();
        _cameraChanger.SetOrbitCamera();
        _lockerBase.ExitBase();
    }

    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name, 55);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        //enseñar tutorial del minijuego
        _genericScreenUi.SetText(_tutorial, 20);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(4f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(1f);
        ShowUI();
        _playerValues.SetInputsEnabled(true);
    }
}