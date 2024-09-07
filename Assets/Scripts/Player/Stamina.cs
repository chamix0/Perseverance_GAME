using Mechanics.General_Inputs;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Color greenColor, recoverColor;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool _staminaChanged;
    public bool beingShown;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _staminaChanged = _playerValues.allStaminaUsed;

        fill.color = _playerValues.allStaminaUsed ? recoverColor : greenColor;
        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //show stamina bar
        CurrentInput currentInput = _playerValues.GetCurrentInput();
        if (currentInput is CurrentInput.RaceMovement && !beingShown)
        {
            ShowStamina();
        }
        
        if (beingShown)
        {
            if (_playerValues.allStaminaUsed != _staminaChanged)
            {
                fill.color= _playerValues.allStaminaUsed ? recoverColor : greenColor;
                _staminaChanged = _playerValues.allStaminaUsed;
            }

            slider.value = _playerValues.stamina / 100;
        }
    }

    public void HideStamina()
    {
        canvasGroup.alpha = 0;
        beingShown = false;
    }

    public void ShowStamina()
    {
        canvasGroup.alpha = 1;
        beingShown = true;
    }
}