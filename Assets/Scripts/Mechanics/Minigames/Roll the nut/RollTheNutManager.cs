using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RollTheNutManager : Minigame
{
    public GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;

    EventSystem m_EventSystem;

    //text to show on screen before the game
    private readonly string _name = "Roll the nut",
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.",
        endMessage = "WELL DONE!";

    //values
    private int maxTurns = 5;

    //variables
    private int turnCount;
    private float prevVal = 0;
    private float prevOffset;
    private bool canTurn;

    //game variables
    private bool isEnabled;
    private const int NUM_ROUNDS = 5;


    //components
    [SerializeField] private GameObject uiObject;
    [SerializeField] private Shader shader;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private PlayerAnimations _playerAnimations;
    private Image nut;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;

    //variables


    //lists
    private List<Image> counterImages;

    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    void Start()
    {
        //Fetch the Event System from the Scene
        m_EventSystem = FindObjectOfType<EventSystem>();

        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        nut = uiObject.transform.Find("nut").gameObject.GetComponent<Image>();
        _minigameManager.UpdateCounter(turnCount);
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }


    void Update()
    {
        //Check if the left Mouse button is clicked
        if (isEnabled && Input.GetKey(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

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
                    if (point.x > 0)
                        aux *= -1;
                    //turn 1 full time clockwise
                    float actualOffset = aux * Mathf.Rad2Deg;
                    float offset = actualOffset - prevOffset;

                    if (offset < 0)
                        canTurn = true;

                    if (Mathf.Abs(offset) < 10f)
                    {
                        if (canTurn)
                            result.gameObject.transform.localRotation = Quaternion.Euler(0, 0, offset + prevVal);
                    }


                    prevOffset = actualOffset;
                    float actualVal = result.gameObject.transform.localRotation.eulerAngles.z;
                    if (prevVal == 0)
                    {
                        result.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 359);
                    }
                    else if (offset < 0 && prevVal > 0 && prevVal < 20 && actualVal < 360 && actualVal > 340)
                    {
                        turnCount++;
                        _minigameManager.UpdateCounter(turnCount);
                        canTurn = true;
                        if (turnCount >= maxTurns)
                            EndMinigame();
                    }
                    else if (offset >= 0)
                    {
                        canTurn = false;
                    }

                    print(offset);
                    prevVal = actualVal;
                }
            }
        }
    }

    public override void StartMinigame()
    {
        maxTurns = 5;
        turnCount = 0;
        _playerValues.SetCurrentInput(CurrentInput.Roll_The_Nut_Minigame);
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
        isEnabled = false;
        HideUI();
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
        isEnabled = true;
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
        _playerAnimations.SetSitAnim(false);
        yield return new WaitForSeconds(1f);
        _playerValues.SetCurrentInput(CurrentInput.Movement);
        _playerValues.SetInputsEnabled(true);
        _playerValues.SetCanMove(true);
    }
}