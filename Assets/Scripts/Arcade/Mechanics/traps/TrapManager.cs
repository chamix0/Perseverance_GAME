using System.Collections;
using System.Collections.Generic;
using Arcade.Mechanics.Doors;
using Player.Observer_pattern;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class TrapManager : MonoBehaviour
{
    private List<LaserEnemy> lasers;

    private BoxCollider trigger;

    private ArcadePlayerData _playerData;
    private GuiManager _guiManager;
    private bool _isIn, _lasersOn;

    //values
    [SerializeField] private int prize;

    void Start()
    {
        lasers = new List<LaserEnemy>(GetComponentsInChildren<LaserEnemy>());
        trigger = GetComponent<BoxCollider>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerData = FindObjectOfType<ArcadePlayerData>();
    }

    private void StartLasers()
    {
        foreach (var laser in lasers)
            laser.TurnOnLaser();
    }

    private void StopLasers()
    {
        foreach (var laser in lasers)
            laser.TurnOffLaser();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isIn = true;
            _guiManager.ShowMessage();
            if (_playerData.GetPower())
            {
                if (CanPay())
                    _guiManager.SetMessageText_("Press E to enable trap for the next 30 seconds. \n <color=#0d9146>" +
                                                prize + " pts</color>");
                else
                    _guiManager.SetMessageText_("Press E to enable trap for the next 30 seconds. \n <color=#911f0d>" +
                                                prize + "  pts</color>");
            }
            else
            {
                _guiManager.SetMessageText_("Turn on the POWER to  use the trap.");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isIn && !_lasersOn && CanPay() && _playerData.GetPower())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(TurnOnLaserCoroutine());
                _playerData.RemovePoints(prize);
                _isIn = false;
                _guiManager.HideMessage();
                trigger.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isIn = false;
            _guiManager.HideMessage();
        }
    }

    private bool CanPay()
    {
        return _playerData.GetPoints() >= prize;
    }

    private IEnumerator TurnOnLaserCoroutine()
    {
        _lasersOn = true;
        yield return new WaitForSeconds(10);
        StartLasers();
        yield return new WaitForSeconds(30);
        StopLasers();
        trigger.enabled = true;
        _lasersOn = false;
    }
}