using System;
using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

public class RumbleObserver : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private RumbleManager _rumbleManager;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        _rumbleManager = FindObjectOfType<RumbleManager>();
        playerValues.AddObserver(this);
    }


    public void OnNotify(PlayerActions playerAction)
    {
        switch (playerAction)
        {
            case PlayerActions.Stomp:
                _rumbleManager.RumblePulse(0.223f, 0.434f, 0.3f);
                break;
            case PlayerActions.Shoot:
                _rumbleManager.RumblePulse(0.223f, 0.334f, 0.2f);
                break;
            case PlayerActions.Die:
                _rumbleManager.RumblePulse(0.3f, 0.4f, 0.5f);
                break;
            case PlayerActions.GrenadeExplode:
                _rumbleManager.RumblePulse(0.3f, 0.5f, 0.7f);
                break;
            case PlayerActions.Damage:
                _rumbleManager.RumblePulse(0.223f, 0.334f, 0.1f);
                break;
        }
    }
}