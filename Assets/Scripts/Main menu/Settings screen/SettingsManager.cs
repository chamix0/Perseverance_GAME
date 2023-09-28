using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called before the first frame update
    private SoundManager soundManager;
    private JSONsaving _jsoNsaving;
    private LoadScreen _loadScreen;

    //settings
    private List<Slider> _sliders;
    [SerializeField] private Slider masterVolumeSlider, vfxVolumeSlider, musicVolumeSlider, uiVolumeSlider;
    [SerializeField] private CanvasGroup _canvasGroup, musicCanvas, experimentalCanvas;
    [SerializeField] private Button selectSoundButton, selectExperimentalButton, dumpButton, arduinoButton, cubeButton;

    //sliders and buttons
    private int index = 0;
    private float targetSliderValue;
    private bool updateValue;

    private MainMenuSounds _sounds;

    private void Awake()
    {
        _sliders = new List<Slider>();
    }

    void Start()
    {
        _sounds = FindObjectOfType<MainMenuSounds>();
        soundManager = FindObjectOfType<SoundManager>();
        _jsoNsaving = FindObjectOfType<JSONsaving>();
        _loadScreen = FindObjectOfType<LoadScreen>();

        _sliders.AddRange(new[] { masterVolumeSlider, vfxVolumeSlider, musicVolumeSlider, uiVolumeSlider });
        //previus game sound values
        masterVolumeSlider.value = GetGameDataSettings().GetMasterVolume();
        soundManager.SetMasterVolume(GetGameDataSettings().GetMasterVolume());
        vfxVolumeSlider.value = GetGameDataSettings().GetVfxVolume();
        soundManager.SetVfxVolume(GetGameDataSettings().GetVfxVolume());
        musicVolumeSlider.value = GetGameDataSettings().GetMusicVolume();
        soundManager.SetMusicVolume(GetGameDataSettings().GetMusicVolume());
        uiVolumeSlider.value = GetGameDataSettings().GetUiVolume();
        soundManager.SetUiVolume(GetGameDataSettings().GetUiVolume());
        //sliders
        masterVolumeSlider.onValueChanged.AddListener(MasterSliderAction);
        vfxVolumeSlider.onValueChanged.AddListener(VfxSliderAction);
        musicVolumeSlider.onValueChanged.AddListener(MusicSliderAction);
        uiVolumeSlider.onValueChanged.AddListener(UiSliderAction);
        Highlight();
        //buttons
        selectSoundButton.onClick.AddListener(ShowMusic);
        selectExperimentalButton.onClick.AddListener(ShowExperimental);
        dumpButton.onClick.AddListener(() =>
        {
            _jsoNsaving.dumpData();
            _loadScreen.LoadMenu();
        });
        arduinoButton.onClick.AddListener(() => { _loadScreen.LoadArduinoConnect(); });
        cubeButton.onClick.AddListener(() => { _loadScreen.LoadCubeConexion(); });
        HideExperimental();
        ShowMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateValue)
            UpdatesliderValue();
    }

    public void ShowUI()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void HideUI()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void ShowMusic()
    {
        musicCanvas.alpha = 1;
        musicCanvas.interactable = true;
        musicCanvas.blocksRaycasts = true;
        HideExperimental();
    }

    public void HideMusic()
    {
        musicCanvas.alpha = 0;
        musicCanvas.interactable = false;
        musicCanvas.blocksRaycasts = false;
    }


    public void ShowExperimental()
    {
        experimentalCanvas.alpha = 1;
        experimentalCanvas.interactable = true;
        experimentalCanvas.blocksRaycasts = true;
        HideMusic();
    }

    public void HideExperimental()
    {
        experimentalCanvas.alpha = 0;
        experimentalCanvas.interactable = false;
        experimentalCanvas.blocksRaycasts = false;
    }

    #region Sliders

    private void MasterSliderAction(float value)
    {
        updateValue = false;

        index = 0;
        Highlight();
        soundManager.SetMasterVolume(value);
        if (GameDataExists())
        {
            GetGameDataSettings().SetMasterVolume(value);
            _jsoNsaving.SaveTheData();
        }
    }

    private void VfxSliderAction(float value)
    {
        updateValue = false;

        index = 1;
        Highlight();
        soundManager.SetVfxVolume(value);
        if (GameDataExists())
        {
            GetGameDataSettings().SetVfxVolume(value);
            _jsoNsaving.SaveTheData();
        }
    }

    private void MusicSliderAction(float value)
    {
        updateValue = false;

        index = 2;
        Highlight();
        soundManager.SetMusicVolume(value);
        if (GameDataExists())
        {
            GetGameDataSettings().SetMusicVolume(value);
            _jsoNsaving.SaveTheData();
        }
    }

    private void UiSliderAction(float value)
    {
        updateValue = false;
        index = 3;
        Highlight();
        soundManager.SetUiVolume(value);
        if (GameDataExists())
        {
            GetGameDataSettings().SetUiVolume(value);
            _jsoNsaving.SaveTheData();
        }
    }

    public void SelectNext()
    {
        updateValue = false;
        _sounds.ChangeOptionSound();
        index = (index + 1) % (_sliders.Count);
        Highlight();
        updateValue = false;
    }

    public void SelectPrev()
    {
        updateValue = false;
        _sounds.ChangeOptionSound();
        index = index - 1 < 0 ? _sliders.Count - 1 : index - 1;
        Highlight();
        updateValue = false;
    }

    private void Highlight()
    {
        for (var i = 0; i < _sliders.Count; i++)
        {
            var block = _sliders[i].colors;
            if (i == index)
                block.normalColor = Color.white;
            else
                block.normalColor = Color.black;
            _sliders[i].colors = block;
        }
    }


    public void IncreaseValue()
    {
        if (index < _sliders.Count)
        {
            updateValue = true;
            targetSliderValue = Mathf.Min(1, _sliders[index].value + 0.05f);
        }
    }

    public void DecreaseValue()
    {
        if (index < _sliders.Count)
        {
            updateValue = true;
            targetSliderValue = Mathf.Max(0, _sliders[index].value - 0.05f);
        }
    }

    private void UpdatesliderValue()
    {
        _sliders[index].value = Mathf.MoveTowards(_sliders[index].value, targetSliderValue, 2 * Time.deltaTime);
        if (Mathf.Abs(targetSliderValue - _sliders[index].value) <= 0.01f)
        {
            updateValue = false;
        }
    }

    #endregion


    private bool GameDataExists()
    {
        int lastGameIndex = _jsoNsaving._saveData.GetLastSessionSlotIndex();
        if (lastGameIndex != -1)
        {
            return true;
        }

        return false;
    }

    private GameData GetGameDataSettings()
    {
        return _jsoNsaving._saveData.GetGameData(_jsoNsaving._saveData.GetLastSessionSlotIndex());
    }
}