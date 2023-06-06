using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ObetiveType
{
    Add,
    Remove
}

public class ObjetivesTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private Objetives objetives;
    [SerializeField] private ObetiveType objetiveType;
    [SerializeField] private string text;

    void Start()
    {
        objetives = FindObjectOfType<Objetives>();
    }

    // Update is called once per frame


    private void OnTriggerExit(Collider other)
    {
        if (objetiveType is ObetiveType.Add)
            objetives.SetNewObjetive(text);
        else
            objetives.RemoveObjetive();
    }
}