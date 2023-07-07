using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Vector3 offset;
    public Transform target;
    public float speed;
    

    // Update is called once per frame
    void Update()
    {
        Quaternion lookAtRotation = Quaternion.identity;
        lookAtRotation = Quaternion.LookRotation(transform.position - target.position);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(offset.x, lookAtRotation.eulerAngles.y + offset.y, offset.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, LookAtRotationOnly_Y, Time.unscaledDeltaTime * speed);
    }
    
}