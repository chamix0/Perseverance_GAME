using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;
[DefaultExecutionOrder(4)]
public class PlayerLights : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    [SerializeField] List<Light> _lights;
    Material EarLightsColor;
    private PlayerValues playerValues;

    private Color offColor = new Color(0.172549024f, 3.24705887f, 0.188235298f, 0f);
    private Color onColor = new Color(0.748013675f, 12.9882374f, 0.816014946f, 0f);
    public float onIntensity = 20;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        foreach (var material in transform.Find("Boton 2").gameObject.GetComponent<Renderer>().materials)
        {
            if (material.name == "EAR LIGHTS (Instance)")
            {
                EarLightsColor = material;
                break;
            }
        }

       
        EarLightsColor.SetColor(EmissionColor, offColor);
         TurnOffLights();
    }

    public void TurnOnLights()
    {
        foreach (var light in _lights)
        {
            light.intensity = onIntensity;
        }

        playerValues.TurnOnLights();

        EarLightsColor.SetColor(EmissionColor, onColor);
    }

    public void TurnOffLights()
    {
        EarLightsColor.SetColor(EmissionColor, offColor);
        playerValues.TurnOffLights();
        foreach (var light in _lights)
        {
            light.intensity = 0;
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        switch (playerAction)
        {
            case PlayerActions.TurnOnLights:
                TurnOnLights();
                break;
            case PlayerActions.TurnOffLights:
                TurnOffLights();
                break;
        }
    }
}