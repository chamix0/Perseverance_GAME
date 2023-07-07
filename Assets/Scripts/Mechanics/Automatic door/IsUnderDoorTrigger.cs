using UnityEngine;

public class IsUnderDoorTrigger : MonoBehaviour
{
    //components
    private DoorManager _doorBase;
    private bool playerIn;
    private bool enemyIn;

    void Start()
    {
        _doorBase = transform.parent.GetComponentInChildren<DoorManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _doorBase.OpenDoor();
            _doorBase._inside = true;
            playerIn = true;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            _doorBase.OpenDoor();
            _doorBase._inside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            if (!enemyIn)
            {
                _doorBase._inside = false;
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyIn = false;
            if (!playerIn)
            {
                _doorBase._inside = false;
            }
        }
    }
}