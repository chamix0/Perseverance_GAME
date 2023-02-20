using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MinigameManager : MonoBehaviour
{
    //components
    [SerializeField] private GameObject counterObject;
    [SerializeField] private Shader shader;

    //variables
    private int lastMinigame = -1;

    //lists
    private List<Image> counterImages;
    [SerializeField] private List<Minigame> _minigames;

    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    private void Awake()
    {
        counterImages = new List<Image>();
    }

    void Start()
    {
        counterImages.AddRange(counterObject.GetComponentsInChildren<Image>());
        SetCounterImages();
        SetCounterVisivility(false);
    }

    #region Counter

    public void SetCounterVisivility(bool val)
    {
        counterObject.SetActive(val);
    }

    public void UpdateCounter(int correctCount)
    {
        for (int i = 0; i < counterImages.Count; i++)
        {
            if (i < correctCount)
            {
                counterImages[i].material.SetFloat(MyAlpha, 1f);
                counterImages[i].material.SetColor(BackgroundColor, Color.magenta);
            }
            else
            {
                counterImages[i].material.SetColor(BackgroundColor, Color.green);
                counterImages[i].material.SetFloat(MyAlpha, 0.1f);
            }
        }
    }

    private void SetCounterImages()
    {
        foreach (var image in counterImages)
        {
            Material material = new Material(shader);
            image.material = material;
        }
    }

    #endregion


    public void StartRandomMinigame()
    {
        int index;
        if (lastMinigame == -1)
            index = Random.Range(0, _minigames.Count);
        else
        {
            do
            {
                index = Random.Range(0, _minigames.Count);
            } while (index != lastMinigame);
            lastMinigame = index;
        }

        print("selecting  minigame");
        _minigames[index].StartMinigame();
    }
}