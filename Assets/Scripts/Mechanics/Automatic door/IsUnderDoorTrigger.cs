using UnityEngine;

public class IsUnderDoorTrigger : MonoBehaviour
{
    //components
    private DoorManager _doorBase;

    void Start()
    {
        _doorBase = transform.parent.GetComponentInChildren<DoorManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            _doorBase.OpenDoor();
            _doorBase._inside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            _doorBase._inside = false;
        }
    }
}