using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(4)]
public class MinigameManager : MonoBehaviour
{
    //components
    [SerializeField] private GameObject counterObject;
    [SerializeField] private Shader shader;
    private GenericScreenUi _genericScreenUi;
    private PlayerValues _playerValues;
    private CameraChanger _cameraChanger;

    //variables
    private int _lastMinigame = -1;
    private bool currentMinigameFinished;

    //lists
    [SerializeField] private List<Image> _counterImages;
    private List<Minigame> minigames;
    private List<Minigame> _minigamesNotPlayed;

    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    private void Awake()
    {
        _minigamesNotPlayed = new List<Minigame>();
        minigames = new List<Minigame>();
    }

    void Start()
    {
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        _playerValues = FindObjectOfType<PlayerValues>();
        _cameraChanger = FindObjectOfType<CameraChanger>();
        //minigames 
        minigames.Add(GetComponent<ColorsManager>());
        minigames.Add(GetComponent<AsteroidsManager>());
        minigames.Add(GetComponent<PushFastManager>());
        minigames.Add(GetComponent<JustWaitManager>());
        minigames.Add(GetComponent<RollTheNutManager>());
        minigames.Add(GetComponent<AdjustValuesManager>());
        minigames.Add(GetComponent<MemoryMingameManager>());
        minigames.Add(GetComponent<DontTouchWallsManager>());
        minigames.Add(GetComponent<PuzzleManager>());
        _minigamesNotPlayed.AddRange(minigames.ToArray());
        SetCounterImages();
        UpdateCounter(0);
        SetCounterVisivility(false);
    }

    #region Counter

    public void SetCounterVisivility(bool val)
    {
        counterObject.SetActive(val);
    }

    public void UpdateCounter(int correctCount)
    {
        for (int i = 0; i < _counterImages.Count; i++)
        {
            _counterImages[i].material.SetColor(BackgroundColor, Color.green);

            if (i < correctCount)
                _counterImages[i].material.SetFloat(MyAlpha, 1f);
            else
                _counterImages[i].material.SetFloat(MyAlpha, 0.1f);
        }
    }

    private void SetCounterImages()
    {
        foreach (var image in _counterImages)
        {
            Material material = new Material(shader);
            image.material = material;
        }
    }

    #endregion


    public void StartRandomMinigame()
    {
        int index = Random.Range(0, _minigamesNotPlayed.Count);
        Minigame currentMinigame = _minigamesNotPlayed[index];
        _minigamesNotPlayed.RemoveAt(index);
        if (_minigamesNotPlayed.Count <= 0)
            _minigamesNotPlayed.AddRange(minigames.ToArray());

        currentMinigame.StartMinigame();
        currentMinigameFinished = false;
    }

    public IEnumerator EndMinigame()
    {
        _genericScreenUi.SetText("WELL DONE!", 10);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(2f);
        _genericScreenUi.FadeOutText();
        yield return new WaitForSeconds(2f);
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(true, 0f);
        CursorManager.HideCursor();
        _playerValues.NotifyAction(PlayerActions.MinigameFinished);
        currentMinigameFinished = true;
        _playerValues.NotifyAction(PlayerActions.TurnOnPower);
    }

    public bool GetMinigameFinished()
    {
        return currentMinigameFinished;
    }
}