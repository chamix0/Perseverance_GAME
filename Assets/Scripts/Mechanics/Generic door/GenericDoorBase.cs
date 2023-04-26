using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDoorBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    [SerializeField] private GameObject _snapPos;
    private CameraChanger cameraChanger;
    private OrbitCameraController _cameraController;
    private GameObject door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private GenericScreenUi _genericScreenUi;

    //variables
    private bool _sealDoor, _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY, _openY;

    //values


    void Start()
    {
        _cameraController = FindObjectOfType<OrbitCameraController>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        var parent = transform.parent;
        cameraChanger = FindObjectOfType<CameraChanger>();
        door = parent.Find("Door").gameObject;
        _closeY = door.transform.position.y;
        _openY = _closeY + 5;

        _playerValues = FindObjectOfType<PlayerValues>();
    }

    private void Update()
    {
        if (_openDoor)
        {
            if (door.transform.position.y < _openY)
                door.transform.position += new Vector3(0, 0.1f, 0);
            else
                _openDoor = false;
        }

        if (_closeDoor)
        {
            if (door.transform.position.y > _closeY)
                door.transform.position -= new Vector3(0, 0.1f, 0);
            else
                _closeDoor = false;
        }
    }

    private void OpenDoor()
    {
        _openDoor = true;
        _closeDoor = false;
    }

    public void CloseDoor()
    {
        _openDoor = false;
        _closeDoor = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_sealDoor && !_inside)
        {
            _inside = true;
            var eulerAngles = _snapPos.transform.eulerAngles;
            _playerValues.snapRotationTo(eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            // _cameraController.RotateXCustom(MyUtils.Clamp0360(eulerAngles.y + 180));
            // _cameraController.RotateYCustom(0.5f);
            _cameraController.FreezeCamera();
            _playerValues.Sit();
            cameraChanger.SetScreenCamera();
            //mostrar mensaje
            StartCoroutine(ShowMessageOpenDoor());
        }
    }


    IEnumerator ShowMessageOpenDoor()
    {
        string msg = "Opening door";


        _genericScreenUi.SetText(msg);
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(5f);
        _genericScreenUi.FadeOutText();
        cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(false, 2.5f);
        OpenDoor();
        yield return new WaitForSeconds(2.5f);
        _playerValues.SetGear(0);
        yield return new WaitForSeconds(3f);
        _playerValues.StopMovement();
        _cameraController.UnFreezeCamera();
        _playerValues.SetInputsEnabled(true);
        _inside = false;
        _sealDoor = true;
    }
}