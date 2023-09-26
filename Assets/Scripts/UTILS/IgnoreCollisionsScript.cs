using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionsScript : MonoBehaviour
{
    private void Awake()
    {
        Physics.IgnoreLayerCollision(11, 12);
        Physics.IgnoreLayerCollision(8, 10);
        Physics.IgnoreLayerCollision(9, 13);
        Physics.IgnoreLayerCollision(8, 13);
        Physics.IgnoreLayerCollision(2, 13);
        Physics.IgnoreLayerCollision(8, 8);
        Physics.IgnoreLayerCollision(18, 11);

    }
}