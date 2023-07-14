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
    [SerializeField] private Transform newGamePos, startedGamePos;
    private string[] bodyParts = { "Cylinder.001", "Cylinder.015", "Cube.012", "Cube.007", "Cube.001", "Cube.005" };
    void Start()
    {
        //collisions
        Physics.IgnoreLayerCollision(9, 13);
        Physics.IgnoreLayerCollision(8, 13);

        
        _playerValues = FindObjectOfType<PlayerValues>();
        model = _playerValues.gameData.GetEddoModel();
        Material[] oldMaterials = modelObjects[0].GetComponent<Renderer>().materials;
        Material[] newMaterials = modelTextures[model].gameObject.GetComponent<Renderer>().sharedMaterials;
        SetMaterials(oldMaterials, newMaterials);

        for (int i = 1; i < 7; i++)
        {
              
            oldMaterials = modelObjects[i].GetComponent<Renderer>().materials;
            newMaterials = modelTextures[model].transform.Find(bodyParts[i-1]).gameObject.GetComponent<Renderer>()
                .sharedMaterials;
            SetMaterials(oldMaterials, newMaterials);
        }
        

        SpawnPoint();
        
        //cursor
        CursorManager.HideCursor();
    }

    private void SpawnPoint()
    {
        if (_playerValues.gameData.GetIsNewGame())
        {
            if (newGamePos)
                _playerValues.transform.position = newGamePos.position;
        }
        else
        {
            if (startedGamePos)
                _playerValues.transform.position = startedGamePos.position;
            
        }
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