using System.Collections;
using System.Collections.Generic;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicCameraMovementsGUI : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TMP_Text info;
    private MyInputManager _myInputManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _myInputManager = FindObjectOfType<MyInputManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        staminaSlider.value = _playerValues.stamina / 100;
        info.text = "Lights: " + _playerValues.GetLights() + "\n" +
                    "Can move: " + _playerValues.GetCanMove() + "\n" +
                    "is Grounded: " + _playerValues.GetIsGrounded() + "\n" +
                    "Gear: " + _playerValues.GetGear() + "\n" +
                    "Stucked: " + _playerValues.GetIsStucked() + "\n" +
                    "imputs enabled: " + _playerValues.GetInputsEnabled();
    }
}