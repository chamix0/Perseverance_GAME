using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShader : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private Material mat;

    private PlayerValues playerValues;

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(8,10);
        playerValues = FindObjectOfType<PlayerValues>();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        mat.SetVector("_pos", playerValues.transform.position);
    }
}