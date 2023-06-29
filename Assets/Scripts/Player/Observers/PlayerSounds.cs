using System;
using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    [SerializeField] private AudioSource audioSourceInstantSounds, audioSourceContinuousSounds, audioSourceNoiseSounds;
    [SerializeField] private List<AudioClip> audioClips;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.RiseGear)
        {
            audioSourceInstantSounds.clip = audioClips[0];
            audioSourceInstantSounds.Play();
        }
        else if (playerAction is PlayerActions.DecreaseGear)
        {
            audioSourceInstantSounds.clip = audioClips[1];
            audioSourceInstantSounds.Play();
        }

        else if (playerAction is PlayerActions.Stop)
        {
            audioSourceContinuousSounds.Stop();
        }
        else if (playerAction is PlayerActions.RecuperateDistraction)
        {
            audioSourceInstantSounds.clip = audioClips[7];
            audioSourceInstantSounds.Play();
        }
        else if (playerAction is PlayerActions.SlowWalking)
        {
            audioSourceContinuousSounds.clip = audioClips[2];
            audioSourceContinuousSounds.Play();
        }
        else if (playerAction is PlayerActions.Walking)
        {
            audioSourceContinuousSounds.clip = audioClips[3];
            audioSourceContinuousSounds.Play();
        }
        else if (playerAction is PlayerActions.Runing)
        {
            audioSourceContinuousSounds.clip = audioClips[4];
            audioSourceContinuousSounds.Play();
        }
        else if (playerAction is PlayerActions.Sprint)
        {
            audioSourceContinuousSounds.clip = audioClips[11];
            audioSourceContinuousSounds.Play();
        }
        else if (playerAction is PlayerActions.ThrowDistraction)
        {
            audioSourceInstantSounds.clip = audioClips[6];
            audioSourceInstantSounds.Play();
        }
        else if (playerAction is PlayerActions.LowDamage)
        {
            audioSourceNoiseSounds.clip = audioClips[8];
            audioSourceNoiseSounds.Play();
        }
        else if (playerAction is PlayerActions.MediumDamage)
        {
            audioSourceNoiseSounds.clip = audioClips[9];
            audioSourceNoiseSounds.Play();
        }
        else if (playerAction is PlayerActions.HighDamage)
        {
            audioSourceNoiseSounds.clip = audioClips[10];
            audioSourceNoiseSounds.Play();
        }
        else if (playerAction is PlayerActions.NoDamage)
        {
            audioSourceNoiseSounds.Stop();
        }
        else if (playerAction is PlayerActions.Sit)
        {
            audioSourceInstantSounds.clip = audioClips[13];
            audioSourceInstantSounds.Play();
        }
        else if (playerAction is PlayerActions.StandUp)
        {
            audioSourceInstantSounds.clip = audioClips[12];
            audioSourceInstantSounds.Play();
        }
    }
}