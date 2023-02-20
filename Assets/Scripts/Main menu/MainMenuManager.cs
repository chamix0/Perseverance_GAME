using System;
using System.Collections;
using System.Collections.Generic;
using Main_menu;
using Main_menu.Load_game_screen;
using Main_menu.New_game_screen;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class MainMenuManager : MonoBehaviour
{
    //components
    private MyMenuInputManager _menuInputManager;
    private NewGameManager _newGameManager;
    private MenuCamerasController _camerasController;
    private JSONsaving _jsoNsaving;
    private SaveData _saveData;
    [SerializeField] private Shader buttonShader;
    private LoadGameManager _loadGameManager;

    //variables
    public Color _colorHighlighted;
    private Color _colorSelected;
    public Color _notInteracctableColor;
    private bool continueInteractable, NewGameInteractable;

    private int selectedButton;

    //lists
    [SerializeField] private List<Button> _buttons;
    private List<TMP_Text> _texts;
    private List<Material> _materials;


    //shader properties
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");


    private void Awake()
    {
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuInputManager = FindObjectOfType<MyMenuInputManager>();
        _newGameManager = FindObjectOfType<NewGameManager>();
        _loadGameManager = FindObjectOfType<LoadGameManager>();
        _jsoNsaving = GetComponent<JSONsaving>();
        _materials = new List<Material>();
        _texts = new List<TMP_Text>();
        _saveData = _jsoNsaving._saveData;
    }

    void Start()
    {
        selectedButton = 0;
        _colorSelected = _buttons[0].GetComponent<Image>().material.GetColor(BackgroundColor);
        SetButtons();
        CheckForContinueAndNewGame();
        if (!continueInteractable)
            selectedButton = 1;
        UpdateColors();
    }

    public void clicked(int button)
    {
        int aux = button;
        selectedButton = aux;
        UpdateColors();
        _texts[aux].color = Color.white;
        _materials[aux].SetColor(BackgroundColor, _colorSelected);
        StartCoroutine(ActionForButtonCoroutine(aux));
    }

    public void PressEnter()
    {
        int aux = selectedButton;
        UpdateColors();
        _texts[aux].color = Color.white;
        _materials[aux].SetColor(BackgroundColor, _colorSelected);
        StartCoroutine(ActionForButtonCoroutine(aux));
    }

    /// <summary>
    /// Enables and disable continue and new game. Continue is disabled when there is no save data at all and
    /// new game is disabled when all slots are full.
    /// </summary>
    public void CheckForContinueAndNewGame()
    {
        if (_saveData.GetLastSessionSlotIndex() == -1)
        {
            continueInteractable = false;
            _texts[0].color = _notInteracctableColor;
            _buttons[0].interactable = false;
        }
        else
        {
            continueInteractable = true;
            _buttons[0].interactable = true;
        }

        if (!_saveData.AreThereEmptySlots())
        {
            NewGameInteractable = false;
            _texts[1].color = _notInteracctableColor;
            _buttons[1].interactable = false;
        }
        else
        {
            NewGameInteractable = true;
            _buttons[1].interactable = true;
        }
    }

    #region Actual actions for buttons

    /// <summary>
    /// Perform corresponding actions depending to the selected button
    /// </summary>
    /// <param name="index">index of the selected button</param>
    /// <returns></returns>
    IEnumerator ActionForButtonCoroutine(int index)
    {
        CheckForContinueAndNewGame();
        if (index != 2)
            _loadGameManager.HideUI();
        if (index == 1)
            _camerasController.SetCamera(1);

        yield return new WaitForSeconds(0.25f);
        switch (index)
        {
            //continue
            case 0:
                print("continue with slot: " + _saveData.GetLastSessionSlotIndex());
                //change scene to game
                break;
            //new game
            case 1:
                //cambiar la camara para pasar al modo seleccion de personajes
                _menuInputManager.SetCurrentInput(CurrentMenuInput.NewGame);
                _newGameManager.ShowUI();
                break;
            //load game
            case 2:
                _menuInputManager.SetCurrentInput(CurrentMenuInput.LoadGame);
                _loadGameManager.ShowUI();
                _loadGameManager.UpdateText();
                _loadGameManager.UpdateColors();

                break;
            //tutorial
            case 3:
                break;
            //settings
            case 4:
                break;
            //Gallery
            case 5:
                break;
            //credits
            case 6:
                break;
            //Exit
            case 7:
                Exit();
                break;
        }
    }

    private void Exit() => Application.Quit();

    #endregion

    public void SelectNextButton()
    {
        int aux = selectedButton;
        aux = (aux + 1) % _buttons.Count;

        if (aux == 0 && !continueInteractable)
            aux = 1;
        if (aux == 1 && !NewGameInteractable)
            aux = 2;
        selectedButton = aux;
        UpdateColors();
    }

    public void SelectPrevButton()
    {
        int aux = selectedButton;
        aux = aux - 1 < 0 ? _buttons.Count - 1 : aux - 1;
        if (aux == 1 && !NewGameInteractable)
            aux = 0;
        if (aux == 0 && !continueInteractable)
            aux = _buttons.Count - 1;

        selectedButton = aux;
        UpdateColors();
    }

    private void UpdateColors()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _texts[i].color = Color.black;
            if (i != selectedButton)
            {
                _materials[i].SetFloat(MyAlpha, 0);
            }
            else
            {
                _materials[i].SetFloat(MyAlpha, 1f);
                _materials[i].SetColor(BackgroundColor, _colorHighlighted);
            }
        }

        if (!continueInteractable)
            _texts[0].color = _notInteracctableColor;
        if (!NewGameInteractable)
            _texts[1].color = _notInteracctableColor;
    }

    private void SetButtons()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            Material material = new Material(buttonShader);
            _buttons[i].GetComponent<Image>().material = material;
            _materials.Add(material);
            _texts.Add(_buttons[i].GetComponentInChildren<TMP_Text>());
            int aux = i;
            _buttons[i].onClick.AddListener(() => clicked(aux));
        }
    }
}