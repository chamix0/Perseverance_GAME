using System.Collections;
using Arcade.Mechanics.Bullets;
using Arcade.Mechanics.Granades;
using Mechanics.General_Inputs;
using UnityEngine;
[DefaultExecutionOrder(4)]
public class BulletShopBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private BoxCollider _boxCollider;
    public bool shopActive, isGrenade;


    //shop values
    private BulletShopManager shopManager;
    [SerializeField] private BulletType thisShopBulletType;
    [SerializeField] private GrenadeType thisShopGrenadeType;
    [SerializeField] int bulletPrize;

    //variables
    private bool _inside, shopping;


    void Start()
    {
        shopManager = FindObjectOfType<BulletShopManager>();
        _boxCollider = GetComponent<BoxCollider>();
        var parent = transform.parent;
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _playerValues = FindObjectOfType<PlayerValues>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && shopActive && !_inside && !shopping)
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
        shopManager.StartShop(thisShopBulletType, thisShopGrenadeType, this, bulletPrize);
        _cameraChanger.SetScreenCamera();
    }
    public void DeactivateBase()
    {
        shopActive = false;
        _boxCollider.enabled = false;
    }

    public void ActivateBase()
    {
        shopActive = true;
        _boxCollider.enabled = true;
    }

    public void ExitBase()
    {
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(true, 1f, CurrentInput.ArcadeMechanics);
        shopping = false;
    }
}