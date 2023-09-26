using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RainbowText : MonoBehaviour
{
    // Start is called before the first frame update
    private float value = 0;
    [SerializeField] private float speed = 0.1f;
    private TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        value = MyUtils.Clamp0360(value + speed);
        text.color = Color.HSVToRGB(value/360, 1, 1);
    }
}