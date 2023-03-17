using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(2)]
public class AsteroidsManager : Minigame
{
    //game variables
    private const int NUM_ROUNDS = 5;
    private int round = 0;

    //text to show on screen before the game
    private readonly string _name = "asteroids",
        _tutorial = "Click the corresponding color\n or \n turn the corresponding face.",
        endMessage = "WELL DONE!";

    //components
    [SerializeField] private GameObject uiObject;
    private float[] _arenaMeasures; //0 height 1 width
    [SerializeField] private Shader shader;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private PlayerAnimations _playerAnimations;
    private GenericScreenUi _genericScreenUi;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _asteroidTemplate;
    [SerializeField] private GameObject _asteroidContainer;

    //variables
    public float _speed = 0.1f;
    public float _cubeSpeed = 0.1f;
    private int horizontalGear = 0;
    private int verticaGear = 0;
    private bool minigameStarted = false;
    public float killingRange = 0.01f;

    //lists
    private Stack<AsteroidBehavior> _asteroidBehaviors;
    private List<AsteroidBehavior> _asteroidBehaviorsAlive;

    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    private void Awake()
    {
        _asteroidBehaviors = new Stack<AsteroidBehavior>();
        _asteroidBehaviorsAlive = new List<AsteroidBehavior>();
    }

    private void Update()
    {
        if (minigameStarted)
        {
            if (verticaGear < 0)
            {
                VerticalMovement(-1, _cubeSpeed);
            }
            else if (verticaGear > 0)
            {
                VerticalMovement(1, _cubeSpeed);
            }

            if (horizontalGear < 0)
            {
                HorizontalMovement(1, _cubeSpeed);
            }
            else if (horizontalGear > 0)
            {
                HorizontalMovement(-1, _cubeSpeed);
            }
        }
    }

    void Start()
    {
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _minigameManager = FindObjectOfType<MinigameManager>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _arenaMeasures = new[]
            { uiObject.GetComponent<RectTransform>().rect.height, uiObject.GetComponent<RectTransform>().rect.width };
        _player.SetActive(false);
        _minigameManager.UpdateCounter(round);
        _genericScreenUi.SetTextAlpha(0);
        HideUI();
    }

    public void VerticalMovement(int direction, float speed)
    {
        if (direction == 1)
        {
            if (_player.transform.localPosition.y < (_arenaMeasures[0] - _arenaMeasures[0] / 100) / 2)
                _player.transform.localPosition +=
                    new Vector3(0, speed * Time.deltaTime, 0);
            else
            {
                _player.transform.localPosition =
                    new Vector3(_player.transform.localPosition.x,-_arenaMeasures[0] / 2 , 0);
                horizontalGear = 0;
            }
        }
        else
        {
            if (_player.transform.localPosition.y > (-_arenaMeasures[0] + _arenaMeasures[0] / 100) / 2)
                _player.transform.localPosition -=
                    new Vector3(0, speed * Time.deltaTime, 0);
            else
            {
                _player.transform.localPosition =
                    new Vector3(_player.transform.localPosition.x,_arenaMeasures[0] / 2 , 0);
                horizontalGear = 0;
            }
        }
    }

    public void HorizontalMovement(int direction, float speed)
    {
        if (direction == 1)
        {
            if (_player.transform.localPosition.x < (_arenaMeasures[1] - _arenaMeasures[1] / 100) / 2)
                _player.transform.localPosition +=
                    new Vector3(speed * Time.deltaTime, 0, 0);
            else
            {
                _player.transform.localPosition =
                    new Vector3(-_arenaMeasures[1] / 2, _player.transform.localPosition.y, 0);
                verticaGear = 0;
            }
        }
        else
        {
            if (_player.transform.localPosition.x > (-_arenaMeasures[1] + _arenaMeasures[1] / 100) / 2)
                _player.transform.localPosition -=
                    new Vector3(speed * Time.deltaTime, 0, 0);
            else
            {
                _player.transform.localPosition =
                    new Vector3(_arenaMeasures[1] / 2, _player.transform.localPosition.y, 0);
                verticaGear = 0;
            }
        }
    }

    public float[] GetArenaTransform()
    {
        return _arenaMeasures;
    }

    public Vector3 VectorTowardsPlayer(Vector3 asteroid)
    {
        return _player.transform.localPosition - asteroid;
    }

    public float DistanceToPlayer(Vector3 asteroidPosition)
    {
        return Vector3.Distance(_player.transform.localPosition, asteroidPosition);
    }

    public void SetGearsZero()
    {
        horizontalGear = 0;
        verticaGear = 0;
    }

    public void HorizontalMovementCube(int direction)
    {
        verticaGear = 0;
        if (direction > 0)
            horizontalGear = Math.Min(horizontalGear + 1, 1);
        else
            horizontalGear = Math.Max(horizontalGear - 1, -1);
    }

    public void VerticalMovementCube(int direction)
    {
        horizontalGear = 0;
        if (direction > 0)
            verticaGear = Math.Min(verticaGear + 1, 1);
        else
            verticaGear = Math.Max(verticaGear - 1, -1);
    }

    private void StartRound()
    {
        for (int i = 0; i < (round * 2) + 1; i++)
        {
            if (_asteroidBehaviors.Count > 0)
            {
                AsteroidBehavior aux = _asteroidBehaviors.Pop();
                aux.Spawn();
                _asteroidBehaviorsAlive.Add(aux);
            }
            else
            {
                GameObject asteroid = Instantiate(_asteroidTemplate, _asteroidContainer.transform);
                AsteroidBehavior aux = asteroid.GetComponent<AsteroidBehavior>();
                _asteroidBehaviorsAlive.Add(aux);
            }
        }
    }

//will be called every time an asteroid dies
    public void EndRound()
    {
        //round finished
        if (_asteroidBehaviorsAlive.Count == 0)
        {
            round++;
            if (round < NUM_ROUNDS)
            {
                _minigameManager.UpdateCounter(round);
                StartRound();
            }
            else
            {
                EndMinigame();
            }
        }
    }

    public void KillPlayer()
    {
        foreach (AsteroidBehavior asteroid in _asteroidBehaviorsAlive)
        {
            asteroid.resetAsteroid();
            _asteroidBehaviors.Push(asteroid);
        }

        _asteroidBehaviorsAlive.Clear();
        round = 0;
        _minigameManager.UpdateCounter(round);
        StartRound();
    }

    public void TransferAsteroid(AsteroidBehavior asteroidBehavior)
    {
        _asteroidBehaviors.Push(asteroidBehavior);
        _asteroidBehaviorsAlive.Remove(asteroidBehavior);
    }

    public override void StartMinigame()
    {
        round = 0;
        ShowUI();
        HideGameUi();
        _playerValues.SetCurrentInput(CurrentInput.AsteroidMinigame);
        _playerValues.SetInputsEnabled(true);
        StartCoroutine(StartGameCoroutine());
    }

    private void EndMinigame()
    {
        _player.SetActive(false);
        _minigameManager.UpdateCounter(0);
        HideGameUi();
        minigameStarted = false;
        _playerValues.SetInputsEnabled(false);
        StartCoroutine(EndGameCoroutine());
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
        _minigameManager.SetCounterVisivility(true);
    }

    private void HideGameUi()
    {
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
        minigameStarted = true;
        _player.SetActive(true);
        StartRound();
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