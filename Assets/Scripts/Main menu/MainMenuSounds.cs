using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuSounds : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _audioClips;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }



    public void ChangeOptionSound()
    {
        _audioSource.clip = _audioClips[0];
        _audioSource.Play();
    }

    public void SelectOptionSound()
    {
        _audioSource.clip = _audioClips[1];
        _audioSource.Play();
    }

    public void ReturnSound()
    {
        _audioSource.clip = _audioClips[2];
        _audioSource.Play();
    }
}