using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCenterReference : MonoBehaviour
{
    public Image _imageTop, _imageFront, _imageBottom, _imageRight, _imageBack, _imageLeft;
    public CanvasGroup rootObject;
    [SerializeField] private CubeInputs _cubeInputs = null;
    private bool updateColors;

    private void Start()
    {
        
        CubeInputs aux = FindObjectOfType<CubeInputs>();
        if (aux != null)
            _cubeInputs = aux;
        _imageTop.color = Color.clear;
        _imageFront.color = Color.clear;
        rootObject.alpha=0;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateColors && _cubeInputs)
        {
            if (_imageTop.color != _cubeInputs.GetTopColor())
                _imageTop.color = _cubeInputs.GetTopColor();
            if (_imageFront.color != _cubeInputs.GetFrontColor())
                _imageFront.color = _cubeInputs.GetFrontColor();
        }
    }

    public void ShowColors()
    {
        rootObject.alpha=1;
        updateColors = true;
    }
}