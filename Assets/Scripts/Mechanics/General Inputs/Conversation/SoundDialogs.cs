using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDialogs : MonoBehaviour
{
    [SerializeField] private AudioSource instantSounds, constantSounds;
    [SerializeField] private List<AudioClip> clips;


    public void PlayTypeing()
    {
        constantSounds.clip = clips[0];
        constantSounds.Play();
    }

    public void StopTypeing()
    {
        constantSounds.Stop();
    }

    public void PlaySelect()
    {
        instantSounds.clip = clips[1];
        instantSounds.Play();
    }

    public void PlayChange()
    {
        instantSounds.clip = clips[2];
        instantSounds.Play();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}