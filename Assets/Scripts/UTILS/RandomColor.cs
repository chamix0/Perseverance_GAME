using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
                GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.4f, 0.5f, 0.5f, 1f);

    }
    
}
