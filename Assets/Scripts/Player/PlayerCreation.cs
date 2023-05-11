using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(2)]
public class PlayerCreation : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private int model;
    [SerializeField] private List<GameObject> modelTextures;
    [SerializeField] private List<GameObject> modelObjects;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");
    private static readonly int FresnelColor = Shader.PropertyToID("_fresnel_color");

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        model = _playerValues.gameData.GetEddoModel();
        Material[] oldMaterials = modelObjects[0].GetComponent<Renderer>().materials;
        Material[] newMaterials = modelTextures[model].gameObject.GetComponent<Renderer>().sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);

        oldMaterials = modelObjects[1].GetComponent<Renderer>().materials;
        newMaterials = modelTextures[model].transform.Find("Cylinder.001").gameObject.GetComponent<Renderer>()
            .sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);

        oldMaterials = modelObjects[2].GetComponent<Renderer>().materials;
        newMaterials = modelTextures[model].transform.Find("Cylinder.015").gameObject.GetComponent<Renderer>()
            .sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);

        oldMaterials = modelObjects[3].GetComponent<Renderer>().materials;
        newMaterials = modelTextures[model].transform.Find("Cube.012").gameObject.GetComponent<Renderer>()
            .sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);
        
        oldMaterials = modelObjects[4].GetComponent<Renderer>().materials;
        newMaterials = modelTextures[model].transform.Find("Cube.007").gameObject.GetComponent<Renderer>()
            .sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);
        
        oldMaterials = modelObjects[5].GetComponent<Renderer>().materials;
        newMaterials = modelTextures[model].transform.Find("Cube.001").gameObject.GetComponent<Renderer>()
            .sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);
        
        oldMaterials = modelObjects[6].GetComponent<Renderer>().materials;
        newMaterials = modelTextures[model].transform.Find("Cube.005").gameObject.GetComponent<Renderer>()
            .sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);
    }

    private void SetMaterials(Material[] oldMaterials, Material[] newMaterials)
    {
        for (int i = 0; i < oldMaterials.Length; i++)
        {
            Texture texture = newMaterials[i].GetTexture("_BaseMap");
            oldMaterials[i].SetTexture("_BaseMap", texture);
            if (newMaterials[i].name == "SCREEN")
            {
                oldMaterials[i].SetColor(BackgroundColor, newMaterials[i].GetColor(BackgroundColor));
                oldMaterials[i].SetColor(FresnelColor, newMaterials[i].GetColor(FresnelColor));
            }
        }
    }
}