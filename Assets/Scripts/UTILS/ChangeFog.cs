using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

enum FogTriggerModes
{
    Enter,
    Exit,
    Both
}

public class ChangeFog : MonoBehaviour
{
    // Start is called before the first frame update
    private Color originalColor;
    [SerializeField] Color desiredColor = Color.black;
    private bool updateFog;
    private Color targetColor;
    [SerializeField] private float transitionTime = 0.5f;
    private float transitionOriginalTime;
    [SerializeField] private FogTriggerModes _mode = FogTriggerModes.Both;

    void Start()
    {
        originalColor = RenderSettings.fogColor;
        transitionOriginalTime = transitionTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateFog)
            UpdateFog();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag($"Player") && _mode is FogTriggerModes.Both or FogTriggerModes.Enter)
        {
            ChangeTheColor(desiredColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag($"Player") && _mode is FogTriggerModes.Both or FogTriggerModes.Exit)
        {
            ChangeTheColor(originalColor);
        }
    }

    private void ChangeTheColor(Color color)
    {
        if (RenderSettings.fogColor != color)
        {
            updateFog = true;
            targetColor = color;
        }
    }

    private void UpdateFog()
    {
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetColor, Time.deltaTime / transitionTime);
        transitionTime -= Time.deltaTime;
        if (RenderSettings.fogColor == targetColor)
        {
            updateFog = false;
            transitionTime = transitionOriginalTime;
        }
    }
}