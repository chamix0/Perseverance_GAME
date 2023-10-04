using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PuzzleManager : Minigame
{
    //text to show on screen before the game
    private readonly string _name = "Match the pattern",
        _tutorial = "use the screen buttons or the cube to match the top pattern";

    private const string EndMessage = "WELL DONE!";


    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private MinigameSoundManager soundManager;

    //variables
    private bool minigameStarted;
    private int correctCount = 0;

    //lists
    [SerializeField] private List<Image> puzzleImages, solutionImages;
    private Color[][] solution, puzzleColor;
    private Color[] usableColors = { Color.green, Color.yellow, Color.red };

    //buttons
    private List<Button> buttons;
    [SerializeField] private Button topRightButton, topLeftButton;
    [SerializeField] private Button botRightButton, botLeftButton;
    [SerializeField] private Button rightUpButton, rightDownButton;
    [SerializeField] private Button leftUpButton, leftDownButton;
    private int verticalIndex, horizontalIndex;
    private List<List<Button>> navegationButtons;

    void Start()
    {
        buttons = new List<Button>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        soundManager = GetComponent<MinigameSoundManager>();
        _genericScreenUi.SetTextAlpha(0);
        SetButtons();
        puzzleColor = new Color[3][];
        solution = new Color[3][];
        HideUI();
        navegationButtons = new List<List<Button>>();
        navegationButtons.Add(new List<Button>(new[] { topLeftButton, topRightButton }));
        navegationButtons.Add(new List<Button>(new[] { leftUpButton, rightUpButton }));
        navegationButtons.Add(new List<Button>(new[] { leftDownButton, rightDownButton }));
        navegationButtons.Add(new List<Button>(new[] { botLeftButton, botRightButton }));
    }

    #region Set Up Puzzle

    private void SetSolution()
    {
        do
        {
            List<Color> usedColors = new List<Color>();
            for (int i = 0; i < 9; i++)
            {
                int index = Random.Range(0, usableColors.Length);
                usedColors.Add(usableColors[index]);
            }


            for (int i = 0; i < 3; i++)
            {
                solution[i] = new Color[3];
                for (int j = 0; j < 3; j++)
                {
                    solution[i][j] = usedColors[3 * i + j];
                    solutionImages[3 * i + j].color = solution[i][j];
                }
            }

            Color centerCol = usedColors[4];
            usedColors.RemoveAt(4);
            for (int i = 0; i < 3; i++)
            {
                puzzleColor[i] = new Color[3];
                for (int j = 0; j < 3; j++)
                {
                    if ((i == j) && i == 1)
                        puzzleColor[i][j] = centerCol;
                    else
                    {
                        int ranIndex = Random.Range(0, usedColors.Count);
                        puzzleColor[i][j] = usedColors[ranIndex];
                        usedColors.RemoveAt(ranIndex);
                    }
                }
            }
        } while (checkSol());


        SetPuzzleColors();
    }

    private void SetPuzzleColors()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                puzzleImages[3 * i + j].color = puzzleColor[i][j];
            }
        }
    }

    private void SetButtons()
    {
        //top row
        topRightButton.onClick.AddListener(() => ShiftTopRow(1));
        topLeftButton.onClick.AddListener(() => ShiftTopRow(-1));

        //bot row
        botRightButton.onClick.AddListener(() => ShiftBotRow(1));
        botLeftButton.onClick.AddListener(() => ShiftBotRow(-1));

        //left column
        leftUpButton.onClick.AddListener(() => ShiftLeftRow(1));
        leftDownButton.onClick.AddListener(() => ShiftLeftRow(-1));

        //right column
        rightUpButton.onClick.AddListener(() => ShiftRightRow(1));
        rightDownButton.onClick.AddListener(() => ShiftRightRow(-1));
        Button[] aux =
        {
            topRightButton, topLeftButton, botLeftButton, botRightButton, leftDownButton, leftUpButton, rightUpButton,
            rightDownButton
        };
        buttons.AddRange(aux);
    }

    #endregion

    #region Puzzle mechanics

    public void ShiftTopRow(int dir)
    {
        Color[] aux = new Color[3];
        Array.Copy(puzzleColor[0], aux, 3);
        if (dir < 0)
        {
            for (int i = 0; i < 3; i++)
                puzzleColor[0][i] = aux[(i + 1) % 3];
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                int index = i - 1 < 0 ? 2 : i - 1;
                puzzleColor[0][i] = aux[index];
            }
        }

        SetPuzzleColors();
        CheckEndMinigame();
    }

    public void ShiftBotRow(int dir)
    {
        Color[] aux = new Color[3];
        Array.Copy(puzzleColor[2], aux, 3);
        if (dir < 0)
        {
            for (int i = 0; i < 3; i++)
                puzzleColor[2][i] = aux[(i + 1) % 3];
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                int index = i - 1 < 0 ? 2 : i - 1;
                puzzleColor[2][i] = aux[index];
            }
        }

        SetPuzzleColors();
        CheckEndMinigame();
    }

    public void ShiftLeftRow(int dir)
    {
        Color[] aux = { puzzleColor[0][0], puzzleColor[1][0], puzzleColor[2][0] };
        if (dir < 0)
        {
            for (int i = 0; i < 3; i++)
            {
                int index = i - 1 < 0 ? 2 : i - 1;
                puzzleColor[i][0] = aux[index];
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
                puzzleColor[i][0] = aux[(i + 1) % 3];
        }

        SetPuzzleColors();
        CheckEndMinigame();
    }

    public void ShiftRightRow(int dir)
    {
        Color[] aux = { puzzleColor[0][2], puzzleColor[1][2], puzzleColor[2][2] };
        if (dir < 0)
        {
            for (int i = 0; i < 3; i++)
            {
                int index = i - 1 < 0 ? 2 : i - 1;
                puzzleColor[i][2] = aux[index];
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
                puzzleColor[i][2] = aux[(i + 1) % 3];
        }

        SetPuzzleColors();
        CheckEndMinigame();
    }

    private bool checkSol()
    {
        soundManager.PlayClickSound();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (puzzleColor[i][j] != solution[i][j])
                    return false;
            }
        }

        return true;
    }

    private void CheckEndMinigame()
    {
        if (checkSol())
        {
            correctCount++;
            _minigameManager.UpdateCounter(correctCount);
            if (correctCount >= 5)
            {
                EndMinigame();
            }
            else
            {
                SetSolution();
                SetPuzzleColors();
            }
        }
    }

    #endregion

    public void Right()
    {
        horizontalIndex = (horizontalIndex + 1) % navegationButtons[verticalIndex].Count;
        HighLightButton();
    }

    public void Left()
    {
        horizontalIndex = horizontalIndex - 1 < 0 ? navegationButtons[verticalIndex].Count - 1 : horizontalIndex - 1;
        HighLightButton();
    }

    public void Up()
    {
        verticalIndex = (verticalIndex + 1) % navegationButtons.Count;
        HighLightButton();
    }

    public void Down()
    {
        verticalIndex = verticalIndex - 1 < 0 ? navegationButtons.Count - 1 : verticalIndex - 1;
        HighLightButton();
    }

    private void HighLightButton()
    {
        for (int i = 0; i < navegationButtons.Count; i++)
        {
            for (int j = 0; j < navegationButtons[i].Count; j++)
            {
                if (i == verticalIndex && j == horizontalIndex)
                    navegationButtons[i][j].image.color = Color.black;
                else
                    navegationButtons[i][j].image.color = Color.white;
            }
        }
    }

    public void SelectButton()
    {
        Button button = navegationButtons[verticalIndex][horizontalIndex];
        if (button.IsInteractable())
        {
            button.onClick.Invoke();
        }
    }

    private void SetActiveButtons(bool val)
    {
        foreach (var button in buttons)
        {
            button.interactable = val;
        }
    }


    public override void ShowUI()
    {
        uiObject.SetActive(true);
        _minigameManager.SetCounterVisivility(true);
    }

    public override void HideUI()
    {
        _minigameManager.SetCounterVisivility(false);
        uiObject.SetActive(false);
    }


    public override void StartMinigame()
    {
        SetSolution();
        _playerValues.SetCurrentInput(CurrentInput.PuzzleMinigame);
        _playerValues.SetInputsEnabled(false);
        _minigameManager.UpdateCounter(0);
        StartCoroutine(StartGameCoroutine());
        //cursor
        CursorManager.ShowCursor();
    }

    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        SetActiveButtons(false);
        _playerValues.SetInputsEnabled(false);
        HideUI();
        StartCoroutine(_minigameManager.EndMinigame());
        //cursor
        CursorManager.HideCursor();
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
        SetActiveButtons(true);
        _playerValues.NotifyAction(PlayerActions.PuzzleMinigame);

        //empezar minijuego
    }
}