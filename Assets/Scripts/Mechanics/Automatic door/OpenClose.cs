using UnityEngine;

enum DoorMode
{
    Open,
    Close,
    Automatic
}

public class OpenClose : MonoBehaviour
{
    //componentsw
    private DoorManager _doorBase;
    [SerializeField] private DoorMode doorMode;

    void Start()
    {
        _doorBase = transform.parent.GetComponentInChildren<DoorManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")||other.gameObject.CompareTag("Enemy"))
        {
            switch (doorMode)
            {
                case DoorMode.Open:
                    _doorBase.OpenDoor();
                    break;
                case DoorMode.Close:
                    _doorBase.CloseDoor();
                    break;
                case DoorMode.Automatic:
                    if (_doorBase.opened)
                    {
                        _doorBase.CloseDoor();
                    }
                    else
                    {
                        _doorBase.OpenDoor();
                    }

                    break;
            }
        }
    }
}