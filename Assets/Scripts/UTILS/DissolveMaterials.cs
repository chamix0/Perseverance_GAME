using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class DissolveMaterials : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rootObject;
    private Dictionary<int, Material[]> materials, dissolveMaterials;
    [SerializeField] private Shader shader, hitShader;
    [SerializeField] private List<GameObject> exceptions;
    [SerializeField] private bool hideOnStart = false;
    [SerializeField] private float speed = 0.5f;
    private List<MeshRenderer> meshRenderers;
    private List<SkinnedMeshRenderer> skinnedMeshRenderers;
    private Material hitMaterial, freezeMaterial;
    private bool updateValue, freeze;

    private float currentVal = 1, targetValue;

    // private float _tA;
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
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int Alpha = Shader.PropertyToID("_alpha");


    private void Awake()
    {
        meshRenderers = new List<MeshRenderer>();
        skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
        materials = new Dictionary<int, Material[]>();
        dissolveMaterials = new Dictionary<int, Material[]>();
        if (hitShader)
        {
            hitMaterial = new Material(hitShader);
            hitMaterial.SetColor(BackgroundColor, Color.white);
            hitMaterial.SetFloat(Alpha, 0.8f);
            freezeMaterial = new Material(hitShader);
            freezeMaterial.SetColor(BackgroundColor, new Color(0.3398006f, 0.8043742f, 0.8679245f, 1));
            freezeMaterial.SetFloat(Alpha, 0.3f);
        }
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


        exceptions.AddRange(aux.ToArray());

        foreach (var meshRenderer in rootObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderers.Add(meshRenderer);
            if (!materials.ContainsKey(meshRenderer.GetInstanceID()) && !exceptions.Contains(meshRenderer.gameObject))
            {
                Material[] oldMats = meshRenderer.sharedMaterials;
                materials.Add(meshRenderer.GetInstanceID(), oldMats);
                List<Material> auxMats = new List<Material>();
                foreach (var oldMat in oldMats)
                {
                    Material disMat = new Material(shader);
                    if (oldMat.HasProperty(BaseMap))
                        disMat.SetTexture(Albedo, oldMat.GetTexture(BaseMap));
                    if (oldMat.HasProperty(Emission))
                        disMat.SetTexture(Emission, oldMat.GetTexture(EmissionMap));
                    disMat.SetInt(HasEmission, 0);
                    if (oldMat.HasProperty(MetallicGlossMap))
                        disMat.SetTexture(Metalic, oldMat.GetTexture(MetallicGlossMap));
                    auxMats.Add(disMat);
                }

                dissolveMaterials.Add(meshRenderer.GetInstanceID(), auxMats.ToArray());
            }
        }

        foreach (var skinnedMeshRenderer in rootObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skinnedMeshRenderers.Add(skinnedMeshRenderer);
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
        foreach (var meshRenderer in meshRenderers)
        {
            if (materials.ContainsKey(meshRenderer.GetInstanceID()) && !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = materials[meshRenderer.GetInstanceID()];
            }
        }

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
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
        foreach (var meshRenderer in meshRenderers)
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
            }
        }

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
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
    }

    public void DissolveIn()
    {
        int lastId = 0;
        freeze = false;
        foreach (var meshRenderer in meshRenderers)
        {
            if (!exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
            }
        }

        foreach (var meshRenderer in skinnedMeshRenderers)
        {
            if (dissolveMaterials.ContainsKey(meshRenderer.GetInstanceID()) &&
                !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
                lastId = meshRenderer.GetInstanceID();
                foreach (var mat in dissolveMaterials[meshRenderer.GetInstanceID()])
                    mat.SetFloat(TimeStep, 1);
            }
        }
        

        currentVal = 1;

        targetValue = 0;
        updateValue = true;
    }

    public void DissolveOut()
    {
        int lastId = 0;
        freeze = false;
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            if (dissolveMaterials.ContainsKey(skinnedMeshRenderer.GetInstanceID()) &&
                !exceptions.Contains(skinnedMeshRenderer.gameObject))
            {
                skinnedMeshRenderer.sharedMaterials = dissolveMaterials[skinnedMeshRenderer.GetInstanceID()];
            }
        }

        foreach (var meshRenderer in meshRenderers)
        {
            if (dissolveMaterials.ContainsKey(meshRenderer.GetInstanceID()) &&
                !exceptions.Contains(meshRenderer.gameObject))
            {
                meshRenderer.sharedMaterials = dissolveMaterials[meshRenderer.GetInstanceID()];
                lastId = meshRenderer.GetInstanceID();
            }
        }

        // if (dissolveMaterials.ContainsKey(lastId))
        //     foreach (var mat in dissolveMaterials[lastId])
        //     {
        //         currentVal = mat.GetFloat(TimeStep);
        //     }

        currentVal = 0;
        targetValue = 1;
        // _tA = 0.0f;
        updateValue = true;
        StopAllCoroutines();
    }

    public void Hit(bool burn)
    {
        if (!burn)
            hitMaterial.SetColor(BackgroundColor, Color.white);
        else
            hitMaterial.SetColor(BackgroundColor, new Color(0.9716981f, 0.6452097f, 0, 1));

        if (updateValue == false)
        {
            foreach (var skinnedMeshRenderer in meshRenderers)
            {
                if (!exceptions.Contains(skinnedMeshRenderer.gameObject))
                {
                    skinnedMeshRenderer.sharedMaterial = hitMaterial;
                }
            }

            foreach (var meshRenderer in skinnedMeshRenderers)
            {
                if (dissolveMaterials.ContainsKey(meshRenderer.GetInstanceID()) &&
                    !exceptions.Contains(meshRenderer.gameObject))
                {
                    meshRenderer.sharedMaterial = hitMaterial;
                }
            }

            StartCoroutine(HitCoroutine());
        }
    }

    public void Freeze()
    {
        StopAllCoroutines();
        if (updateValue == false)
        {
            freeze = true;
            foreach (var skinnedMeshRenderer in meshRenderers)
            {
                if (!exceptions.Contains(skinnedMeshRenderer.gameObject))
                {
                    skinnedMeshRenderer.sharedMaterial = freezeMaterial;
                }
            }

            foreach (var meshRenderer in skinnedMeshRenderers)
            {
                if (dissolveMaterials.ContainsKey(meshRenderer.GetInstanceID()) &&
                    !exceptions.Contains(meshRenderer.gameObject))
                {
                    meshRenderer.sharedMaterial = freezeMaterial;
                }
            }
        }
    }

    public void UnFreeze()
    {
        if (updateValue == false)
        {
            if (freeze)
            {
                freeze = false;
                PutOriginalMaterials();
            }
        }
    }

    IEnumerator HitCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (updateValue == false)
        {
            if (freeze)
                Freeze();
            else
                PutOriginalMaterials();
        }
    }


    private void Fade()
    {
        // currentVal = Mathf.Lerp(currentVal, targetValue, _tA);
        float t = speed * Time.unscaledDeltaTime;

        currentVal = Mathf.MoveTowards(currentVal, targetValue, t);
        foreach (var sharedMats in dissolveMaterials)
        {
            foreach (var mat in sharedMats.Value)
            {
                mat.SetFloat(TimeStep, currentVal);
            }
        }


        if (Mathf.Abs(targetValue - currentVal) < 0.1f)
        {
            if (targetValue == 0)
            {
                PutOriginalMaterials();
            }

            updateValue = false;
        }
    }
}