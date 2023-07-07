using System.Collections.Generic;
using Main_menu;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    //components
    private MyMenuInputManager _myInputManager;
    private GalleryManager galleryManager;
    private MenuCamerasController _camerasController;
    private SaveData _saveData;
    [SerializeField] private List<Transform> imagesPoints;
    [SerializeField] private List<Transform> lookAtPoints;
    private int index = 0;
    private MainMenuSounds _sounds;
    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        galleryManager = FindObjectOfType<GalleryManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
    }



    public void ShowNext()
    {
        _sounds.ChangeOptionSound();
        index = (index + 1) % imagesPoints.Count;
        _camerasController.SetCameraNewPosAndLookAt(MenuCameras.Gallery, imagesPoints[index].position,
            lookAtPoints[index]);
    }

    public void ShowPrev()
    {
        _sounds.ChangeOptionSound();
        index = index - 1 < 0 ? imagesPoints.Count - 1 : index - 1;
        _camerasController.SetCameraNewPosAndLookAt(MenuCameras.Gallery, imagesPoints[index].position,
            lookAtPoints[index]);
    }
}