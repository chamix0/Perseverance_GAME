using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UTILS;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup loadingScreen;
    [SerializeField] private TMP_Text _text;
    private PlayerNewInputs _playerNewInputs;
    private Stopwatch _stopWatch;
    private string sceneIndex;
    private bool loading;
    private float loadingTime = 5;
    private AsyncOperation _operation;

    private void Start()
    {
        _playerNewInputs = FindObjectOfType<PlayerNewInputs>();
        loadingScreen.alpha = 0;
        _stopWatch = new Stopwatch();
        LightProbes.needsRetetrahedralization += LightProbes.TetrahedralizeAsync;
    }

    private void Update()
    {
        if (loading)
            LoadingScene();
    }

    public void LoadCubeConexion()
    {
        LoadScene("Scenes/Game/connect cube");
    }

    public void LoadMenu()
    {
        LoadScene("Scenes/Game/Main menu");
    }

    public void LoadLevels()
    {
        LoadScene("Scenes/Game/separated levels/Main level");
    }

    public void LoadCredits()
    {
        LoadScene("Scenes/Game/final credits");
    }

    public void LoadMovementTutorial()
    {
        LoadScene("Scenes/Game/Tutorials/Door 1");
    }

    public void LoadRaceTutorial()
    {
        LoadScene("Scenes/Game/Tutorials/race 1");
    }

    public void LoadStealthTutorial()
    {
        LoadScene("Scenes/Game/Tutorials/camera enem 1");
    }

    public void LoadMinigamesTutorial()
    {
        LoadScene("Scenes/Game/Tutorials/terminals 1");
    }

    public void LoadShootingTutorial()
    {
        LoadScene("Scenes/Game/Tutorials/target door 1");
    }

    public void LoadArduinoConnect()
    {
        LoadScene("Scenes/Game/Connect Arduino");
    }

    public void LoadEnemiesTutorial()
    {
        LoadScene("Scenes/Game/Tutorials/enemies 1");
    }

    public void LoadArcadeGame()
    {
        LoadScene("Scenes/Game/Arcade/Arcade Game");
    }

    #region Load levels async

    public void LoadFoundry()
    {
        SceneManager.LoadSceneAsync("Scenes/Game/separated levels/Foundry", LoadSceneMode.Additive);
    }

    public void LoadFreezer()
    {
        _operation = SceneManager.LoadSceneAsync("Scenes/Game/separated levels/Freezer", LoadSceneMode.Additive);
    }

    public void LoadWarehouse()
    {
        _operation = SceneManager.LoadSceneAsync("Scenes/Game/separated levels/Warehouse", LoadSceneMode.Additive);
    }

    public void LoadGarden()
    {
        _operation = SceneManager.LoadSceneAsync("Scenes/Game/separated levels/Garden", LoadSceneMode.Additive);
    }

    public void LoadResidential()
    {
        _operation = SceneManager.LoadSceneAsync("Scenes/Game/separated levels/Residential", LoadSceneMode.Additive);
    }

    #endregion

    public void LoadArcadeStatsScreen()
    {
        SceneManager.LoadScene("Scenes/Game/Arcade/Arcade final Screen");
    }

    public bool Loaded()
    {
        return _operation.isDone;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetLoadingText(string text)
    {
        _text.text = text;
    }


    private void LoadScene(string index)
    {
        loadingScreen.alpha = 1;
        loadingScreen.interactable = false;
        loadingScreen.blocksRaycasts = true;
        _stopWatch.Start();
        sceneIndex = index;
        loading = true;
        if (_playerNewInputs)
            _playerNewInputs.DisableControls();
    }


    private void LoadingScene()
    {
        if (_stopWatch.Elapsed.TotalSeconds > loadingTime)
        {
            _stopWatch.Stop();
            loading = false;
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        }
    }
}