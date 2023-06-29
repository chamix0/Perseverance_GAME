using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienSoundTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool DeleteAfter;
    [SerializeField] private bool queueMusic;
    [SerializeField] private AmbienSoundClip _clip;
    private AmbientSound _ambientSound;
    
    void Start()
    {
        _ambientSound = FindObjectOfType<AmbientSound>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (queueMusic)
                _ambientSound.QueueSound(_clip);
            else
                _ambientSound.ChangeSound(_clip);

            if (DeleteAfter)
                gameObject.SetActive(false);

        }
    }
}
