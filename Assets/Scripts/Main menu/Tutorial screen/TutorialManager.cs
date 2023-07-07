using System.Collections.Generic;
using Main_menu;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private MenuCamerasController _camerasController;
    private SaveData _saveData;
    private MainMenuSounds _sounds;
    private LoadScreen _loadScreen;
    [SerializeField] private List<Transform> imagesPoints;
    [SerializeField] private List<Transform> lookAtPoints;
    private int index = 0;

    void Start()
    {
        _loadScreen = FindObjectOfType<LoadScreen>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
        _sounds = FindObjectOfType<MainMenuSounds>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowNext()
    {
        _sounds.ChangeOptionSound();
        index = (index + 1) % imagesPoints.Count;
        _camerasController.SetCameraNewPosAndLookAt(MenuCameras.Tutorial, imagesPoints[index].position,
            lookAtPoints[index]);
    }

    public void ShowPrev()
    {
        _sounds.ChangeOptionSound();

        index = index - 1 < 0 ? imagesPoints.Count - 1 : index - 1;
        _camerasController.SetCameraNewPosAndLookAt(MenuCameras.Tutorial, imagesPoints[index].position,
            lookAtPoints[index]);
    }

    public void Select()
    {
        if (index == 16)
        {
            _sounds.SelectOptionSound();
            _loadScreen.LoadMovementTutorial();
        }
        else if (index == 17)
        {
            _sounds.SelectOptionSound();
            _loadScreen.LoadRaceTutorial();
        }
        else if (index == 18)
        {
            _sounds.SelectOptionSound();
            _loadScreen.LoadStealthTutorial();
        }
        else if (index == 19)
        {
            _sounds.SelectOptionSound();
            _loadScreen.LoadMinigamesTutorial();
        }
        else if (index == 20)
        {
            _sounds.SelectOptionSound();
            _loadScreen.LoadShootingTutorial();
        }
        else if (index == 21)
        {
            _sounds.SelectOptionSound();
            _loadScreen.LoadEnemiesTutorial();
        }
    }
}