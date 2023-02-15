using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> models;
    private List<List<Renderer>> renderers;
    private int modelIndex;

    private void Start()
    {
        modelIndex = 1;
        renderers = new List<List<Renderer>>();
        GetRenderers();
        HideModels();
        ActivateModel(modelIndex);
    }


    public void ShowNext()
    {
        int aux = modelIndex;
        DeactivateModel(aux);
        aux = (aux + 1) % models.Count;
        ActivateModel(aux);
        modelIndex = aux;
    }

    public void ShowPrev()
    {
        int aux = modelIndex;
        DeactivateModel(aux);
        aux = aux - 1 < 0 ? models.Count - 1 : aux - 1;
        ActivateModel(aux);
        modelIndex = aux;
    }

    private void HideModels()
    {
        for (int i = 0; i < models.Count; i++)
            DeactivateModel(i);
    }

    private void GetRenderers()
    {
        foreach (var model in models)
        {
            List<Renderer> auxRenderers = new List<Renderer>(model.GetComponentsInChildren<Renderer>());
            renderers.Add(auxRenderers);
        }
    }

    private void DeactivateModel(int index)
    {
        foreach (var renderer in renderers[index])
            renderer.enabled = false;
    }

    private void ActivateModel(int index)
    {
        foreach (var renderer in renderers[index])
            renderer.enabled = true;
    }
}