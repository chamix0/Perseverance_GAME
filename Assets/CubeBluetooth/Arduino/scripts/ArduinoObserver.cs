using System;
using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

public class ArduinoObserver : MonoBehaviour, IObserver
{
    private PlayerValues _playerValues;
    private ArduinoManager _arduinoManager;
    private PauseManager _pauseManager;
    private MachineGun _machineGun;
    private bool pauseDisplayed, damage;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _pauseManager = FindObjectOfType<PauseManager>();
        _machineGun = FindObjectOfType<MachineGun>();
        try
        {
            _arduinoManager = FindObjectOfType<ArduinoManager>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        if (_arduinoManager)
        {
            _playerValues.AddObserver(this);
            _machineGun.AddObserver(this);
        }
        else
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_arduinoManager.GetShake())
        {
            ShakeAction();
        }

        if (_playerValues.GetPaused())
        {
            if (!pauseDisplayed)
            {
                pauseDisplayed = true;
                _arduinoManager.SetPauseIcon();
            }
        }
        else
        {
            pauseDisplayed = false;
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (!_playerValues.GetPaused())
        {
            //vibration
            if (playerAction is PlayerActions.Shoot)
                StartCoroutine(ShootCoroutine());
            else if (playerAction is PlayerActions.Stomp)
                StartCoroutine(StompCoroutine());
            else if (playerAction is PlayerActions.MinigameFinished)
                StartCoroutine(StompCoroutine());
            else if (playerAction is PlayerActions.NoDamage)
            {
                StopAllCoroutines();
                _arduinoManager.StopVibration();
            }
            else if (playerAction is PlayerActions.LowDamage)
            {
                StopAllCoroutines();
                StartCoroutine(LowDamageCoroutine());
            }
            else if (playerAction is PlayerActions.MediumDamage)
            {
                StopAllCoroutines();
                StartCoroutine(MediumDamageCoroutine());
            }
            else if (playerAction is PlayerActions.HighDamage)
            {
                StopAllCoroutines();
                _arduinoManager.StartVibration();
            }
            //faces
            else if (playerAction is PlayerActions.BlinkFace)
                _arduinoManager.SetBlinkFace();
            else if (playerAction is PlayerActions.NormalFace)
                _arduinoManager.SetNormalFace();
            else if (playerAction is PlayerActions.Die)
                _arduinoManager.SetDeadFace();
            else if (playerAction is PlayerActions.ScaredFace)
                _arduinoManager.SetScaredFace();
            else if (playerAction is PlayerActions.TurnOnLights or PlayerActions.TurnOffLights)
                _arduinoManager.SetLights();
            //minigames
            else if (playerAction is PlayerActions.Asteroids)
                _arduinoManager.SetAsteroids();
            else if (playerAction is PlayerActions.ColorsMinigame)
                _arduinoManager.SetColorMinigame();
            else if (playerAction is PlayerActions.MemorizeMinigame)
                _arduinoManager.SetMemorizeMinigame();
            else if (playerAction is PlayerActions.JustWaitMinigame)
                _arduinoManager.SetWaitMinigame();
            else if (playerAction is PlayerActions.DontTouchTheWallsMinigame)
                _arduinoManager.SetWallsMinigame();
            else if (playerAction is PlayerActions.RollMinigame)
                _arduinoManager.SetRollMinigame();
            else if (playerAction is PlayerActions.PushFastMinigame)
                _arduinoManager.SetPushMinigame();
            else if (playerAction is PlayerActions.AdjustValuesMinigame)
                _arduinoManager.SetAdjustMinigame();
        }
    }

    void ShakeAction()
    {
        _arduinoManager.SetShakeToFalse();
        _pauseManager.Pause();
        _arduinoManager.SetPauseIcon();
    }

    IEnumerator ShootCoroutine()
    {
        _arduinoManager.StartVibration();
        yield return new WaitForSeconds(0.2f);
        _arduinoManager.StopVibration();
    }

    IEnumerator StompCoroutine()
    {
        _arduinoManager.StartVibration();
        yield return new WaitForSeconds(0.3f);
        _arduinoManager.StopVibration();
    }

    IEnumerator LowDamageCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(1);
            _arduinoManager.StartVibration();
            yield return new WaitForSeconds(0.1f);
            _arduinoManager.StopVibration();
        } while (true);
    }

    IEnumerator MediumDamageCoroutine()
    {
        do
        {
            _arduinoManager.StartVibration();
            yield return new WaitForSeconds(0.5f);
            _arduinoManager.StopVibration();
        } while (true);
    }
}