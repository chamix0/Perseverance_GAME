using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;

public class DissolveMaterials : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rootObject;
    private Dictionary<string, Material> materials, dissolveMaterials;
    [SerializeField] private Shader shader;
    [SerializeField] private List<GameObject> exceptions;
    [SerializeField] private bool hideOnStart = false;

    private bool updateValue;
    private float currentVal, targetValue;
    private float _tA;
    private static readonly int TimeStep = Shader.PropertyToID("_Time_Step");
    private static readonly int Albedo = Shader.PropertyToID("_Albedo");
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    private static readonly int Emission = Shader.PropertyToID("_Emission");
    private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
    private static readonly int Metalic = Shader.PropertyToID("_Metalic");
    private static readonly int Metallic = Shader.PropertyToID("_Metallic");
    private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
    private static readonly int HasEmission = Shader.PropertyToID("_HasEmission");
    private static readonly int Normal = Shader.PropertyToID("_Normal");
    private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");


    private void Awake()
    {
        materials = new Dictionary<string, Material>();
        dissolveMaterials = new Dictionary<string, Material>();
    }

    void Start()
    {
        GetOriginalMaterials();
        if (hideOnStart)
            HideMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateValue)
        {
            Fade();
        }
    }

    private void GetOriginalMaterials()
    {
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                Material oldMat = meshRenderer.sharedMaterial;
                materials.Add(meshRenderer.name, oldMat);
                Material disMat = new Material(shader);
                dissolveMaterials.Add(meshRenderer.name, disMat);
                disMat.SetTexture(Albedo, oldMat.GetTexture(BaseMap));
                if (oldMat.GetTexture(EmissionMap) != null)
                    disMat.SetTexture(Emission, oldMat.GetTexture(EmissionMap));
                else
                    disMat.SetInt(HasEmission, 0);
                disMat.SetTexture(Metalic, oldMat.GetTexture(MetallicGlossMap));
            }
        }
    }

    private void PutOriginalMaterials()
    {
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterial = materials[meshRenderer.name];
            }
        }
    }

    public void HideMaterials()
    {
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterial = dissolveMaterials[meshRenderer.name];
                dissolveMaterials[meshRenderer.name].SetFloat(TimeStep, 1);
            }
        }

        _tA = 0.0f;
        targetValue = 1;
    }

    public void DissolveIn()
    {
        string lastName = "";
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterial = dissolveMaterials[meshRenderer.name];
                lastName = meshRenderer.name;
            }
        }

        currentVal = dissolveMaterials[lastName].GetFloat(TimeStep);
        targetValue = 0;
        _tA = 0.0f;
        updateValue = true;
    }

    public void DissolveOut()
    {
        string lastName = "";
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (dissolveMaterials.ContainsKey(meshRenderer.name) && !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterial = dissolveMaterials[meshRenderer.name];
                lastName = meshRenderer.name;
            }
        }

        if (dissolveMaterials.ContainsKey(lastName))
            currentVal = dissolveMaterials[lastName].GetFloat(TimeStep);
        targetValue = 1;
        _tA = 0.0f;
        updateValue = true;
    }


    private void Fade()
    {
        currentVal = Mathf.Lerp(currentVal, targetValue, _tA);
        foreach (var mat in dissolveMaterials)
        {
            mat.Value.SetFloat(TimeStep, currentVal);
        }

        _tA += 0.1f * Time.deltaTime;
        if (_tA >= 0.3f)
        {
            _tA = 1.0f;
            updateValue = false;
            if (currentVal <= 0.5f)
            {
                PutOriginalMaterials();
            }
        }
    }
}