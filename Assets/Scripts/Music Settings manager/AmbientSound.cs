using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmbienSoundClip
{
    Normal,
    Nature,
    Freezer,
    AfterBossFight,
    BossFight,
    Rooms,
    RandomMusic
}

public class AmbientSound : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> clips;
    private bool fadeOut = false, fadeIn = false;
    [SerializeField] private float fadeSpeed = 1f;
    private AmbienSoundClip currentClip;

    void Start()
    {
        _audioSource.clip = SelectClip(AmbienSoundClip.Normal);
        _audioSource.Play();
        currentClip = AmbienSoundClip.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (_audioSource.loop && GetRemeaningTimeOfClip() < 0.1f)
        {
            ChangeSound(currentClip);
            _audioSource.loop = true;
            _audioSource.Play();
        }

        if (fadeOut)
        {
            FadeOut();
        }

        if (fadeIn)
        {
            FadeIn();
        }
    }

    public float GetRemeaningTimeOfClip()
    {
        return _audioSource.clip.length - _audioSource.time;
    }

    public void ChangeSound(AmbienSoundClip clip)
    {
        if (clip != currentClip)
        {
            currentClip = clip;
            StartCoroutine(ChangeClipCoroutine(SelectClip(clip)));
        }
    }

    public void QueueSound(AmbienSoundClip clip)
    {
        if (currentClip != clip)
        {
            // currentClip = clip;
            StartCoroutine(ChangeClipCoroutine(SelectClip(clip)));
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }

    private AudioClip SelectClip(AmbienSoundClip ambienSoundClip)
    {
        AudioClip selectedClip;
        switch (ambienSoundClip)
        {
            case AmbienSoundClip.Normal:
                selectedClip = clips[0];
                break;
            case AmbienSoundClip.Nature:
                selectedClip = clips[1];
                break;
            case AmbienSoundClip.Freezer:
                selectedClip = clips[2];
                break;
            case AmbienSoundClip.Rooms:
                selectedClip = clips[6];
                break;
            case AmbienSoundClip.RandomMusic:
                selectedClip = clips[4];
                break;
            case AmbienSoundClip.AfterBossFight:
                selectedClip = clips[3];
                break;
            case AmbienSoundClip.BossFight:
                selectedClip = clips[5];
                break;
            default:
                selectedClip = clips[0];
                break;
        }

        return selectedClip;
    }

    private void FadeOut()
    {
        _audioSource.volume = Mathf.Max(0, _audioSource.volume - fadeSpeed * Time.deltaTime);
        if (_audioSource.volume <= 0)
        {
            fadeOut = false;
        }
    }

    private void FadeIn()
    {
        if (!fadeOut)
        {
            _audioSource.volume = Mathf.Min(1, _audioSource.volume + fadeSpeed * Time.deltaTime);
            if (_audioSource.volume >= 1)
            {
                fadeIn = false;
            }
        }
    }

    IEnumerator ChangeClipCoroutine(AudioClip clip)
    {
        fadeOut = true;
        yield return new WaitUntil(() => !fadeOut);
        _audioSource.clip = clip;
        _audioSource.Play();
        fadeIn = true;
    }
}