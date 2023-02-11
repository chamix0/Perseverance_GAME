using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicCameraMovementsGUI : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TMP_Text info;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        staminaSlider.value = _playerValues.stamina / 100;
        info.text = "Lights: " + _playerValues.lightsOn + "\n" +
                    "Can move: " + _playerValues.canMove + "\n" +
                    "is Grounded: " + _playerValues.isGrounded + "\n" +
                    "Gear: " + _playerValues.GetGear() + "\n";
    }
}