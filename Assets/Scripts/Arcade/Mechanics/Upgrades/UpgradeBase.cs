using System.Collections;
using System.Collections.Generic;
using Arcade.Mechanics.Doors;
using Mechanics.General_Inputs;
using UnityEngine;
[DefaultExecutionOrder(3)]
public class UpgradeBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private BoxCollider _collider;

    //shop values
    private UpgradeManager upgradeManager;
    [SerializeField] private bool isActive;
    [SerializeField] private ZonesArcade zone;

    //variables
    private bool _inside, shopping;


    void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        var parent = transform.parent;
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _playerValues = FindObjectOfType<PlayerValues>();
        _collider = GetComponent<BoxCollider>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActive  && !_inside && !shopping)
        {
            _inside = true;
            shopping = true;
            _playerValues.snapRotationTo(_snapPos.transform.eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _playerValues.Sit();
            StartCoroutine(ChangeCameraCoroutine());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && _inside && shopping)
        {
            _inside = false;
        }
    }

    IEnumerator ChangeCameraCoroutine()
    {
        yield return new WaitForSeconds(1f);
        upgradeManager.StartUpgrades(this);
        _cameraChanger.SetScreenCamera();
    }

    public void ExitBase()
    {
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(true, 1f, CurrentInput.ArcadeMechanics);
        shopping = false;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool val)
    {
        isActive = val;
    }
    

    public void DeactivateBase()
    {
        isActive = false;
        _collider.enabled = false;
    }

    public void ActivateBase()
    {
        isActive = true;
        _collider.enabled = true;
    }

    public ZonesArcade GetZone => zone;
}