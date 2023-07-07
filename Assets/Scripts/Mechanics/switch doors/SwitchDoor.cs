using UnityEngine;

public class SwitchDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public bool redBlue;
    private DoorManager _doorManager;

    void Start()
    {
        _doorManager = GetComponent<DoorManager>();
    }
    
    // Update is called once per frame
    public void OpenDoor()
    {
      _doorManager.OpenDoor();
    }

    public void CloseDoor()
    {
      _doorManager.CloseDoor();
    }
}