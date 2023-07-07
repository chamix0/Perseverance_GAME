using UnityEngine;

public class StartZone : MonoBehaviour
{
    private TimeZoneManager _timeZoneManager;

    private void Start()
    {
        _timeZoneManager = transform.parent.GetComponent<TimeZoneManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _timeZoneManager.StartRun();
            enabled = false;
        }
    }
}