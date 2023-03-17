using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
[DefaultExecutionOrder(3)]
public class RollTheNutManager : Minigame
{
    public GraphicRaycaster mRaycaster;
    PointerEventData _mPointerEventData;

    EventSystem _mEventSystem;

    //text to show on screen before the game
    private readonly string _name = "Roll the nut",
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.";

    private const string endMessage = "WELL DONE!";

    //values
    private int _maxTurns = 5;

    //variables
    private int _turnCount;
    private float _prevVal = 0;
    private float _prevOffset;
    private bool _canTurn;

    //game variables
    private bool _isEnabled;


    //components
    [SerializeField] private GameObject uiObject;
    [SerializeField] private Shader shader;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private GameObject _nut;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    private float _targetAngle;
    private bool _updateAngle;

    private readonly float angleStep = 90;
    //variables
    
    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    void Start()
    {
        //Fetch the Event System from the Scene
        _mEventSystem = FindObjectOfType<EventSystem>();

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
            //Set up the new Pointer Event
            _mPointerEventData = new PointerEventData(_mEventSystem);
            //Set the Pointer Event Position to that of the mouse position
            _mPointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            mRaycaster.Raycast(_mPointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "nut")
                {
                    Vector3 point = result.worldPosition - result.gameObject.transform.position;
                    float h = point.magnitude;
                    float ca = point.y;
                    float aux;
                    aux = Mathf.Acos(ca / h);
                    if (point.x < 0)
                        aux *= -1;
                    //turn 1 full time clockwise
                    float actualOffset = aux * Mathf.Rad2Deg;
                    float offset = actualOffset - _prevOffset;

                    if (offset < 0)
                        _canTurn = true;

                    if (Mathf.Abs(offset) < 10f)
                    {
                        if (_canTurn)
                            result.gameObject.transform.localRotation = Quaternion.Euler(0, 0, offset + _prevVal);
                    }


                    _prevOffset = actualOffset;
                    float actualVal = result.gameObject.transform.localRotation.eulerAngles.z;
                    if (_prevVal == 0)
                    {
                        result.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 359);
                    }
                    else if (offset < 0 && _prevVal > 0 && _prevVal < 20 && actualVal < 360 && actualVal > 340)
                    {
                        _turnCount++;
                        _minigameManager.UpdateCounter(_turnCount);
                        _canTurn = true;
                        if (_turnCount >= _maxTurns)
                            EndMinigame();
                    }
                    else if (offset >= 0)
                    {
                        _canTurn = false;
                    }

                    print(offset);
                    _prevVal = actualVal;
                }
            }
        }

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
                Time.deltaTime * 5f);

        if (Mathf.Abs(_nut.transform.localRotation.eulerAngles.z - MyUtils.Clamp0360(_targetAngle)) < 0.01f)
        {
            _updateAngle = false;
        }
    }

    public void RollCounterClockWise()
    {
        print("FASDFASDFASDFA");
        _targetAngle = RoundValue(MyUtils.Clamp0360(_nut.transform.localRotation.eulerAngles.z + angleStep));
        if (_targetAngle == 90f)
        {
            if (_turnCount > 0)
            {
                _turnCount--;
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
            _minigameManager.UpdateCounter(_turnCount);
            _canTurn = true;
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

    private void EndMinigame()
    {
        _isEnabled = false;
        HideUI();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
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
        ShowUI();
        _isEnabled = true;
        _playerValues.SetInputsEnabled(true);


        //empezar minijuego
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
        _playerValues.StandUp(true, 3);
    }
}