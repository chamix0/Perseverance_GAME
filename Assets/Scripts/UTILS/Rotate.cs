using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotateSpeed;
    [SerializeField] Axis axis = Axis.Y;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (axis)
        {
            case Axis.X:
                transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);
                break;
            case Axis.Y:
                transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
                break;
            case Axis.Z:
                transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
                break;
        }
    }
}