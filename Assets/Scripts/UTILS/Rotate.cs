using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotateSpeed;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}