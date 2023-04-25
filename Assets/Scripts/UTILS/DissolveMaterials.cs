using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class DissolveMaterials : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rootObject;
    private Dictionary<int, Material[]> materials, dissolveMaterials;
    [SerializeField] private Shader shader;
    [SerializeField] private List<GameObject> exceptions;
    [SerializeField] private bool hideOnStart = false;
    [SerializeField] private float speed = 0.5f;
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
        materials = new Dictionary<int, Material[]>();
        dissolveMaterials = new Dictionary<int, Material[]>();
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
        List<GameObject> aux = new List<GameObject>();
        foreach (var obj in exceptions)
        {
            foreach (var child in obj.GetComponentsInChildren<Transform>())
            {
                if (!exceptions.Contains(child.gameObject))
                    aux.Add(child.gameObject);
            }
        }
        //
        // foreach (var mats in rootObject.GetComponentsInChildren<DissolveMaterials>())
        // {
        //     if (mats.gameObject != rootObject)
        //     {
        //         foreach (var pair in mats.GetMats())
        //             materials.Add(pair.Key, pair.Value);
        //         foreach (var pair in mats.GetDisMats())
        //             dissolveMaterials.Add(pair.Key, pair.Value);
        //     }
        // }

        exceptions.AddRange(aux.ToArray());

        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!materials.ContainsKey(meshRenderer.GetInstanceID()) && !exceptions.Contains(meshRenderer.gameObject))
            {
                Material[] oldMats = meshRenderer.sharedMaterials;
                materials.Add(meshRenderer.GetInstanceID(), oldMats);
                List<Material> auxMats = new List<Material>();
                foreach (var oldMat in oldMats)
                {
                    Material disMat = new Material(shader);
                    disMat.SetTexture(Albedo, oldMat.GetTexture(BaseMap));
                    disMat.SetTexture(Emission, oldMat.GetTexture(EmissionMap));
                    disMat.SetInt(HasEmission, 0);
                    disMat.SetTexture(Metalic, oldMat.GetTexture(MetallicGlossMap));
                    auxMats.Add(disMat);
                }

                dissolveMaterials.Add(meshRenderer.GetInstanceID(), auxMats.ToArray());
            }
        }

        foreach (var skinnedMeshRenderer in rootObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (!materials.ContainsKey(skinnedMeshRenderer.GetInstanceID()) &&
                !exceptions.Contains(skinnedMeshRenderer.gameObject))
            {
                Material[] oldMats = skinnedMeshRenderer.sharedMaterials;
                materials.Add(skinnedMeshRenderer.GetInstanceID(), oldMats);
                List<Material> auxMats = new List<Material>();
                foreach (var oldMat in oldMats)
                {
                    Material disMat = new Material(shader);
                    disMat.SetTexture(Albedo, oldMat.GetTexture(BaseMap));
                    if (oldMat.GetTexture(EmissionMap) != null)
                        disMat.SetTexture(Emission, oldMat.GetTexture(EmissionMap));
                    else
                        disMat.SetInt(HasEmission, 0);
                    disMat.SetTexture(Metalic, oldMat.GetTexture(MetallicGlossMap));
                    auxMats.Add(disMat);
                }

                dissolveMaterials.Add(skinnedMeshRenderer.GetInstanceID(), auxMats.ToArray());
            }
        }
    }

    public KeyValuePair<int, Material[]>[] GetMats()
    {
        return materials.ToArray();
    }

    public KeyValuePair<int, Material[]>[] GetDisMats()
    {
        return dissolveMaterials.ToArray();
    }

    private void PutOriginalMaterials()
    {
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (materials.ContainsKey(meshRenderer.GetInstanceID()) && !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = materials[meshRenderer.GetInstanceID()];
            }
        }

        foreach (var skinnedMeshRenderer in rootObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (materials.ContainsKey(skinnedMeshRenderer.GetInstanceID()) &&
                !exceptions.Contains(skinnedMeshRenderer.gameObject))
            {
                skinnedMeshRenderer.sharedMaterials = materials[skinnedMeshRenderer.GetInstanceID()];
            }
        }
    }

    public void HideMaterials()
    {
        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
                foreach (var mat in dissolveMaterials[meshRenderer.GetInstanceID()])
                {
                    mat.SetFloat(TimeStep, 1);
                }
            }
        }

        foreach (var skinnedMeshRenderer in rootObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (!exceptions.Contains(skinnedMeshRenderer.gameObject))
            {
                skinnedMeshRenderer.sharedMaterials = dissolveMaterials[skinnedMeshRenderer.GetInstanceID()];
                foreach (var mat in dissolveMaterials[skinnedMeshRenderer.GetInstanceID()])
                {
                    mat.SetFloat(TimeStep, 1);
                }
            }
        }

        _tA = 0.0f;
        targetValue = 1;
    }

    public void DissolveIn()
    {
        int lastId = 0;
        foreach (var skinnedMeshRenderer in rootObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (!exceptions.Contains(skinnedMeshRenderer.gameObject))
            {
                skinnedMeshRenderer.sharedMaterials = dissolveMaterials[skinnedMeshRenderer.GetInstanceID()];
            }
        }

        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (dissolveMaterials.ContainsKey(meshRenderer.GetInstanceID()) &&
                !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
                lastId = meshRenderer.GetInstanceID();
            }
        }


        foreach (var mat in dissolveMaterials[lastId])
        {
            currentVal = mat.GetFloat(TimeStep);
        }

        targetValue = 0;
        _tA = 0.0f;
        updateValue = true;
    }

    public void DissolveOut()
    {
        int lastId = 0;

        foreach (var skinnedMeshRenderer in rootObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (dissolveMaterials.ContainsKey(skinnedMeshRenderer.GetInstanceID()) &&
                !exceptions.Contains(skinnedMeshRenderer.gameObject))
            {
                skinnedMeshRenderer.sharedMaterials = dissolveMaterials[skinnedMeshRenderer.GetInstanceID()];
            }
        }

        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (dissolveMaterials.ContainsKey(meshRenderer.GetInstanceID()) &&
                !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
                lastId = meshRenderer.GetInstanceID();
            }
        }

        if (dissolveMaterials.ContainsKey(lastId))
            foreach (var mat in dissolveMaterials[lastId])
            {
                currentVal = mat.GetFloat(TimeStep);
            }

        targetValue = 1;
        _tA = 0.0f;
        updateValue = true;
    }


    private void Fade()
    {
        // currentVal = Mathf.Lerp(currentVal, targetValue, _tA);
        _tA = speed * Time.deltaTime;

        currentVal = Mathf.MoveTowards(currentVal, targetValue, _tA);
        foreach (var sharedMats in dissolveMaterials)
        {
            foreach (var mat in sharedMats.Value)
            {
                mat.SetFloat(TimeStep, currentVal);
            }
        }

        if (targetValue == 0)
        {
            if (Mathf.Abs(targetValue - currentVal) < 0.1f)
            {
                _tA = 1.0f;
                updateValue = false;
                PutOriginalMaterials();
            }
        }
    }
}