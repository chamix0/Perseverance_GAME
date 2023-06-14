using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderUnescaledTime : MonoBehaviour
{
    private Image image;

    private static readonly int UnescaledTime = Shader.PropertyToID("_UnescaledTime");

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        image.material.SetFloat(UnescaledTime,Time.unscaledTime);
    }
}
