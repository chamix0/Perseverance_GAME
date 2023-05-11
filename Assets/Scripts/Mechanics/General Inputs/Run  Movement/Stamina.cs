using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Color greenColor, recoverColor;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool staminaChanged;
    public bool beingShown;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        staminaChanged = playerValues.allStaminaUsed;
        fill.material.SetColor(BackgroundColor, playerValues.allStaminaUsed ? recoverColor : greenColor);
        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (beingShown)
        {
            if (playerValues.allStaminaUsed != staminaChanged)
            {
                fill.material.SetColor(BackgroundColor, playerValues.allStaminaUsed ? recoverColor : greenColor);
                staminaChanged = playerValues.allStaminaUsed;
            }

            slider.value = playerValues.stamina / 100;
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