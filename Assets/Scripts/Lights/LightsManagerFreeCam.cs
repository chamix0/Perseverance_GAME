using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManagerFreeCam : MonoBehaviour
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
        foreach (var light in FindObjectsOfType<Light>())
        {
            if (light.type is LightType.Spot or LightType.Point)
            {
                lights.Add(new LightValue(light, light.intensity));
                light.enabled = false;
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireSphere(playerValues.GetPos(), distance);
    // }


    private void Update()
    {
        foreach (var light in lights)
        {
            if (Vector3.Distance(transform.position, light.GetLight().transform.position) < distance)
            {
                if (Physics.Raycast(transform.position, light.GetLight().transform.position - transform.position,
                        Vector3.Distance(light.GetLight().transform.position, transform.position),
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
            if (light.GetLight().intensity == 0)
            {
                turnOffLights.Remove(light);
                light.GetLight().enabled = false;
            }
        }
    }
}
