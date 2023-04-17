using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using UnityEngine;

public class InputModeChangerTrigger : MonoBehaviour
{
    private PlayerValues _playerValues;
    public CurrentInput _currentInput;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerValues.SetCurrentInput(_currentInput);
        }
    }
}
