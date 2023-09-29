using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup loadingScreen;
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        loadingScreen.alpha = 0;
    }

    public void LoadCubeConexion()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/connect cube"));
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Main menu"));
    }

    public void LoadLevels()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/separated levels/Main level"));
    }

    public void LoadCredits()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/final credits"));
    }

    public void LoadMovementTutorial()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Tutorials/Door 1"));
    }

    public void LoadRaceTutorial()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Tutorials/race 1"));
    }

    public void LoadStealthTutorial()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Tutorials/camera enem 1"));
    }

    public void LoadMinigamesTutorial()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Tutorials/terminals 1"));
    }

    public void LoadShootingTutorial()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Tutorials/target door 1"));
    }

    public void LoadArduinoConnect()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Connect Arduino"));
    }

    public void LoadEnemiesTutorial()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Tutorials/enemies 1"));
    }

    public void LoadArcadeGame()
    {
        StartCoroutine(LoadSceneCoroutine("Scenes/Game/Arcade/Arcade Game"));
    }

    #region Load levels async

    public void LoadFoundry()
    {
        SceneManager.LoadScene("Scenes/Game/separated levels/Foundry", LoadSceneMode.Additive);
    }

    public void LoadFreezer()
    {
        SceneManager.LoadScene("Scenes/Game/separated levels/Freezer", LoadSceneMode.Additive);
    }

    public void LoadWarehouse()
    {
        SceneManager.LoadScene("Scenes/Game/separated levels/Warehouse", LoadSceneMode.Additive);
    }

    public void LoadGarden()
    {
        SceneManager.LoadScene("Scenes/Game/separated levels/Garden", LoadSceneMode.Additive);
    }

    public void LoadResidential()
    {
        SceneManager.LoadScene("Scenes/Game/separated levels/Residential", LoadSceneMode.Additive);
    }

    #endregion

    public void LoadArcadeStatsScreen()
    {
        SceneManager.LoadScene("Scenes/Game/Arcade/Arcade final Screen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetLoadingText(string text)
    {
        _text.text = text;
    }

    IEnumerator LoadSceneCoroutine(string sceneIndex)
    {
        loadingScreen.alpha = 1;
        loadingScreen.interactable = false;
        loadingScreen.blocksRaycasts = true;
        print("AAAA");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(sceneIndex,LoadSceneMode.Single);
    }
}