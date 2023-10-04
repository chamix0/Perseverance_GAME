using System.Collections.Generic;
using Main_menu;
using Main_menu.New_game_screen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeMenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup, secondPlayButtonCanvas;
    [SerializeField] private Button playButton, secondPlayButton;
    [SerializeField] private List<TMP_Text> leaderBoardNames, leaderBoardRounds, leaderBoardPoints;
    private JSONsaving _jsoNsaving;
    private SaveData _saveData;
    private MyMenuInputManager _menuInputManager;
    private MenuCamerasController _camerasController;
    private NewGameManager _newGameManager;
    private MainMenuSounds _sounds;
    private LoadScreen _loadScreen;

    private void Start()
    {
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _menuInputManager = FindObjectOfType<MyMenuInputManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _newGameManager = FindObjectOfType<NewGameManager>();
        _loadScreen = FindObjectOfType<LoadScreen>();
        _saveData = _jsoNsaving._saveData;
        _sounds = FindObjectOfType<MainMenuSounds>();
        HideUi();
        playButton.onClick.AddListener(PlayButtonAction);
        secondPlayButton.onClick.AddListener(PlaySecondButtonAction);
        HidePlayButton();
    }

    public void PlayButtonAction()
    {
        _menuInputManager.SetCurrentInput(CurrentMenuInput.Arcade);
        _camerasController.SetCamera(MenuCameras.NewGame);
        _newGameManager.ShowUI();
        playButton.interactable = false;
    }

    public void PlaySecondButtonAction()
    {
        _sounds.SelectOptionSound();
        _jsoNsaving._saveData.SetArcadeModel(_newGameManager.GetModelIndex());
        _jsoNsaving._saveData.SetArcadeName(_newGameManager.GetName());
        _jsoNsaving.SaveTheData();
        _loadScreen.LoadArcadeGame();
        playButton.interactable = false;
    }

    public void ShowUi()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        UpdateLeaderBoards();
    }

    public void ShowPlayButton()
    {
        secondPlayButtonCanvas.alpha = 1;
        secondPlayButtonCanvas.interactable = true;
        secondPlayButtonCanvas.blocksRaycasts = true;
    }

    public void HidePlayButton()
    {
        secondPlayButtonCanvas.alpha = 0;
        secondPlayButtonCanvas.interactable = false;
        secondPlayButtonCanvas.blocksRaycasts = false;
    }

    public void HideUi()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private void UpdateLeaderBoards()
    {
        Score[] leaderBoards = _saveData.GetLeaderBoard();
        for (int i = 0; i < leaderBoardNames.Count; i++)
        {
            leaderBoardNames[i].text = leaderBoards[i].GetName();
            leaderBoardRounds[i].text = leaderBoards[i].GetRound() + "";
            leaderBoardPoints[i].text = leaderBoards[i].GetPoints() + "";
        }
    }
}