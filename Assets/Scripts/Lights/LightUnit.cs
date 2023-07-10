using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(8)]
public class LightUnit : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Light _light;
    [SerializeField] private MeshRenderer _meshRenderer;
    public LightValue _lightValue;
    private List<Material> _materials;

    void Start()
    {
        _materials = new List<Material>(_meshRenderer.materials);
        foreach (var material in _materials)
        {
            print(material.name);
        }
        _lightValue = new LightValue(_light, _light.intensity, _materials.ToArray());
    }

    // Update is called once per frame


    public LightValue GetLightValue()
    {
        return _lightValue;
    }
}