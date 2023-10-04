using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    private Gamepad _gamepad;

    public void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        _gamepad = Gamepad.current;
        if (_gamepad != null)
        {
            //start rumble
            _gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            //stop rumble
            StartCoroutine(StopRumbleCoroutine(duration));
        }
    }

    private IEnumerator StopRumbleCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        _gamepad.SetMotorSpeeds(0, 0);
    }
}