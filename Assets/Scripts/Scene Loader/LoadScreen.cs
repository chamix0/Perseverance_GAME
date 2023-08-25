using System.Collections;
using TMPro;
using Unity.Profiling;
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

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadScenAsync(sceneIndex));
    }

    public void LoadCubeConexion()
    {
        StartCoroutine(LoadScenAsync(1));
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadScenAsync(2));
    }

    public void LoadLevels()
    {
        StartCoroutine(LoadScenAsync(3));
    }

    public void LoadCredits()
    {
        StartCoroutine(LoadScenAsync(11));
    }

    public void LoadKeyOrCube()
    {
        StartCoroutine(LoadScenAsync(0));
    }

    public void LoadMovementTutorial()
    {
        StartCoroutine(LoadScenAsync(4));
    }

    public void LoadRaceTutorial()
    {
        StartCoroutine(LoadScenAsync(5));
    }

    public void LoadStealthTutorial()
    {
        StartCoroutine(LoadScenAsync(6));
    }

    public void LoadMinigamesTutorial()
    {
        StartCoroutine(LoadScenAsync(7));
    }

    public void LoadShootingTutorial()
    {
        StartCoroutine(LoadScenAsync(8));
    }

    public void LoadArduinoConnect()
    {
        StartCoroutine(LoadScenAsync(9));
    }

    public void LoadEnemiesTutorial()
    {
        StartCoroutine(LoadScenAsync(10));
    }

    #region Load levels async

    public void LoadFreezer()
    {
        SceneManager.LoadScene(12, LoadSceneMode.Additive);
    }

    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetLoadingText(string text)
    {
        _text.text = text;
    }

    IEnumerator LoadScenAsync(int sceneIndex)
    {
        loadingScreen.alpha = 1;

        yield return new WaitForSeconds(5);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    }
}