using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSounds : MonoBehaviour
{
    [SerializeField] private AudioSource instantSounds;
    [SerializeField] private List<AudioClip> clips;
    

    public void PlaySelect()
    {
        instantSounds.clip = clips[0];
        instantSounds.Play();
    }

    public void PlayChange()
    {
        instantSounds.clip = clips[1];
        instantSounds.Play();
    }
}
