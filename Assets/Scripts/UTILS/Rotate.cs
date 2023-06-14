using UnityEngine;
using UnityEngine.Animations;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotateSpeed;
    [SerializeField] Vector3 axis = Vector3.one;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        transform.Rotate(rotateSpeed * Time.deltaTime*axis.x, rotateSpeed * Time.deltaTime*axis.y, rotateSpeed * Time.deltaTime*axis.z);

    }
}