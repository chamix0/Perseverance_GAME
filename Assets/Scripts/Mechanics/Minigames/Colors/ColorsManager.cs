using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(2)]
public class ColorsManager : Minigame
{
    //game variables
    private bool isCorrect;
    private const int NUM_ROUNDS = 5;
    private int correctCount = 0;

    //text to show on screen before the game
    private readonly string _name = "colors",
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.",
        endMessage = "WELL DONE!";

    //components
    [SerializeField] private GameObject uiObject;
    private GameObject buttons;
    private GameObject template;
    [SerializeField] private Shader shader;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;

    private MinigameSoundManager minigameSoundManager;
    //variables

    private Color targetColor;
    private Image targetImage;

    //lists
    private List<Button> _buttons;
    private List<Color> _colors, randomColors;
    private List<Color> buttonColors;

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
    }

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        minigameSoundManager = GetComponent<MinigameSoundManager>();
        buttons = uiObject.transform.Find("Buttons").gameObject;
        template = uiObject.transform.Find("color template").gameObject;
        _buttons.AddRange(buttons.GetComponentsInChildren<Button>());
        targetImage = template.GetComponentInChildren<Image>();
        for (int i = 0; i < _buttons.Count; i++)
        {
            Material material = new Material(shader);
            _buttons[i].GetComponent<Image>().material = material;
        }

        Material targetMaterial = new Material(shader);
        targetImage.material = targetMaterial;
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


    private void Select(Color color)
    {
        if (color.Equals(targetColor))
        {
            correctCount++;
            ChangeTargetColor();
            minigameSoundManager.PlayCorrectSound();
            if (correctCount >= NUM_ROUNDS)
                EndMinigame();
        }
        else
        {
            minigameSoundManager.PlayInCorrectSound();
            correctCount = 0;
        }

        _minigameManager.UpdateCounter(correctCount);
    }


    public void MoveFace(Move move)
    {
        if (move.color.Equals(targetColor))
        {
            correctCount++;
            ChangeTargetColor();
            minigameSoundManager.PlayCorrectSound();
            if (correctCount >= NUM_ROUNDS)
                EndMinigame();
        }
        else
        {
            minigameSoundManager.PlayInCorrectSound();
            correctCount = 0;
        }

        _minigameManager.UpdateCounter(correctCount);
    }


    private void ChangeTargetColor()
    {
        Color oldColor = targetColor;
        do
        {
            int ranColorTarget = Random.Range(0, _colors.Count);
            targetColor = _colors[ranColorTarget];
        } while (oldColor == targetColor);

        targetImage.material.SetColor(BackgroundColor, targetColor);
    }

    public override void StartMinigame()
    {
        correctCount = 0;
        DistributeColors();
        ChangeTargetColor();
        _minigameManager.UpdateCounter(0);
        ShowUI();
        HideGameUi();
        _playerValues.SetCurrentInput(CurrentInput.ColorsMinigame);
        _playerValues.SetInputsEnabled(true);
        StartCoroutine(StartGameCoroutine());
    }

    private void EndMinigame()
    {
        minigameSoundManager.PlayFinishedSound();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        HideGameUi();
        HideUI();
        StartCoroutine(_minigameManager.EndMinigame());
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
        _genericScreenUi.SetText(_name, 50);
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
        ShowGameUi();
        _playerValues.NotifyAction(PlayerActions.ColorsMinigame);
    }
    
}