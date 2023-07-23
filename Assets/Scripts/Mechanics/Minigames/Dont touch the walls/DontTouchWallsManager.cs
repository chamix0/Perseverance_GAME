using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;
using Random = UnityEngine.Random;

public class DontTouchWallsManager : Minigame
{
    //text to show on screen before the game
    private readonly string _name = "Dont touch the walls",
        _tutorial = "Reach the goal safely",
        endMessage = "WELL DONE!";

    private const int NUM_ROUNDS = 5;
    private int round = 0;

    //components
    [SerializeField] private GameObject uiObject;
    [SerializeField] private Transform laberinthContainer;
    private LaberinthWalls currentLaberinth;
    private float[] _arenaMeasures; //0 height 1 width
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;
    private MinigameManager _minigameManager;
    private GenericScreenUi _genericScreenUi;
    [SerializeField] private GameObject _player;
    private MinigameSoundManager minigameSoundManager;

    //variables
    public float _speed = 0.1f;
    public float _cubeSpeed = 0.1f;
    private int horizontalGear = 0;
    private int verticaGear = 0;
    private bool minigameStarted = false;

    //laberinths
    [SerializeField] private List<GameObject> easyLaberinthWallsObj, mediumLaberinthWallsObj, hardLaberinthWallsObj;
    private List<LaberinthWalls> laberinthWallsList, easyLaberinthWalls, mediumLaberinthWalls, hardLaberinthWalls;

    private void Awake()
    {
        laberinthWallsList = new List<LaberinthWalls>();
        easyLaberinthWalls = new List<LaberinthWalls>();
        mediumLaberinthWalls = new List<LaberinthWalls>();
        hardLaberinthWalls = new List<LaberinthWalls>();


        minigameSoundManager = GetComponent<MinigameSoundManager>();
    }

    void Start()
    {
        GetAllLaberinths();
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

    private void Update()
    {
        if (minigameStarted)
        {
            Vector2 dir = Vector2.zero;
            if (verticaGear < 0)
            {
                dir.y -= 1;
                VerticalMovement(-1, _cubeSpeed);
            }
            else if (verticaGear > 0)
            {
                dir.y += 1;
                VerticalMovement(1, _cubeSpeed);
            }

            if (horizontalGear < 0)
            {
                dir.x -= 1;
                HorizontalMovement(-1, _cubeSpeed);
            }
            else if (horizontalGear > 0)
            {
                dir.x += 1;
                HorizontalMovement(1, _cubeSpeed);
            }

            SpriteRotation(dir);
        }
    }

    private void GetAllLaberinths()
    {
        foreach (var obj in easyLaberinthWallsObj)
        {
            GameObject aux = Instantiate(obj, laberinthContainer);
            LaberinthWalls laberinth = aux.GetComponent<LaberinthWalls>();
            easyLaberinthWalls.Add(laberinth);
            laberinth.DisableLaberinth();
        }

        foreach (var obj in mediumLaberinthWallsObj)
        {
            GameObject aux = Instantiate(obj, laberinthContainer);
            LaberinthWalls laberinth = aux.GetComponent<LaberinthWalls>();
            mediumLaberinthWalls.Add(laberinth);
            laberinth.DisableLaberinth();
        }

        foreach (var obj in hardLaberinthWallsObj)
        {
            GameObject aux = Instantiate(obj, laberinthContainer);
            LaberinthWalls laberinth = aux.GetComponent<LaberinthWalls>();
            hardLaberinthWalls.Add(laberinth);
            laberinth.DisableLaberinth();
        }
    }

    private void HideAllLaberinths()
    {
        foreach (var laberinth in easyLaberinthWalls)
            laberinth.DisableLaberinth();
        foreach (var laberinth in mediumLaberinthWalls)
            laberinth.DisableLaberinth();
        foreach (var laberinth in hardLaberinthWalls)
            laberinth.DisableLaberinth();
    }

    private void SelectLaberinths()
    {
        int randomIndex;
        laberinthWallsList.Clear();
        List<int> usedIndex = new List<int>();

        //easy
        for (int i = 0; i < 2; i++)
        {
            do
            {
                randomIndex = Random.Range(0, easyLaberinthWalls.Count);
            } while (usedIndex.Contains(randomIndex));

            usedIndex.Add(randomIndex);
            laberinthWallsList.Add(easyLaberinthWalls[randomIndex]);
        }

        usedIndex.Clear();

        //medium 
        for (int i = 0; i < 2; i++)
        {
            do
            {
                randomIndex = Random.Range(0, mediumLaberinthWalls.Count);
            } while (usedIndex.Contains(randomIndex));

            usedIndex.Add(randomIndex);
            laberinthWallsList.Add(mediumLaberinthWalls[randomIndex]);
        }

        //hard
        randomIndex = Random.Range(0, hardLaberinthWalls.Count);
        laberinthWallsList.Add(hardLaberinthWalls[randomIndex]);
    }

    #region Movement

    private void RotatePlayer(float angle)
    {
        _player.transform.localRotation =
            Quaternion.Slerp(_player.transform.localRotation, Quaternion.Euler(0, 0, MyUtils.Clamp0360(angle)),
                Time.deltaTime * 10f);
    }

    public void SpriteRotation(Vector2 direction)
    {
        float x = direction.x;
        float y = direction.y;
        if (x > 0)
        {
            RotatePlayer(0);
            if (y < 0)
                RotatePlayer(-45);
            else if (y > 0)
                RotatePlayer(45);
        }
        else if (x == 0)
        {
            if (y < 0)
                RotatePlayer(-90);
            else if (y > 0)
                RotatePlayer(90);
        }
        else if (x < 0)
        {
            RotatePlayer(180);
            if (y < 0)
                RotatePlayer(-135);
            else if (y > 0)
                RotatePlayer(135);
        }
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
                    new Vector3(_player.transform.localPosition.x, -_arenaMeasures[0] / 2, 0);
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
                    new Vector3(_player.transform.localPosition.x, _arenaMeasures[0] / 2, 0);
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

    #endregion

    public float[] GetArenaTransform()
    {
        return _arenaMeasures;
    }

    public Vector3 VectorTowardsPlayer(Vector3 asteroid)
    {
        return _player.transform.localPosition - asteroid;
    }

    public Vector3 PlayerPos()
    {
        return _player.transform.localPosition;
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


    private void ShowCurrentLaberinth()
    {
        for (int i = 0; i < laberinthWallsList.Count; i++)
        {
            if (i == round)
                laberinthWallsList[i].ShowLaberinth();
            else
                laberinthWallsList[i].DisableLaberinth();
        }
    }

    private void StartRound()
    {
        ShowCurrentLaberinth();
        _player.transform.localPosition = laberinthWallsList[round].GetSpawnPoint();
        //change laberinth
    }

//will be called every time an asteroid dies
    public void EndRound()
    {
        //round finished

        round++;
        if (round < NUM_ROUNDS)
        {
            minigameSoundManager.PlayCorrectSound();
            _minigameManager.UpdateCounter(round);
            StartRound();
        }
        else
        {
            EndMinigame();
        }
    }

    public void KillPlayer()
    {
        //reset pos
        _player.transform.localPosition = laberinthWallsList[0].GetSpawnPoint();
        minigameSoundManager.PlayInCorrectSound();
        round = 0;
        _minigameManager.UpdateCounter(round);
        StartRound();
    }


    public override void StartMinigame()
    {
        round = 0;
        SelectLaberinths();
        _minigameManager.UpdateCounter(0);
        _playerValues.SetCurrentInput(CurrentInput.DontTouchTheWallsMinigame);
        _playerValues.SetInputsEnabled(true);
        StartCoroutine(StartGameCoroutine());
    }

    private void EndMinigame()
    {
        minigameSoundManager.PlayFinishedSound();
        _player.SetActive(false);
        _minigameManager.UpdateCounter(0);
        HideAllLaberinths();
        minigameStarted = false;
        _playerValues.SetInputsEnabled(false);
        laberinthWallsList.Clear();
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

    #region tutorial

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


    IEnumerator StartGameCoroutine()
    {
        //enseñar nombre del minijuego
        _genericScreenUi.SetText(_name, 30);
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
        ShowUI();
        minigameStarted = true;
        _player.SetActive(true);
        StartRound();
        _playerValues.NotifyAction(PlayerActions.DontTouchTheWallsMinigame);
    }
}