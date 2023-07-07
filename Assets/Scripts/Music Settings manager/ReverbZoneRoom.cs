using UnityEngine;

public class ReverbZoneRoom : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioReverbZone _reverbZone;
    void Start()
    {
        _reverbZone = GetComponent<AudioReverbZone>();
        _reverbZone.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _reverbZone.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _reverbZone.enabled = false;
        }
    }
}
