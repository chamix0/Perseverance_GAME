using System.Collections;
using System.Collections.Generic;
using Main_menu;
using Main_menu.Load_game_screen;
using Main_menu.New_game_screen;
using TMPro;
using UnityEngine;
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
    private LoadScreen loadScreen;

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

    //tutorial
    [SerializeField] private TMP_Text tutorialText;

    //shader properties
    private static readonly int MyAlpha = Shader.PropertyToID("_MyAlpha");
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");

    //sounds
    private MainMenuSounds _sounds;


    private void Awake()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _menuInputManager = FindObjectOfType<MyMenuInputManager>();
        _newGameManager = FindObjectOfType<NewGameManager>();
        _loadGameManager = FindObjectOfType<LoadGameManager>();
        loadScreen = FindObjectOfType<LoadScreen>();
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
        _sounds.SelectOptionSound();

        _loadGameManager.HideUI();
        _menuInputManager.SetCurrentInput(CurrentMenuInput.Menu);
        int aux = button;
        selectedButton = aux;
        UpdateColors();
        _texts[aux].color = Color.white;
        _materials[aux].SetColor(BackgroundColor, _colorSelected);
        StartCoroutine(ActionForButtonCoroutine(aux));
    }

    public void PressEnter()
    {
        _sounds.SelectOptionSound();


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

        //cameras
        if (index != 2)
            _loadGameManager.HideUI();
        if (index == 1)
            _camerasController.SetCamera(MenuCameras.NewGame);
        else if (index == 3)
            _camerasController.SetCamera(MenuCameras.Tutorial);
        else if (index == 4)
            _camerasController.SetCamera(MenuCameras.Settings);
        else if (index == 5)
            _camerasController.SetCamera(MenuCameras.Gallery);
        else if (index == 6)
            _camerasController.SetCamera(MenuCameras.Credits);


        yield return new WaitForSeconds(0.25f);
        switch (index)
        {
            //continue
            case 0:
                loadScreen.LoadLevels();
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
                _menuInputManager.SetCurrentInput(CurrentMenuInput.Tutorial);

                break;
            //settings
            case 4:
                _menuInputManager.SetCurrentInput(CurrentMenuInput.Settings);

                break;
            //Gallery
            case 5:
                _menuInputManager.SetCurrentInput(CurrentMenuInput.Gallery);

                break;
            //credits
            case 6:
                _menuInputManager.SetCurrentInput(CurrentMenuInput.Credits);
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
        _sounds.ChangeOptionSound();

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
        _sounds.ChangeOptionSound();

        int aux = selectedButton;
        aux = aux - 1 < 0 ? _buttons.Count - 1 : aux - 1;
        if (aux == 1 && !NewGameInteractable)
            aux = 0;
        if (aux == 0 && !continueInteractable)
            aux = _buttons.Count - 1;

        selectedButton = aux;
        UpdateColors();
    }

    public void SetTutortialText(string text)
    {
        tutorialText.text = text;
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