using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

enum PlayerFightAction
{
    Defend = 0,
    SpecialDefense = 1,
    Attack = 2,
    SpecialAttack = 3
}

enum BossAction
{
    Defend = 0,
    Attack = 1,
    Nothing = 2
}

public enum ScreenPhase
{
    Boss,
    Game,
    Fight
}

public class MiniBossManager : MonoBehaviour
{
    //text to show on screen before the game
    private string _name = "Boss",
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.",
        endMessage = "WELL DONE!";

    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private GenericScreenUi _genericScreenUi;
    private CameraChanger _cameraChanger;
    [SerializeField] private GameObject pieceContainer;
    [SerializeField] private GameObject pieceTemplate;
    [SerializeField] private GameObject positionsContainer;
    private GameObject bossScreen;
    private GameObject gameScreen;

    //texts
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text bossIntentionText;
    [SerializeField] private TMP_Text bossTimeText;
    [SerializeField] private TMP_Text correctCountText;
    [SerializeField] private TMP_Text inCorrectCountText;
    [SerializeField] private TMP_Text comboCountText;
    [SerializeField] private TMP_Text maxComboCountText;


    //timer
    private Stopwatch _timer;
    public int bossTurnTime;
    public int gameMaxTime;

    //buttons
    [Header("Action buttons")] [SerializeField]
    private Button defendButton;

    [SerializeField] private Button specialDefendButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button specialAttackButton;
    [SerializeField] private Shader _shader;
    [SerializeField] private Color _colorHighlighted;
    private Color _normalColor;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    //values
    private const int NumPieces = 7;
    public ScreenPhase _phase = ScreenPhase.Boss;
    private int lives = 3;
    public bool usingCube;


    //list
    private List<MoveSequencePiece> pieces;
    private List<RectTransform> positions;
    private List<Image> _buttonsImages;
    [SerializeField] private List<Sprite> livesSprites;
    private Queue<int> piecesIndex;

    //boss phase stuff
    private PlayerFightAction selectedAction = PlayerFightAction.Attack;
    private BossAction _bossAction;
    [SerializeField] private Image bossImage;

    //game phase stuff
    public int maxSequence = 100;
    private List<SequenceValue> _sequenceValues;
    private int _sequenceValueIndex;
    private int _inCorrectCount;
    private int _correctCount;
    private int _currentCombo;
    private int _maxCombo;
    private KeyCode kCode; //this stores your custom key


    //fight phase stuff
    [SerializeField] private Image livesImage;


    private void Awake()
    {
        positions = new List<RectTransform>();
        _buttonsImages = new List<Image>();
        piecesIndex = new Queue<int>(new[] { 6, 5, 4, 3, 2, 1, 0 });
        pieces = new List<MoveSequencePiece>();
        _timer = new Stopwatch();
        _sequenceValues = new List<SequenceValue>();
        for (int i = 0; i < maxSequence; i++)
        {
            _sequenceValues.Add(new SequenceValue());
        }
    }

    void Start()
    {
        bossScreen = uiObject.transform.transform.Find("boss screen").gameObject;
        gameScreen = uiObject.transform.transform.Find("game screen").gameObject;

        for (int i = 1; i <= NumPieces; i++)
        {
            positions.Add(positionsContainer.transform.transform.Find("pos (" + i + ")").gameObject
                .GetComponent<RectTransform>());
        }

        defendButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.Defend); });
        specialDefendButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.SpecialDefense); });
        attackButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.Attack); });
        specialAttackButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.SpecialAttack); });
        _buttonsImages.AddRange(new[]
        {
            defendButton.GetComponent<Image>(), specialDefendButton.GetComponent<Image>(),
            attackButton.GetComponent<Image>(), specialAttackButton.GetComponent<Image>()
        });
        foreach (var image in _buttonsImages)
        {
            Material material = new Material(_shader);
            image.material = material;
        }

        _normalColor = _buttonsImages[0].material.GetColor(BackgroundColor);
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _genericScreenUi.SetTextAlpha(0);
        SetLivesSprite();
        HideUI();
    }

    private void Update()
    {
        if (_phase == ScreenPhase.Boss)
        {
            if (_timer.Elapsed.TotalSeconds > bossTurnTime)
                EndBossTurn();
            else
                bossTimeText.text = "" + (int)(bossTurnTime - _timer.Elapsed.TotalSeconds);
        }
        else if (_phase == ScreenPhase.Game)
        {
            correctCountText.text = "" + _correctCount;
            inCorrectCountText.text = "" + _inCorrectCount;
            comboCountText.text = "" + _currentCombo;
            maxComboCountText.text = "" + _maxCombo;
        }
        else
        {
        }
    }

    public void StartMinigame(string bossName, Sprite bossSprite)
    {
        _name = bossName;
        bossNameText.text = bossName;
        bossImage.sprite = bossSprite;
        lives = 3;
        _playerValues.SetCurrentInput(CurrentInput.MiniBoss);
        _playerValues.SetInputsEnabled(true);
        StartCoroutine(StartGameCoroutine());
    }

    private void EndMinigame()
    {
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
    }


    #region SHOW/HIDE UI

    private void ShowBossScreen()
    {
        ShowUI();
        bossScreen.SetActive(true);
        HideGameScreen();
    }

    private void HideBossScreen()
    {
        bossScreen.SetActive(false);
    }

    private void ShowGameScreen()
    {
        ShowUI();
        gameScreen.SetActive(true);
        HideBossScreen();
    }

    private void HideGameScreen()
    {
        gameScreen.SetActive(false);
    }

    public void ShowUI()
    {
        uiObject.SetActive(true);
    }

    public void HideUI()
    {
        uiObject.SetActive(false);
    }

    #endregion

    #region boss phase

    private void EnterBossPhase()
    {
        _phase = ScreenPhase.Boss;
        ShowBossScreen();
        HideGameScreen();
        HighlightButton();
        SetBossAction();
        //start turn timer
        _timer.Start();
    }

    private void EndBossTurn()
    {
        _timer.Stop();
        _timer.Reset();
        EnterGamePhase();
    }

    private void SetBossAction()
    {
        int action = Random.Range(0, 3);
        if (action == 0)
        {
            _bossAction = BossAction.Defend;
            bossIntentionText.text = bossNameText.text + " is going to defend.";
        }
        else if (action == 1)
        {
            _bossAction = BossAction.Attack;
            bossIntentionText.text = bossNameText.text + " is going to attack.";
        }
        else
        {
            _bossAction = BossAction.Nothing;
            bossIntentionText.text = bossNameText.text + " is going to do nothing.";
        }
    }

    private void SelectAction(PlayerFightAction action)
    {
        selectedAction = action;
        HighlightButton();
    }

    private void HighlightButton()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == (int)selectedAction)
            {
                _buttonsImages[i].material.SetColor(BackgroundColor, _colorHighlighted);
            }
            else
            {
                _buttonsImages[i].material.SetColor(BackgroundColor, _normalColor);
            }
        }
    }

    #endregion

    #region game phase

    private void EnterGamePhase()
    {
        _phase = ScreenPhase.Game;
        HideBossScreen();
        ShowGameScreen();
        CreatePieces();
    }

    private void CreatePieces()
    {
        for (int i = 0; i < NumPieces; i++)
        {
            GameObject pieceObj = Instantiate(pieceTemplate, pieceContainer.transform);
            MoveSequencePiece piece = pieceObj.GetComponent<MoveSequencePiece>();
            pieces.Add(piece);
            int aux = i;
            
            StartCoroutine(MovePieceCoroutine(piece, aux));
        }

        SetAlphaPieces();
        UpdatePiecesValues();
    }

    IEnumerator MovePieceCoroutine(MoveSequencePiece piece, int index)
    {
        yield return new WaitForSeconds(0f);
        piece.MoveElement(positions[index], false);
    }

    public void ChangeInputToCube()
    {
        usingCube = true;
        UpdatePiecesValues();
    }

    public void ChangeInputToKey()
    {
        usingCube = false;
        UpdatePiecesValues();
    }

    private void UpdatePiecesValues()
    {
        int aux = _sequenceValueIndex + 4;
        int[] auxIndexes = piecesIndex.ToArray();

        for (int i = 0; i < NumPieces; i++)
        {
            int index = aux - i;
            if (i <= 4)
            {
                if (usingCube)
                    pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetCubeValue().ToString());
                else
                    pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetKeyValue());
            }
            else
            {
                if (index >= 0)
                    if (usingCube)
                        pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetCubeValue().ToString());
                    else
                        pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetKeyValue());
                else
                    pieces[auxIndexes[^(i + 1)]].SetText("");
            }
        }
    }

    private void UpdateNextPiece()
    {
        int[] auxIndexes = piecesIndex.ToArray();
        int aux = _sequenceValueIndex + 4;
        if (usingCube)
            pieces[auxIndexes[^1]].SetText(_sequenceValues[aux].GetCubeValue().ToString());
        else
            pieces[auxIndexes[^1]].SetText(_sequenceValues[aux].GetKeyValue());
    }

    public void NextPiece()
    {
        int[] auxIndexes = piecesIndex.ToArray();
        for (int i = 0; i < NumPieces; i++)
        {
            if (i < NumPieces - 1)
                pieces[auxIndexes[^(i + 1)]].MoveElement(positions[i + 1], false);
            else
                pieces[auxIndexes[^(i + 1)]].MoveElement(positions[0], true);
        }

        int aux = piecesIndex.Dequeue();
        piecesIndex.Enqueue(aux);
        UpdateNextPiece();
        SetAlphaPieces();
    }

    private void SetAlphaPieces()
    {
        int[] auxIndexes = piecesIndex.ToArray();
        for (int i = 0; i < NumPieces; i++)
        {
            if (i == 0)
                pieces[auxIndexes[^(i + 1)]].SetAlpha(0);
            else if (i == 1)
                pieces[auxIndexes[^(i + 1)]].SetAlpha(0.2f);
            else if (i > 1 && i <= 4)
                pieces[auxIndexes[^(i + 1)]].SetAlpha(1);
            else if (i == 5)
                pieces[auxIndexes[^(i + 1)]].SetAlpha(0.5f);
            else
                pieces[auxIndexes[^(i + 1)]].SetAlpha(0);
        }
    }

    public void ProcessInput(string letter)
    {
        if (letter.ToLower() == _sequenceValues[_sequenceValueIndex].GetKeyValue().ToLower())
        {
            _correctCount++;
            _sequenceValueIndex++;
            _currentCombo++;
            if (_currentCombo > _maxCombo)
                _maxCombo = _currentCombo;
        }
        else
        {
            _currentCombo = 0;
            _inCorrectCount++;
            _sequenceValueIndex++;
        }

        if (usingCube)
            ChangeInputToKey();
        NextPiece();
    }

    public void ProcessInput(Move move)
    {
        if (move.Equals(_sequenceValues[_sequenceValueIndex].GetCubeValue()))
        {
            _correctCount++;
            _sequenceValueIndex++;
            _currentCombo++;
            if (_currentCombo > _maxCombo)
                _maxCombo = _currentCombo;
        }
        else
        {
            _currentCombo = 0;
            _inCorrectCount++;
            _sequenceValueIndex++;
        }

        if (!usingCube)
            ChangeInputToCube();
        NextPiece();
    }

    #endregion

    #region fight phase

    private void RemoveLive()
    {
        lives--;
        SetLivesSprite();
    }

    private void SetLivesSprite()
    {
        livesImage.sprite = livesSprites[lives];
    }

    #endregion

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
        EnterBossPhase(); // StartRound();
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
        _playerValues.StandUp(true, 3f);
    }
}