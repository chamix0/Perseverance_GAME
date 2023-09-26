using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerFightAction
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

struct MoveElement
{
    public RectTransform pos;
    public bool value;
    public MoveSequencePiece piece;

    public MoveElement(RectTransform r, bool b, MoveSequencePiece p)
    {
        pos = r;
        value = b;
        piece = p;
    }
}

public class MiniBossManager : MonoBehaviour
{
    //text to show on screen before the game
    private string _name = "Boss",
        _tutorial = "Defeat the boss.",
        endMessage = "WELL DONE!";

    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private GenericScreenUi _genericScreenUi;
    [SerializeField] private Image bossImage;
    [SerializeField] private Image livesImage;
    private GameObject bossScreen;
    private GameObject gameScreen;
    private GameObject fightScreen;
    private MiniBossBase _miniBossBase;
    private MinigameSoundManager soundManager;

    //timer
    private Stopwatch _timer;
    int _bossTurnTime;
    int _gameMaxTime;


    //values
    private const int NumPieces = 15;
    public ScreenPhase _phase = ScreenPhase.Boss;
    private int lives = 3;
    private float _bossHealth, _bossMaxHealth;
    public bool usingCube;

    //list
    [SerializeField] private List<Sprite> livesSprites;


    #region BOSS PHASE STUFF

    //values
    private PlayerFightAction selectedAction = PlayerFightAction.Attack;
    [SerializeField] private BossAction _bossAction;

    //variables

    //texts
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text bossIntentionText;
    [SerializeField] private TMP_Text bossTimeText;

    //lists
    private List<Image> _buttonsImages;

    //buttons
    [SerializeField] private Button defendButton;
    [SerializeField] private Button specialDefendButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button specialAttackButton;
    [SerializeField] private Shader _shader;
    [SerializeField] private Color _colorHighlighted;
    private Color _normalColor;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    #endregion

    #region GAME PHASE STUFF

    //values
    int _maxSequence = 100;
    private int _normalSequenceLength;
    private int _specialSequenceLength;

    private int difficulty;

    //variables
    private int _sequenceValueIndex;
    private int _inCorrectCount;
    private int _correctCount;
    private int _currentCombo;
    private int _maxCombo;

    //lists
    private List<SequenceValue> _sequenceValues;
    private Queue<int> _piecesIndex;
    private List<RectTransform> _positions;
    private List<MoveSequencePiece> _pieces;

    //texts
    [SerializeField] private TMP_Text gameTimeText;
    [SerializeField] private TMP_Text correctCountText;
    [SerializeField] private TMP_Text inCorrectCountText;
    [SerializeField] private TMP_Text comboCountText;
    [SerializeField] private TMP_Text maxComboCountText;

    //components
    [SerializeField] private GameObject pieceContainer;
    [SerializeField] private GameObject pieceTemplate;
    [SerializeField] private GameObject positionsContainer;

    #endregion

    #region FIGHT PHASE STUFF

    //sliders
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private Slider bossHealthRecoverySlider;
    private float _targetHealthValue;
    private float _tH, _tRh;
    private bool updateSliders;

    //animator
    [SerializeField] private Animator _fightAnimator;

    //text
    [SerializeField] private TMP_Text actionEfficiencyText;

    //sprites
    [SerializeField] private Image bossImageFight;
    [SerializeField] private Image playerImageFight;
    [SerializeField] private Image bossActionImage;
    [SerializeField] private Image playerActionImage;

    [SerializeField]
    private List<Sprite> actionSprites; //0 - attack 1 - deffend 2 - special attack 3 - special deffense

    private static readonly int Fight1 = Animator.StringToHash("fight");
    private static readonly int Action1 = Animator.StringToHash("action");

    #endregion


    private void Awake()
    {
        _positions = new List<RectTransform>();
        _buttonsImages = new List<Image>();
        _piecesIndex = new Queue<int>();
        for (int i = NumPieces - 1; i >= 0; i--)
        {
            _piecesIndex.Enqueue(i);
        }

        _pieces = new List<MoveSequencePiece>();
        _timer = new Stopwatch();
        _sequenceValues = new List<SequenceValue>();
    }

    void Start()
    {
        bossScreen = uiObject.transform.transform.Find("boss screen").gameObject;
        gameScreen = uiObject.transform.transform.Find("game screen").gameObject;
        fightScreen = uiObject.transform.transform.Find("fight screen").gameObject;

        InitializePositions();

        defendButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.Defend); });
        specialDefendButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.SpecialDefense); });
        attackButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.Attack); });
        specialAttackButton.onClick.AddListener(delegate { SelectAction(PlayerFightAction.SpecialAttack); });

        InitializeButtons();

        _normalColor = _buttonsImages[0].material.GetColor(BackgroundColor);
        _playerValues = FindObjectOfType<PlayerValues>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _genericScreenUi.SetTextAlpha(0);
        soundManager = FindObjectOfType<MinigameSoundManager>();
        SetLivesSprite();
        HideUI();
    }

    private void Update()
    {
        if (_phase == ScreenPhase.Boss)
        {
            if (_timer.Elapsed.TotalSeconds > _bossTurnTime)
                EndBossTurn();
            else
                bossTimeText.text = "" + (int)(_bossTurnTime - _timer.Elapsed.TotalSeconds);
        }
        else if (_phase == ScreenPhase.Game)
        {
            if (_timer.Elapsed.TotalSeconds > _gameMaxTime)
                EndGameTurn();
            else
                gameTimeText.text = "" + (int)(_gameMaxTime - _timer.Elapsed.TotalSeconds);
            correctCountText.text = "" + _correctCount + " / " + _maxSequence;
            inCorrectCountText.text = "" + _inCorrectCount;
            comboCountText.text = "" + _currentCombo;
            maxComboCountText.text = "" + _maxCombo;
        }
        else
        {
            if (updateSliders)
                SmoothSetbossHealthbar();
        }
    }

    #region SHOW/HIDE UI

    private void ShowBossScreen()
    {
        ShowUI();
        bossScreen.SetActive(true);
    }

    private void HideBossScreen()
    {
        bossScreen.SetActive(false);
    }

    private void ShowGameScreen()
    {
        gameScreen.SetActive(true);
    }

    private void HideGameScreen()
    {
        gameScreen.SetActive(false);
    }

    private void ShowFightScreen()
    {
        fightScreen.SetActive(true);
    }

    private void HideFightScreen()
    {
        fightScreen.SetActive(false);
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

    #region Boss Phase

    private void EnterBossPhase()
    {
        _phase = ScreenPhase.Boss;
        ShowBossScreen();
        HideGameScreen();
        HideFightScreen();
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
            bossActionImage.sprite = actionSprites[0];
            bossIntentionText.text = bossNameText.text + " is going to defend.";
        }
        else if (action == 1)
        {
            _bossAction = BossAction.Attack;
            bossActionImage.sprite = actionSprites[2];
            bossIntentionText.text = bossNameText.text + " is going to attack.";
        }
        else
        {
            _bossAction = BossAction.Nothing;
            bossActionImage.sprite = null;
            bossIntentionText.text = bossNameText.text + " is going to do nothing.";
        }
    }

    public void SelectAction(PlayerFightAction action)
    {
        selectedAction = action;
        playerActionImage.sprite = actionSprites[(int)selectedAction];
        HighlightButton();
    }

    private void HighlightButton()
    {
        soundManager.PlayClickSound();
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
        _correctCount = 0;
        _currentCombo = 0;
        _maxCombo = 0;
        _inCorrectCount = 0;

        _phase = ScreenPhase.Game;

        HideBossScreen();
        HideFightScreen();
        ShowGameScreen();

        if (selectedAction is PlayerFightAction.SpecialAttack or PlayerFightAction.SpecialAttack)
            _maxSequence = _specialSequenceLength;
        else
            _maxSequence = _normalSequenceLength;

        CreateSequence(difficulty, _maxSequence);

        if (_pieces.Count == 0)
            CreatePieces();
        else
        {
            SetAlphaPieces();
            UpdatePiecesValues();
        }

        _timer.Start();
    }

    private void EndGameTurn()
    {
        _timer.Stop();
        _timer.Reset();
        EnterFightPhase();
    }

    private void CreatePieces()
    {
        for (int i = 0; i < NumPieces; i++)
        {
            GameObject pieceObj = Instantiate(pieceTemplate, pieceContainer.transform);
            MoveSequencePiece piece = pieceObj.GetComponent<MoveSequencePiece>();
            _pieces.Add(piece);
            int aux = i;

            StartCoroutine(MovePieceCoroutine(piece, aux));
        }

        SetAlphaPieces();
        UpdatePiecesValues();
    }

    IEnumerator MovePieceCoroutine(MoveSequencePiece piece, int index)
    {
        yield return new WaitForSeconds(0f);
        piece.MoveAction(_positions[index], false);
    }

    public void ChangeInputToCube()
    {
        if (!usingCube)
        {
            usingCube = true;
            UpdatePiecesValues();
        }
    }

    public void ChangeInputToKey()
    {
        if (usingCube)
        {
            usingCube = false;
            UpdatePiecesValues();
        }
    }

    private void UpdatePiecesValues()
    {
        int aux = _sequenceValueIndex + NumPieces - 5;
        int[] auxIndexes = _piecesIndex.ToArray();

        for (int i = 0; i < NumPieces; i++)
        {
            int index = aux - i;
            if (i <= NumPieces - 5)
            {
                if (usingCube)
                    _pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetCubeValue().ToString());
                else
                    _pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetKeyValue());
            }
            else
            {
                if (index >= 0)
                    if (usingCube)
                        _pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetCubeValue().ToString());
                    else
                        _pieces[auxIndexes[^(i + 1)]].SetText(_sequenceValues[index].GetKeyValue());
                else
                    _pieces[auxIndexes[^(i + 1)]].SetText("");
            }
        }
    }

    private void UpdateNextPiece()
    {
        int[] auxIndexes = _piecesIndex.ToArray();
        int aux = _sequenceValueIndex + NumPieces - 5;
        if (aux < _maxSequence)
        {
            if (usingCube)
                _pieces[auxIndexes[^1]].SetText(_sequenceValues[aux].GetCubeValue().ToString());
            else
                _pieces[auxIndexes[^1]].SetText(_sequenceValues[aux].GetKeyValue());
        }
        else _pieces[auxIndexes[^1]].SetText("");
    }

    public void NextPiece()
    {
        int[] auxIndexes = _piecesIndex.ToArray();
        for (int i = 0; i < NumPieces; i++)
        {
            if (i < NumPieces - 1)
                _pieces[auxIndexes[^(i + 1)]].MoveAction(_positions[i + 1], false);
            else
                _pieces[auxIndexes[^(i + 1)]].MoveAction(_positions[0], true);
        }

        int aux = _piecesIndex.Dequeue();
        _piecesIndex.Enqueue(aux);
        UpdateNextPiece();
        SetAlphaPieces();
    }

    private void CreateSequence(int dif, int max)
    {
        _sequenceValues.Clear();
        _sequenceValueIndex = 0;
        for (int i = 0; i < max; i++)
        {
            _sequenceValues.Add(new SequenceValue(dif));
        }
    }

    private void SetAlphaPieces()
    {
        int[] auxIndexes = _piecesIndex.ToArray();
        for (int i = 0; i < NumPieces; i++)
        {
            if (i >= NumPieces - 3) _pieces[auxIndexes[^(i + 1)]].SetAlpha(0);
            else if (i == NumPieces - 4) _pieces[auxIndexes[^(i + 1)]].SetAlpha(0.2f);
            else if (i >= NumPieces - 7 && i < NumPieces - 4) _pieces[auxIndexes[^(i + 1)]].SetAlpha(1);
            else if (i >= NumPieces - 8) _pieces[auxIndexes[^(i + 1)]].SetAlpha(0.5f);
            else
            {
                _pieces[auxIndexes[^(i + 1)]].SetColor(Color.black);
                _pieces[auxIndexes[^(i + 1)]].SetAlpha(0);
            }
        }
    }

    public void ProcessInput(string letter)
    {
        int[] auxIndexes = _piecesIndex.ToArray();
        if (_sequenceValueIndex < _maxSequence)
        {
            if (letter.ToLower() == _sequenceValues[_sequenceValueIndex].GetKeyValue().ToLower())
            {
                _correctCount++;
                _sequenceValueIndex++;
                _currentCombo++;
                if (_currentCombo > _maxCombo)
                    _maxCombo = _currentCombo;
                _pieces[auxIndexes[^(NumPieces - 4)]].SetColor(Color.green);
                //play correct sound
                soundManager.PlayTapSound();
            }
            else
            {
                _currentCombo = 0;
                _inCorrectCount++;
                _sequenceValueIndex++;
                _pieces[auxIndexes[^(NumPieces - 4)]].SetColor(Color.red);
                //play incorrect sound
                soundManager.PlayInCorrectSound();
            }

            if (usingCube)
                ChangeInputToKey();
            NextPiece();
        }
    }

    public void ProcessInput(Move move)
    {
        int[] auxIndexes = _piecesIndex.ToArray();
        if (_sequenceValueIndex < _maxSequence)
        {
            if (move.Equals(_sequenceValues[_sequenceValueIndex].GetCubeValue()))
            {
                _correctCount++;
                _sequenceValueIndex++;
                _currentCombo++;
                if (_currentCombo > _maxCombo)
                    _maxCombo = _currentCombo;
                //set the leter in green
                _pieces[auxIndexes[^(NumPieces - 4)]].SetColor(Color.green);
                //play correct sound
                soundManager.PlayTapSound();
            }
            else
            {
                _currentCombo = 0;
                _inCorrectCount++;
                _sequenceValueIndex++;
                //sett the letter in red
                _pieces[auxIndexes[^(NumPieces - 4)]].SetColor(Color.red);
                //play incorrect sound
                soundManager.PlayInCorrectSound();
            }

            if (!usingCube)
                ChangeInputToCube();
            NextPiece();
        }
    }

    #endregion

    #region fight phase

    private void EnterFightPhase()
    {
        _phase = ScreenPhase.Fight;
        HideGameScreen();
        ShowFightScreen();
        Fight();
    }

    private void Fight()
    {
        float efficiency = CalculateEfficiency();
        actionEfficiencyText.text = efficiency + "%";
        bool actionSuccesful = efficiency > 50;
        float maxAttackDamage = _bossMaxHealth / 4;
        float attackDamage = (efficiency / 100) * maxAttackDamage;
        switch (selectedAction)
        {
            case PlayerFightAction.Attack:
                switch (_bossAction)
                {
                    case BossAction.Attack:
                        StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage));
                        _bossHealth = Mathf.Max(0, _bossHealth - attackDamage);
                        lives--;
                        PlayAnimation(5);
                        soundManager.PlayHitSound();
                        StartCoroutine(livesCoroutine(4.5f));
                        break;
                    case BossAction.Defend:
                        if (actionSuccesful)
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage));
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage);
                            PlayAnimation(0);
                            soundManager.PlayDefendSound();
                        }
                        else
                        {
                            PlayAnimation(1);
                        }

                        break;
                    case BossAction.Nothing:
                        StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage));
                        _bossHealth = Mathf.Max(0, _bossHealth - attackDamage);
                        PlayAnimation(0);
                        break;
                }

                break;
            case PlayerFightAction.Defend:
                switch (_bossAction)
                {
                    case BossAction.Attack:
                        if (actionSuccesful)
                            PlayAnimation(2);
                        else
                        {
                            lives--;
                            PlayAnimation(3);
                            StartCoroutine(livesCoroutine(3));
                        }

                        break;
                }

                break;
            case PlayerFightAction.SpecialAttack:
                switch (_bossAction)
                {
                    case BossAction.Attack:
                        if (actionSuccesful)
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, maxAttackDamage * 2));
                            _bossHealth = Mathf.Max(0, _bossHealth - maxAttackDamage * 2);
                        }
                        else
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage * 2));
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage * 2);
                        }

                        lives--;
                        PlayAnimation(5);
                        StartCoroutine(livesCoroutine(4.5f));
                        break;
                    case BossAction.Defend:
                        if (actionSuccesful)
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, maxAttackDamage * 2));
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage * 2);
                            PlayAnimation(0);
                        }
                        else
                            PlayAnimation(1);


                        break;
                    case BossAction.Nothing:
                        if (actionSuccesful)
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, maxAttackDamage * 2));
                            _bossHealth = Mathf.Max(0, _bossHealth - maxAttackDamage * 2);
                        }
                        else
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage * 2));
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage * 2);
                        }

                        PlayAnimation(0);
                        break;
                }

                break;
            case PlayerFightAction.SpecialDefense:
                switch (_bossAction)
                {
                    case BossAction.Attack:
                        if (actionSuccesful)
                        {
                            PlayAnimation(4);
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage));
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage);
                        }
                        else
                        {
                            lives--;
                            PlayAnimation(3);
                            StartCoroutine(livesCoroutine(3));
                        }

                        break;
                    case BossAction.Defend:
                        if (actionSuccesful)
                        {
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage));
                            PlayAnimation(0);
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage);
                        }

                        break;
                    case BossAction.Nothing:
                        if (actionSuccesful)
                        {
                            PlayAnimation(0);
                            StartCoroutine(HealthBarCoroutine(4, _bossHealth, attackDamage));
                            _bossHealth = Mathf.Max(0, _bossHealth - attackDamage);
                        }

                        break;
                }

                break;
        }

        StartCoroutine(EndFightTurn());
    }

    IEnumerator EndFightTurn()
    {
        yield return new WaitForSeconds(10);
        if (lives <= 0)
            ExitMinigame();
        else if (_bossHealth <= 0)
            EndMinigame();
        else
            EnterBossPhase();
    }

    private void PlayAnimation(int index)
    {
        _fightAnimator.SetInteger(Fight1, index);
        _fightAnimator.SetTrigger(Action1);
    }

    IEnumerator HealthBarCoroutine(float time, float bossHealth, float healthAmount)
    {
        yield return new WaitForSeconds(time);
        SetHealthBar(bossHealth, healthAmount);
    }

    IEnumerator livesCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        RemoveLive();
    }

    private void SetHealthBar(float bossHealth, float healthAmount)
    {
        _targetHealthValue = (bossHealth - healthAmount) / _bossMaxHealth;
        updateSliders = true;
        _tH = 0f;
        _tRh = 0f;
    }

    private void SmoothSetbossHealthbar()
    {
        bossHealthSlider.value = Mathf.Lerp(bossHealthSlider.value, _targetHealthValue, _tH);
        bossHealthRecoverySlider.value = Mathf.Lerp(bossHealthRecoverySlider.value, _targetHealthValue, _tRh);
        _tH += 0.5f * Time.deltaTime;
        _tRh += 0.01f * Time.deltaTime;
        if (_tH > 1.0f && _tRh > 1.0f)
        {
            _tH = 1.0f;
            _tRh = 1.0f;
            updateSliders = false;
        }
    }

    private float CalculateEfficiency()
    {
        float correct = _correctCount;
        float incorrect = _inCorrectCount * 0.5f;
        float maxCombo = _maxCombo;
        float sequenceLength = _maxSequence;

        float comboMultiplier = 1 + (maxCombo / sequenceLength) * 0.1f;
        float overallPuntuation = Mathf.Max(0, (correct - incorrect) * comboMultiplier);
        return (overallPuntuation / sequenceLength) * 100;
    }

    private void RemoveLive()
    {
        SetLivesSprite();
    }

    private void SetLivesSprite()
    {
        livesImage.sprite = livesSprites[lives];
    }

    #endregion

    public void StartMinigame(string bossName, Sprite bossSprite, int bossTime, int gameTime, int sequenceLength,
        int difficul, float bossHealth, MiniBossBase miniBossBase)
    {
        //set partameters
        _miniBossBase = miniBossBase;
        _bossTurnTime = bossTime;
        _gameMaxTime = gameTime;
        _maxSequence = sequenceLength;
        _normalSequenceLength = sequenceLength;
        _specialSequenceLength = sequenceLength * 2;
        _name = bossName;
        bossNameText.text = bossName;
        bossImage.sprite = bossImageFight.sprite = bossSprite;
        lives = 3;
        difficulty = difficul;
        _bossHealth = _bossMaxHealth = bossHealth;

        //set inputs
        _playerValues.SetCurrentInput(CurrentInput.MiniBoss);
        _playerValues.SetInputsEnabled(true);
        StartCoroutine(StartGameCoroutine());

        //cursor
        CursorManager.ShowCursor();
    }

    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        StopMinigame();
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
    }

    private void ExitMinigame()
    {
        StopMinigame();
        _playerValues.SetInputsEnabled(false);
        soundManager.PlayInCorrectSound();
        StartCoroutine(ExitMinigameCoroutine());
        //cursor
        CursorManager.HideCursor();
    }

    private void StopMinigame()
    {
        _timer.Stop();
        HideUI();
    }

    IEnumerator ExitMinigameCoroutine()
    {
        _genericScreenUi.SetText("You lost...", 23);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        _miniBossBase.ExitBase();
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
        bossHealthSlider.value = 1;
        bossHealthRecoverySlider.value = 1;
        StartCoroutine(livesCoroutine(0));
        EnterBossPhase(); // StartRound();
    }

    IEnumerator EndGameCoroutine()
    {
        _genericScreenUi.SetText(endMessage, 32);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        _miniBossBase.EndFight();
    }

    #region Initialization

    private void InitializePositions()
    {
        for (int i = 1; i <= NumPieces; i++)
        {
            _positions.Add(positionsContainer.transform.transform.Find("pos (" + i + ")").gameObject
                .GetComponent<RectTransform>());
        }
    }

    private void InitializeButtons()
    {
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
    }

    #endregion
}