using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private List<AudioClip> clips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayCorrectSound()
    {
        audioSource.clip = clips[0];
        audioSource.Play();
    }
    public void PlayInCorrectSound()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
    }

    public void PlayFinishedSound()
    {
        audioSource.clip = clips[2];
        audioSource.Play();
    }

    public void PlayClickSound()
    {
        audioSource.clip = clips[3];
        audioSource.Play();
    }
}