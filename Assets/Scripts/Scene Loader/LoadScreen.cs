using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup loadingScreen;
    [SerializeField] private Slider slider;

    private void Start()
    {
        loadingScreen.alpha = 0;
        slider.value = 0;
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
        StartCoroutine(LoadScenAsync(4));
    }
    public void LoadKeyOrCube()
    {
        StartCoroutine(LoadScenAsync(0));
    }

    IEnumerator LoadScenAsync(int sceneIndex)
    {
        loadingScreen.alpha = 1;
        
        yield return new WaitForSeconds(5);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progressValue;
            yield return null;
        }
    }
}