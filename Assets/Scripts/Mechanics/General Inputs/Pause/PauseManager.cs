using System;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GuiManager guiManager;
    private List<Move> pauseSecuence;
    private int sequenceIndex;
    private PlayerValues playerValues;
    private PauseSounds pauseSounds;
    private CameraController cameraController;
    private LoadScreen loadScreen;
    private CubeConectionManager cubeConectionManager;

    //settings
    private List<Slider> _sliders;
    private SoundManager soundManager;
    [SerializeField] private Slider masterVolumeSlider, vfxVolumeSlider, musicVolumeSlider, uiVolumeSlider;
    [SerializeField] private Button continueButon, exitButton, reconectButton;

    //sliders and buttons
    private int index = 0;
    private float targetSliderValue;
    private float tX;
    private bool updateValue;

    private void Awake()
    {
        _sliders = new List<Slider>();
    }

    void Start()
    {
        loadScreen = FindObjectOfType<LoadScreen>();
        soundManager = FindObjectOfType<SoundManager>();
        pauseSounds = FindObjectOfType<PauseSounds>();
        playerValues = FindObjectOfType<PlayerValues>();
        guiManager = FindObjectOfType<GuiManager>();
        cameraController = FindObjectOfType<CameraController>();
        try
        {
            cubeConectionManager = FindObjectOfType<CubeConectionManager>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        pauseSecuence = new List<Move>();
        SetSequence();
        continueButon.onClick.AddListener(() =>
        {
            Pause();
            pauseSounds.PlaySelect();
        });
        exitButton.onClick.AddListener(() => ExitButtonAction());
        if (cubeConectionManager)
            reconectButton.onClick.AddListener(() => cubeConectionManager.ReEstablish());
        else reconectButton.gameObject.SetActive(false);

        _sliders.AddRange(new[] { masterVolumeSlider, vfxVolumeSlider, musicVolumeSlider, uiVolumeSlider });
        //previus game sound values
        if (!playerValues.gameData.GetIsNewGame())
        {
            masterVolumeSlider.value = playerValues.gameData.GetMasterVolume();
            soundManager.SetMasterVolume(playerValues.gameData.GetMasterVolume());
            vfxVolumeSlider.value = playerValues.gameData.GetVfxVolume();
            soundManager.SetVfxVolume(playerValues.gameData.GetVfxVolume());
            musicVolumeSlider.value = playerValues.gameData.GetMusicVolume();
            soundManager.SetMusicVolume(playerValues.gameData.GetMusicVolume());
            uiVolumeSlider.value = playerValues.gameData.GetUiVolume();
            soundManager.SetUiVolume(playerValues.gameData.GetUiVolume());
        }

        masterVolumeSlider.onValueChanged.AddListener(MasterSliderAction);
        vfxVolumeSlider.onValueChanged.AddListener(VfxSliderAction);
        musicVolumeSlider.onValueChanged.AddListener(MusicSliderAction);
        uiVolumeSlider.onValueChanged.AddListener(UiSliderAction);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateValue)
            UpdatesliderValue();
    }

    private void SetSequence()
    {
        pauseSecuence.AddRange(new[]
        {
            new Move(FACES.R, 1),
            new Move(FACES.U, 1),
            new Move(FACES.R, -1),
            new Move(FACES.U, -1),
            new Move(FACES.R, -1),
            new Move(FACES.F, 1),
            new Move(FACES.R, 1),
            new Move(FACES.R, 1),
            new Move(FACES.U, -1),
            new Move(FACES.R, -1),
            new Move(FACES.U, -1),
            new Move(FACES.R, 1),
            new Move(FACES.U, 1),
            new Move(FACES.R, -1),
            new Move(FACES.F, -1)
        });
    }

    public void Pause()
    {
        playerValues.SetPaused(!playerValues.GetPaused());

        if (playerValues.GetPaused())
        {
            pauseSounds.PlayChange();
            cameraController.FreezeCamera();
            guiManager.ShowPausePanel();
            if (!guiManager.GetObjetiveText().Equals(""))
                guiManager.ShowObjetives();
            Time.timeScale = 0;
            guiManager.SetTutorial(" ");
            soundManager.SetVfxVolume(0);

            //cursor
            CursorManager.ShowCursor();
        }
        else
        {
            cameraController.UnFreezeCamera();
            guiManager.HidePausePanel();
            guiManager.HideObjetives();
            Time.timeScale = 1;
            soundManager.SetVfxVolume(playerValues.gameData.GetVfxVolume());
            CursorManager.HideCursor();
        }
    }

    public void ExitButtonAction()
    {
        pauseSounds.PlaySelect();
        playerValues.SetCurrentInput(CurrentInput.None);
        Time.timeScale = 1;
        guiManager.HideGui();
        loadScreen.LoadMenu();
    }

    public bool CheckCubePause(Move move)
    {
        if (!playerValues.GetPaused())
        {
            guiManager.ShowPauseIndicator();
            if (move.Equals(pauseSecuence[sequenceIndex]))
            {
                if (sequenceIndex == pauseSecuence.Count - 1)
                {
                    guiManager.HidePauseIndicator();
                    sequenceIndex = 0;
                    return true;
                }

                sequenceIndex++;
                guiManager.SetNextMoveText(pauseSecuence[sequenceIndex].ToString());
                guiManager.FillPauseSlider((float)sequenceIndex / (pauseSecuence.Count - 1));
                return false;
            }

            sequenceIndex = 0;
            guiManager.SetNextMoveText(pauseSecuence[sequenceIndex].ToString());
            guiManager.FillPauseSlider(0);
            return false;
        }

        return false;
    }

    private void MasterSliderAction(float value)
    {
        soundManager.SetMasterVolume(value);
        playerValues.gameData.SetMasterVolume(value);
        playerValues.SaveGame();
    }

    private void VfxSliderAction(float value)
    {
        soundManager.SetVfxVolume(value);
        playerValues.gameData.SetVfxVolume(value);
        playerValues.SaveGame();
    }

    private void MusicSliderAction(float value)
    {
        soundManager.SetMusicVolume(value);
        playerValues.gameData.SetMusicVolume(value);
        playerValues.SaveGame();
    }

    private void UiSliderAction(float value)
    {
        soundManager.SetUiVolume(value);
        playerValues.gameData.SetUiVolume(value);
        playerValues.SaveGame();
    }

    public void SelectNext()
    {
        index = (index + 1) % (_sliders.Count + 2);
        Highlight();
        updateValue = false;
        pauseSounds.PlayChange();
    }

    public void SelectPrev()
    {
        index = index - 1 < 0 ? _sliders.Count + 1 : index - 1;
        Highlight();
        updateValue = false;
        pauseSounds.PlayChange();
    }

    private void Highlight()
    {
        if (index < _sliders.Count)
        {
            for (var i = 0; i < _sliders.Count; i++)
            {
                var block = _sliders[i].colors;
                if (i == index)
                    block.normalColor = Color.green;
                else
                    block.normalColor = Color.gray;
                _sliders[i].colors = block;
            }

            DeSelectExitButton();
            DeSelectContinueButton();
        }
        else if (index == _sliders.Count)
        {
            DeselectSliders();
            SelectExitButton();
            DeSelectContinueButton();
        }
        else
        {
            DeselectSliders();
            DeSelectExitButton();
            SelectContinueButton();
        }
    }

    private void DeselectSliders()
    {
        for (var i = 0; i < _sliders.Count; i++)
        {
            var block = _sliders[i].colors;
            block.normalColor = Color.gray;
            _sliders[i].colors = block;
        }
    }

    private void SelectContinueButton()
    {
        var blockCon = continueButon.colors;
        blockCon.normalColor = Color.gray;
        continueButon.colors = blockCon;
    }

    private void DeSelectContinueButton()
    {
        var blockCon = continueButon.colors;
        blockCon.normalColor = Color.green;
        continueButon.colors = blockCon;
    }

    private void SelectExitButton()
    {
        var blockEx = exitButton.colors;
        blockEx.normalColor = Color.gray;
        exitButton.colors = blockEx;
    }

    private void DeSelectExitButton()
    {
        var blockEx = exitButton.colors;
        blockEx.normalColor = Color.green;
        exitButton.colors = blockEx;
    }

    public void Confirm()
    {
        if (index == _sliders.Count)
        {
            ExitButtonAction();
        }
        else if (index == _sliders.Count + 1)
        {
            Pause();
        }
    }

    public void IncreaseValue()
    {
        if (index < _sliders.Count)
        {
            tX = 0f;
            updateValue = true;
            targetSliderValue = Mathf.Min(1, _sliders[index].value + 0.05f);
        }
    }

    public void DecreaseValue()
    {
        if (index < _sliders.Count)
        {
            tX = 0f;
            updateValue = true;
            targetSliderValue = Mathf.Max(0, _sliders[index].value - 0.05f);
        }
    }

    private void UpdatesliderValue()
    {
        _sliders[index].value = Mathf.Lerp(_sliders[index].value, targetSliderValue, tX);
        tX += 0.5f * Time.unscaledDeltaTime;
        if (tX > 1.0f)
        {
            tX = 1.0f;
            updateValue = false;
        }
    }
}