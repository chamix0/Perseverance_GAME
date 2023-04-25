using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private PlayerValues playerValues;
    public Vector3 offset;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion lookAtRotation = Quaternion.LookRotation(transform.position - playerValues.transform.position);

        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(offset.x, lookAtRotation.eulerAngles.y, offset.z);
        transform.rotation = LookAtRotationOnly_Y;
    }
}