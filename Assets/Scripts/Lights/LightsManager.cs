using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct LightValue
{
    private readonly Light light;
    private readonly float intensity;
    private readonly Material[] mats;
    private static readonly int Alpha = Shader.PropertyToID("_alpha");

    public LightValue(Light light, float intensity)
    {
        this.light = light;
        this.intensity = intensity;
        mats = null;
    }

    public LightValue(Light light, float intensity, Material[] coneMats)
    {
        this.light = light;
        this.intensity = intensity;
        mats = coneMats;
    }

    public Light GetLight()
    {
        return light;
    }

    public float GetIntensity()
    {
        return intensity;
    }
    

    public void SetMatsVal(float val)
    {
        foreach (var mat in mats)
        {
            mat.SetFloat(Alpha, val);
        }
    }

    public void DecreaseMat()
    {
        if (mats!=null)
        {
            foreach (var mat in mats)
            {
                mat.SetFloat(Alpha, Mathf.Max(0, mat.GetFloat(Alpha) - 0.1f));
            }
        }
    }

    public void IncreaseMat()
    {
        if (mats!=null)
        {
            foreach (var mat in mats)
            {
                mat.SetFloat(Alpha, Mathf.Min(0.8f, mat.GetFloat(Alpha) + 0.1f));
            } 
        }
        
    }
}

[DefaultExecutionOrder(10)]
public class LightsManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float distance = 10;
    private List<LightValue> lights, turnOnLights, turnOffLights;
    private PlayerValues playerValues;
    [SerializeField] private LayerMask colisionLayers;
    [SerializeField] private float switchSpeed = 0.1f;

    private void Awake()
    {
        lights = new List<LightValue>();
        turnOffLights = new List<LightValue>();
        turnOnLights = new List<LightValue>();
    }

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        foreach (var light in FindObjectsOfType<LightUnit>())
        {
            if (light.GetLightValue().GetLight().type is LightType.Spot or LightType.Point)
            {
                lights.Add(light._lightValue);
                light.GetLightValue().GetLight().enabled = false;
                light.GetLightValue().SetMatsVal(0);
            }
        }

        foreach (var light in FindObjectsOfType<Light>())
        {
            if (light.type is LightType.Spot or LightType.Point)
            {
                lights.Add(new LightValue(light, light.intensity));
                light.enabled = false;
            }
        }
    }


    private void Update()
    {
        foreach (var light in lights)
        {
            if (Vector3.Distance(playerValues.GetPos(), light.GetLight().transform.position) < distance)
            {
                if (Physics.Raycast(playerValues.GetPos(), light.GetLight().transform.position - playerValues.GetPos(),
                        Vector3.Distance(light.GetLight().transform.position, playerValues.GetPos()),
                        colisionLayers))
                {
                    if (light.GetLight().enabled)
                    {
                        TurnOffLight(light);
                    }
                }
                else
                {
                    if (!light.GetLight().enabled)
                    {
                        TurnOnLight(light);
                    }
                }
            }
            else
            {
                if (light.GetLight().enabled)
                {
                    TurnOffLight(light);
                }
            }
        }


        if (turnOnLights.Count > 0)
            IncreaseValueLight();
        if (turnOffLights.Count > 0)
            DecreaseValueLight();
    }


    private void TurnOnLight(LightValue light)
    {
        if (turnOffLights.Contains(light))
            turnOffLights.Remove(light);

        if (!turnOnLights.Contains(light))
            turnOnLights.Add(light);
    }

    private void TurnOffLight(LightValue light)
    {
        if (turnOnLights.Contains(light))
            turnOnLights.Remove(light);
        if (!turnOffLights.Contains(light))
            turnOffLights.Add(light);
    }

    private void IncreaseValueLight()
    {
        LightValue[] aux = turnOnLights.ToArray();
        foreach (var light in aux)
        {
            if (!light.GetLight().enabled)
            {
                light.GetLight().enabled = true;
                light.GetLight().intensity = 0;
            }

            light.GetLight().intensity += switchSpeed;
            light.IncreaseMat();
            if (light.GetLight().intensity >= light.GetIntensity())
            {
                turnOnLights.Remove(light);
            }
        }
    }

    private void DecreaseValueLight()
    {
        LightValue[] aux = turnOffLights.ToArray();
        foreach (var light in aux)
        {
            light.GetLight().intensity = Mathf.Max(0f, light.GetLight().intensity - switchSpeed);
            light.DecreaseMat();
            if (light.GetLight().intensity == 0)
            {
                turnOffLights.Remove(light);
                light.GetLight().enabled = false;
            }
        }
    }
}