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
    }


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
        if (checkSol())
            EndMinigame();
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
        if (checkSol())
            EndMinigame();
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
        if (checkSol())
            EndMinigame();
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
        if (checkSol())
            EndMinigame();
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

    private void SetActiveButtons(bool val)
    {
        foreach (var button in buttons)
        {
            button.interactable = val;
        }
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

    public override void ShowUI()
    {
        uiObject.SetActive(true);
    }

    public override void HideUI()
    {
        uiObject.SetActive(false);
    }

    #region Tutorial

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


    #endregion
  
    public override void StartMinigame()
    {
        SetSolution();
        _playerValues.SetCurrentInput(CurrentInput.AdjustValuesMinigame);
        _playerValues.SetInputsEnabled(false);
        _minigameManager.UpdateCounter(0);
        StartCoroutine(StartGameCoroutine());
    }

    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        SetActiveButtons(false);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
        //i hided this because i want to add a delay for the player to notice he has won
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
        SetActiveButtons(true);


        //empezar minijuego
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        HideUI();
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
        // puzzleColor = null;
    }
}