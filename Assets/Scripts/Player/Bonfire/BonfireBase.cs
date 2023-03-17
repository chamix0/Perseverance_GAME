using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger cameraChanger;
    private CameraController _cameraController;
    private PlayerAnimations _playerAnimations;
    private GameObject door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private GenericScreenUi _genericScreenUi;

    //variables
    private bool  _inside;


    //values
    private const float OpenY = 5;

    //lists
    
    void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _playerValues = FindObjectOfType<PlayerValues>();
    }
    
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")  && !_inside)
        {
            _inside = true;
            var eulerAngles = _snapPos.transform.eulerAngles;
            _playerValues.snapRotationTo(eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _cameraController.RotateXCustom(MyUtils.Clamp0360(-eulerAngles.y));
            _cameraController.RotateYCustom(0.5f);
            _cameraController.FreezeCamera();
            _playerValues.Sit();
            cameraChanger.SetScreenCamera();
            //mostrar mensaje
            StartCoroutine(ShowMessageBonfire());
        }
    }



    IEnumerator ShowMessageBonfire()
    {
        //Checkpoint
        _genericScreenUi.SetText("Congratulations you reached a checkpoint!");
        _genericScreenUi.FadeInText();
        yield return new WaitForSeconds(5f);
        _genericScreenUi.FadeOutText();
        
        //save game
        
        //recharging simulations
        
        
        
        
        cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(false, 2.5f);
        yield return new WaitForSeconds(2.5f);
        _playerValues.SetGear(0);
        yield return new WaitForSeconds(3f);
        _playerValues.StopMovement();
        _cameraController.UnFreezeCamera();
        _playerValues.SetInputsEnabled(true);
        _inside = false;
        yield return new WaitForSeconds(2f);
    }
}
