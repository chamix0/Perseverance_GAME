using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(3)]
public class MemoryMingameManager : Minigame
{
    private enum Colors
    {
        Blue,
        Green,
        Red,
        Orange,
        White,
        Yellow
    }


    //text to show on screen before the game
    private readonly string _name = "Memorize the sequence",
        _tutorial =
            "Click the colors in order of appearance \n or \n turn the corresponding face with the corresponding color in order of appearance.",
        endMessage = "WELL DONE!";

    //components
    [SerializeField] private GameObject uiObject;
    private GameObject buttons;
    private GameObject template;
    [SerializeField] private Shader shader;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private PlayerAnimations _playerAnimations;
    private GenericScreenUi _genericScreenUi;
    private MinigameManager _minigameManager;

    //Values
    private const int NUM_ROUNDS = 5;

    //variables
    private float textAlpha, targetAlpha, _tA;
    private Image targetImage;
    private int correctSequenceCount;
    private int correctCount;
    private bool isCorrect;
    private int numColorSequence;

    //lists
    private List<Button> _buttons;
    private List<Color> randomColors;
    private List<Color> buttonColors;
    private List<Color> Sequence;
    private List<Color> _colors;


    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    private void Awake()
    {
        buttonColors = new List<Color>();
        _colors = new List<Color>(new[]
            { Color.blue, Color.green, Color.red, new Color(1f, 0.59f, 0.18f), Color.white, Color.yellow, });
        randomColors = new List<Color>(new[]
            { Color.blue, Color.green, Color.red, new Color(1f, 0.59f, 0.18f), Color.white, Color.yellow, });
        _buttons = new List<Button>();
        Sequence = new List<Color>();
    }

    void Start()
    {
        numColorSequence = 1;
        correctCount = 0;
        correctSequenceCount = 0;
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        buttons = uiObject.transform.Find("Buttons").gameObject;
        template = uiObject.transform.Find("color template").gameObject;
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _minigameManager.UpdateCounter(correctCount);
        _buttons.AddRange(buttons.GetComponentsInChildren<Button>());
        targetImage = template.GetComponentInChildren<Image>();

        foreach (var t in _buttons)
        {
            Material material = new Material(shader);
            t.GetComponent<Image>().material = material;
        }

        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }

    private void DistributeColors()
    {
        randomColors.Clear();
        randomColors.AddRange(_colors.ToArray());
        buttonColors.Clear();
        for (var i = 0; i < _buttons.Count; i++)
        {
            var ranColor = Random.Range(0, randomColors.Count - 1);
            _buttons[i].GetComponent<Image>().material.SetColor(BackgroundColor, randomColors[ranColor]);
            buttonColors.Add(randomColors[ranColor]);
            randomColors.RemoveAt(ranColor);
            int aux = i;
            _buttons[i].onClick.RemoveAllListeners();
            _buttons[i].onClick.AddListener(() => Select(buttonColors[aux]));
        }
    }


    public void Select(Color inputColor)
    {
        targetImage.material.SetColor(BackgroundColor, inputColor);
        if (inputColor.Equals(Sequence[correctSequenceCount]))
        {
            correctSequenceCount++;
            if (correctSequenceCount >= numColorSequence)
            {
                numColorSequence++;
                correctSequenceCount = 0;
                correctCount++;
                if (correctCount >= NUM_ROUNDS)
                    EndMinigame();
                ChangeSequence();
            }
        }
        else
        {
            numColorSequence = 1;
            correctCount = 0;
            correctSequenceCount = 0;
            ChangeSequence();
        }

        _minigameManager.UpdateCounter(correctCount);
    }


    // Update is called once per frame

    private void ChangeSequence()
    {
        Sequence.Clear();
        Color oldColor = Color.clear;
        Color actualColor = Color.clear;
        for (int i = 0; i < numColorSequence; i++)
        {
            do
            {
                int ranColorTarget = Random.Range(0, _colors.Count - 1);
                actualColor = _colors[ranColorTarget];
            } while (actualColor == oldColor);

            oldColor = actualColor;
            Sequence.Add(actualColor);
        }

        StartCoroutine(ShowSequenceCoroutine());
    }

    private void SetInteractableButtons(bool value)
    {
        foreach (var button in _buttons)
            button.interactable = value;
        _playerValues.SetInputsEnabled(value);
    }


    private IEnumerator ShowSequenceCoroutine()
    {
        SetInteractableButtons(false);
        yield return new WaitForSeconds(1f);
        targetImage.material.SetColor(BackgroundColor, Color.black);
        yield return new WaitForSeconds(1f);
        foreach (var color in Sequence)
        {
            targetImage.material.SetColor(BackgroundColor, color);
            yield return new WaitForSeconds(1f);
        }

        targetImage.material.SetColor(BackgroundColor, Color.black);
        SetInteractableButtons(true);
    }

    public override void StartMinigame()
    {
        targetImage.material.SetColor(BackgroundColor, Color.black);
        correctCount = 0;
        numColorSequence = 1;
        correctSequenceCount = 0;
        DistributeColors();
        ShowUI();
        HideGameUi();
        _playerValues.SetCurrentInput(CurrentInput.MemoryMinigame);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(StartGameCoroutine());
    }

    private void EndMinigame()
    {
        HideGameUi();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
    }

    public override void ShowUI()
    {
        uiObject.SetActive(true);
        _minigameManager.SetCounterVisivility(false);
    }

    public override void HideUI()
    {
        _minigameManager.SetCounterVisivility(false);
        uiObject.SetActive(false);
    }


    private void ShowGameUi()
    {
        buttons.SetActive(true);
        template.SetActive(true);
        _minigameManager.SetCounterVisivility(true);
    }

    private void HideGameUi()
    {
        buttons.SetActive(false);
        template.SetActive(false);
        _minigameManager.SetCounterVisivility(false);
    }


    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        //enseñar tutorial del minijuego
        _genericScreenUi.SetText(_tutorial);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(4f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(1f);
        ShowGameUi();
        ChangeSequence();
    }

    IEnumerator EndGameCoroutine()
    {
        _genericScreenUi.SetText(endMessage);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        _cameraChanger.SetOrbitCamera();
        yield return new WaitForSeconds(2f);
        _playerValues.StandUp( true, 3f);
    }
}