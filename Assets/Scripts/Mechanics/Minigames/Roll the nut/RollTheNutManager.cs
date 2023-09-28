using System;
using System.Collections;
using Mechanics.General_Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(3)]
public class RollTheNutManager : Minigame
{
    public GraphicRaycaster mRaycaster;
    PointerEventData _mPointerEventData;

    EventSystem _mEventSystem;

    //text to show on screen before the game
    private readonly string _name = "Roll the nut",
        _tutorial = "Roll the nut clockwise with the mouse.";

    private const string endMessage = "WELL DONE!";

    //values
    private int _maxTurns = 5;

    //variables
    private int _turnCount;
    private float _prevVal = 0;

    //game variables
    private bool _isEnabled;


    //components
    [SerializeField] private GameObject uiObject;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private GameObject _nut;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private float _targetAngle;
    private bool _updateAngle;
    private MinigameSoundManager soundManager;
    [SerializeField] private float speed;
    private readonly float angleStep = 90;

    //variables
    private Vector3 mPrevPosition, mPosDelta;

    void Start()
    {
        //Fetch the Event System from the Scene
        _mEventSystem = FindObjectOfType<EventSystem>();
        soundManager = GetComponent<MinigameSoundManager>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _nut = uiObject.transform.Find("nut").gameObject;
        _minigameManager.UpdateCounter(_turnCount);
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }


    void Update()
    {
        //mouse inputs
        if (_isEnabled && Input.GetKey(KeyCode.Mouse0))
        {
            mPosDelta = Input.mousePosition - mPrevPosition;
            float angle = MyUtils.Clamp0360(-_nut.transform.localRotation.eulerAngles.z);
            float dir = mPosDelta.x;

            if (dir > 0)
            {
                _nut.transform.localRotation =
                    Quaternion.Euler(0, 0, _nut.transform.rotation.eulerAngles.z - speed * Math.Abs(dir));
            }
            else if (dir < 0)
            {
                if (!(_turnCount < 0))
                {
                    _nut.transform.localRotation =
                        Quaternion.Euler(0, 0, _nut.transform.rotation.eulerAngles.z + speed * Math.Abs(dir));
                }
            }

            if (angle < 100 && _prevVal > 260)
            {
                soundManager.PlayCorrectSound();
                _turnCount = Mathf.Max(0, _turnCount + 1);
                _minigameManager.UpdateCounter(_turnCount);
                if (_turnCount >= _maxTurns)
                    EndMinigame();
            }
            else if (_prevVal < 100 && angle > 260)
            {
                _turnCount--;
                soundManager.PlayInCorrectSound();
                _minigameManager.UpdateCounter(_turnCount);
            }


            _prevVal = angle;
        }

        mPrevPosition = Input.mousePosition;
//cube inputs
        if (_updateAngle)
        {
            UpdateSnapAngle();
        }
    }

    private void UpdateSnapAngle()
    {
        _nut.transform.localRotation =
            Quaternion.Slerp(_nut.transform.localRotation, Quaternion.Euler(0, 0, MyUtils.Clamp0360(_targetAngle)),
                Time.unscaledDeltaTime * 5f);

        if (Mathf.Abs(_nut.transform.localRotation.eulerAngles.z - MyUtils.Clamp0360(_targetAngle)) < 0.01f)
        {
            _updateAngle = false;
        }
    }

    public void RollCounterClockWise()
    {
        _targetAngle = RoundValue(MyUtils.Clamp0360(_nut.transform.localRotation.eulerAngles.z + angleStep));
        if (_targetAngle == 90f)
        {
            if (_turnCount > 0)
            {
                _turnCount--;
                soundManager.PlayInCorrectSound();
                _minigameManager.UpdateCounter(_turnCount);
                _updateAngle = true;
            }
            else
            {
                _updateAngle = false;
                _targetAngle = RoundValue(_nut.transform.localRotation.eulerAngles.z);
            }
        }
    }

    public void RollClockWise()
    {
        _targetAngle = RoundValue(MyUtils.Clamp0360(_nut.transform.localRotation.eulerAngles.z - angleStep));

        if (_targetAngle == 0)
        {
            _turnCount++;
            soundManager.PlayCorrectSound();
            _minigameManager.UpdateCounter(_turnCount);
            if (_turnCount >= _maxTurns)
                EndMinigame();
        }

        _updateAngle = true;
    }

    float RoundValue(float input)
    {
        return Mathf.Round(input / angleStep) * angleStep;
    }

    public override void StartMinigame()
    {
        _maxTurns = 5;
        _turnCount = 0;
        _playerValues.SetCurrentInput(CurrentInput.RollTheNutMinigame);
        _playerValues.SetInputsEnabled(false);
        _minigameManager.UpdateCounter(0);
        StartCoroutine(StartGameCoroutine());
        //cursor
        CursorManager.ShowCursor();
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

    [SerializeField] private GameObject cubeTutorial,
        keyTutorial;

    public void ShowCubeTutorial()
    {
        if (_isEnabled)
        {
            if (!cubeTutorial.activeSelf)
                cubeTutorial.SetActive(true);
            if (keyTutorial.activeSelf)
                keyTutorial.SetActive(false);
        }
    }

    public void ShowKeyTutorial()
    {
        if (_isEnabled)
        {
            if (cubeTutorial.activeSelf)
                cubeTutorial.SetActive(false);
            if (!keyTutorial.activeSelf)
                keyTutorial.SetActive(true);
        }
    }

    private void EndMinigame()
    {
        soundManager.PlayFinishedSound();
        _isEnabled = false;
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        HideUI();
        StartCoroutine(_minigameManager.EndMinigame());
        //cursor
        CursorManager.HideCursor();
    }

    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name, 40);
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
        _isEnabled = true;
        _playerValues.SetInputsEnabled(true);
        ShowKeyTutorial();
        _playerValues.NotifyAction(PlayerActions.RollMinigame);
    }
}