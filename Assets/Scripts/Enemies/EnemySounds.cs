using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySounds : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySearchingSound()
    {
        audioSource.clip = clips[0];
        audioSource.Play();
    }

    public void PlayAlertSound()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
    }

    public void PlayDieSound()
    {
        audioSource.clip = clips[2];
        audioSource.Play();
    }
    public void PlayHurtSound()
    {
        audioSource.clip = clips[3];
        audioSource.Play();
    }
    // Update is called once per frame
    void Update()
    {
    }
}