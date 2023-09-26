using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioMixer audioMixer;
    private AudioMixerGroup _audioMixerGroup;
    private float auxVfxValue;

    void Start()
    {
        audioMixer.GetFloat("VFXVolume", out auxVfxValue);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetMasterVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.001f, 1);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetVfxVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.001f, 1);
        audioMixer.SetFloat("VFXVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.001f, 1);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetUiVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.001f, 1);
        audioMixer.SetFloat("UiVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMuteVFX(bool value)
    {
        if (value)
        {
            audioMixer.GetFloat("VFXVolume", out auxVfxValue);
            audioMixer.SetFloat("VFXVolume", 0);
        }
        else
        {
            audioMixer.SetFloat("VFXVolume", auxVfxValue);
        }
    }
}