using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[DefaultExecutionOrder(4)]
public class MoveSequencePiece : MonoBehaviour
{
    //DATA
    private TMP_Text _text;
    private bool _updateSnap, _updateAlpha;
    private float _tX = 0, _tY = 0, _tA = 0;
    private float targetAlpha;
    private Vector2 targetPos;
    private RectTransform _rectTransform;
    private bool isLast;


    // Start is called before the first frame update
    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateSnap)
            SmoothMoveElement();
        if (_updateAlpha)
            SmoothSetAlpha();
    }

    public void SetTextColor(Color color)
    {
        _text.color = color;
    }

    public void SetText(string value)
    {
        _text.text = value;
    }

    public void MoveElement(RectTransform pos, bool value)
    {
        targetPos = pos.anchoredPosition;
        _updateSnap = true;
        isLast = value;
        _tX = 0;
        _tY = 0;
    }

    private void SmoothMoveElement()
    {
        Vector3 position = _rectTransform.anchoredPosition;
        Vector3 auxPos = new Vector3();
        auxPos.x = Mathf.Lerp(position.x, targetPos.x, _tX);
        auxPos.y = Mathf.Lerp(position.y, targetPos.y, _tY);
        auxPos.z = 0;

        _rectTransform.anchoredPosition = auxPos;
        if (isLast)
            _tY += 1f * Time.deltaTime;
        else
            _tY += 0.85f * Time.deltaTime;

        if (_tX > 1.0f && _tY > 1.0f)
        {
            _tX = 1.0f;
            _tY = 1.0f;
            _updateSnap = false;
        }
    }

    public void SetAlpha(float value)
    {
        targetAlpha = value;
        _updateAlpha = true;
        _tA = 0;
    }

    private void SmoothSetAlpha()
    {
        _text.alpha = Mathf.Lerp(_text.alpha, targetAlpha, _tA);
        _tA += 1f * Time.deltaTime;
        if (_tA > 1.0f)
        {
            _tA = 1.0f;
            _updateAlpha = false;
        }
    }
}