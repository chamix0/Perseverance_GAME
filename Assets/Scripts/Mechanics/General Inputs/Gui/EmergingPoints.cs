using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmergingPoints : MonoBehaviour
{
    private bool isReady, isFading, isMoving;
    private PlayerValues _playerValues;
    private Vector3 _direction;
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFading)
            Fade();

        if (isMoving)
            Move();
    }

    public void StartPoints(Vector3 initPos, string text)
    {
        transform.position = initPos;
        isReady = false;
        isFading = true;
        isMoving = true;
        _direction = new Vector3(-0.8f, Random.Range(-0.7f, 0.7f), 0).normalized;
        _text.text = text;
        _text.alpha = 1;
        float value = Random.Range(0, 360);
        _text.color = Color.HSVToRGB(value / 360, 1, 1);
    }

    public void StartPoints(Vector3 initPos, string text, Color color)
    {
        transform.position = initPos;
        isReady = false;
        isFading = true;
        isMoving = true;
        _direction = new Vector3(-0.8f, Random.Range(-0.7f, 0.7f), 0).normalized;
        _text.text = text;
        _text.alpha = 1;
        float value = Random.Range(0, 360);
        _text.color = color;
    }

    public void SetIsReadyToUse(bool val)
    {
        isReady = val;
    }

    public bool GetIsReadyToUse()
    {
        return isReady;
    }

    private void Fade()
    {
        _text.alpha = Mathf.Max(_text.alpha - 0.01f, 0);
        if (_text.alpha <= 0)
        {
            isFading = false;
            isMoving = false;
            isReady = true;
        }
    }

    private void Move()
    {
        transform.position += _direction * 0.01f;
    }
}