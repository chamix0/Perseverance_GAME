using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons;
    private List<TMP_Text> _texts;
    private List<Material> _materials;
    private JSONsaving _jsoNsaving;
    private SaveData _saveData;
    public Color _colorHighlighted;
    private Color _colorSelected;
    public Color _notInteracctableColor;
    private bool continueInteractable, NewGameInteractable;
    private int selectedButton;
    [SerializeField] private Shader buttonShader;
    private MyMenuInputManager _menuInputManager;

    private MenuCamerasController _camerasController;

    //shader properties
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");


    // Start is called before the first frame update
    private void Awake()
    {
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuInputManager = FindObjectOfType<MyMenuInputManager>();
        _jsoNsaving = GetComponent<JSONsaving>();
        _saveData = _jsoNsaving._saveData;
    }

    void Start()
    {
        _materials = new List<Material>();
        _texts = new List<TMP_Text>();
        _colorSelected = _buttons[0].GetComponent<Image>().material.GetColor(BackgroundColor);
        for (int i = 0; i < _buttons.Count; i++)
        {
            Material material = new Material(buttonShader);
            _buttons[i].GetComponent<Image>().material = material;
            _materials.Add(material);
            _texts.Add(_buttons[i].GetComponentInChildren<TMP_Text>());
            int aux = i;
            _buttons[i].onClick.AddListener(() => clicked(aux));
        }

        selectedButton = 0;
        CheckForContinueAndNewGame();
        if (!continueInteractable)
            selectedButton = 1;
        UpdateColors();
    }

    // Update is called once per frame
    void Update()
    {
        print(selectedButton);
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

    private void CheckForContinueAndNewGame()
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

    IEnumerator ActionForButtonCoroutine(int index)
    {
        CheckForContinueAndNewGame();
        yield return new WaitForSeconds(1f);
        switch (index)
        {
            //continue
            case 0:
                print("continue --------");
                break;
            //new game
            case 1:
                //cambiar la camara para pasar al modo seleccion de personajes
                _camerasController.SetCamera(1);
                _menuInputManager.SetCurrentInput(CurrentMenuInput.NewGame);
                break;
            //load game
            case 2:
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

    private void Exit()
    {
        Application.Quit();
    }

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
        aux = selectedButton - 1 < 0 ? _buttons.Count - 1 : aux - 1;
        if (aux == 1 && !NewGameInteractable)
            aux = 0;
        if (aux == 0 && !continueInteractable)
            aux = _buttons.Count - 1;

        selectedButton = aux;
        UpdateColors();
    }

    public int GetSelectedButton()
    {
        return selectedButton;
    }

    private void UpdateColors()
    {
        print("selected button " + selectedButton);
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
}