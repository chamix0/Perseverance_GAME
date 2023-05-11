using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(2)]
public class SwitchInterruptor : MonoBehaviour
{
    // Start is called before the first frame update
    private bool redBlue;
    [SerializeField] SwitchDoorManager switchDoorManager;
    private MeshRenderer renderer;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        SetColor();
        switchDoorManager.FlickTheSwitch(redBlue);
    }

    private void SetColor()
    {
        if (redBlue)
            renderer.sharedMaterial.SetColor("_Background_color", Color.magenta);
        else
            renderer.sharedMaterial.SetColor("_Background_color", Color.green);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            redBlue = !redBlue;
            switchDoorManager.FlickTheSwitch(redBlue);
            SetColor();
        }
    }

    public bool GetState()
    {
        return redBlue;
    }
}