using UnityEngine;

public class PlaySound : MonoBehaviour
{
    // Start is called before the first frame update
    private MinigameSoundManager soundManager;
    void Start()
    {
        soundManager = FindObjectOfType<MinigameSoundManager>();
    }

    public void PlayHitSound()
    {
        soundManager.PlayHitSound();
    }
    public void PlayDefendSound()
    {
        soundManager.PlayDefendSound();
    }

    public void PlayHurtSound()
    {
        soundManager.PlayHurtSound();
    }

    public void PlayMissSound()
    {
        soundManager.PlayMissSound();

    }
}
