using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(7)]
public class LookAtPlayer : MonoBehaviour
{
    private PlayerValues playerValues;
    public Vector3 offset;
    public bool lookingPlayer,hardLookPlayer;
    private Transform target;
    private float speed;
    public float  slowSpeed, normalSpeed;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        target = playerValues.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (hardLookPlayer)
        {
            speed = normalSpeed;
        }
        Quaternion lookAtRotation = Quaternion.identity;
        if (lookingPlayer)
            lookAtRotation = Quaternion.LookRotation(transform.position - playerValues.transform.position);
        else
            lookAtRotation = Quaternion.LookRotation(transform.position - target.position);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(offset.x, lookAtRotation.eulerAngles.y + offset.y, offset.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, LookAtRotationOnly_Y, Time.unscaledDeltaTime * speed);
    }

    public void LookPlayer()
    {
        lookingPlayer = true;
        speed = normalSpeed;
    }

    public void LookPlayerSlow()
    {
        lookingPlayer = true;
        speed = slowSpeed;
    }

    public void LookNode(Transform pos)
    {
        speed = normalSpeed;
        target = pos;
        lookingPlayer = false;
    }
}