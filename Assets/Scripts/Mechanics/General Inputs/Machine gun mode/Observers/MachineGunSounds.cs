using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MachineGunSounds : MonoBehaviour, IObserver
{
    [SerializeField] private List<AudioClip> clips;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Aim)

        {
            audioSource.clip = clips[1];
            audioSource.Play();
        }
        else if (playerAction is PlayerActions.ChangeShootingMode)
        {
            audioSource.clip = clips[0];
            audioSource.Play();
        }
    }
}