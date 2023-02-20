using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericScreenUi : MonoBehaviour
{
    // Start is called before the first frame update
    private float textAlpha, targetAlpha, _tA;
    private bool updateText;
    private TMP_Text genericText;

    void Start()
    {
        genericText = GameObject.Find("generic text").gameObject.GetComponent<TMP_Text>();
        genericText.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateText)
        {
            UpdateTextAlpha();
        }
    }

    #region screen text

    public void SetTextAlpha(float value)
    {
        genericText.alpha = value;
    }

    public void SetText(string cad)
    {
        genericText.text = cad;
    }

    public void FadeInText()
    {
        updateText = true;
        _tA = 0;
        targetAlpha = 1;
    }

    public void FadeOutText()
    {
        updateText = true;
        _tA = 0;
        targetAlpha = 0;
    }

    private void UpdateTextAlpha()
    {
        genericText.alpha = Mathf.Lerp(genericText.alpha, targetAlpha, _tA);
        _tA += 0.05f * Time.deltaTime;
        if (_tA > 1.0f)
        {
            _tA = 1.0f;
            updateText = false;
        }
    }

    #endregion
}