using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(4)]
public class MinigameManager : MonoBehaviour
{
    //components
    [SerializeField] private GameObject counterObject;
    [SerializeField] private Shader shader;
    private GuiManager guiManager;

    //variables
    private int _lastMinigame = -1;

    //lists
    [SerializeField] private List<Image> _counterImages;
    private List<Minigame> minigames;

    //shader names
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");

    private void Awake()
    {
        minigames = new List<Minigame>();
    }

    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        //minigames 
        minigames.Add(GetComponent<ColorsManager>());
        minigames.Add(GetComponent<AsteroidsManager>());
        minigames.Add(GetComponent<PushFastManager>());
        minigames.Add(GetComponent<JustWaitManager>());
        minigames.Add(GetComponent<RollTheNutManager>());
        minigames.Add(GetComponent<AdjustValuesManager>());
        minigames.Add(GetComponent<MemoryMingameManager>());
        minigames.Add(GetComponent<DontTouchWallsManager>());

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
        int index;
        if (_lastMinigame == -1)
        {
            index = Random.Range(0, minigames.Count);
            _lastMinigame = index;
        }
        else
        {
            do
            {
                index = Random.Range(0, minigames.Count);
            } while (index == _lastMinigame);

            _lastMinigame = index;
        }

        minigames[7].StartMinigame();
        guiManager.HideGui();
    }

    public void EndMinigame()
    {
        guiManager.ShowGui();
    }
}