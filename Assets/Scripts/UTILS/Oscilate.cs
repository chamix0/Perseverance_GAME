using UnityEngine;

// Floater v0.0.2
// by Donovan Keith
//
// [MIT License](https://opensource.org/licenses/MIT)
public class Oscilate : MonoBehaviour
{
// User Inputs
    public float amplitude = 0.5f;
    public float frequency = 1f;
    [SerializeField] Vector3 dir;
    [SerializeField] private float offset;
    

// Position Storage Variables
    Vector3 posOffset;
    Vector3 tempPos;

// Use this for initialization
    void Start()
    {
// Store the starting position & rotation of the object
        posOffset = transform.localPosition;
        dir.Normalize();
    }

// Update is called once per frame
    void Update()
    {
        tempPos = posOffset;
        float value = Mathf.Sin((Time.fixedTime * Mathf.PI * frequency)+offset) * amplitude;
        tempPos += dir * value;
        transform.localPosition = tempPos;
    }
}