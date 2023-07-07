using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderUnescaledTime : MonoBehaviour
{
    private Image image;
    [SerializeField] private List<Material> _materials;
    private static readonly int UnescaledTime = Shader.PropertyToID("_UnescaledTime");

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            image = GetComponent<Image>();
        }
        catch (Exception e)
        {
            image = null;
            throw;
        }
    }

    private void Update()
    {
        foreach (var material in _materials)
        {
            material.SetFloat(UnescaledTime, Time.unscaledTime);
        }

        if (image)
            image.material.SetFloat(UnescaledTime, Time.unscaledTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
}