using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGenericDoor : MonoBehaviour
{
    //components
    private GenericDoorBase doorBase;
    void Start()
    {
        doorBase = transform.parent.GetComponentInChildren<GenericDoorBase>();
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
          if (other.gameObject.CompareTag("Player"))
                    doorBase.CloseDoor();
    }
    
}
