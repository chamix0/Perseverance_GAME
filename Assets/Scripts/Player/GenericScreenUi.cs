using System.Collections;
using System.Diagnostics;
using Mechanics.General_Inputs;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenericScreenUi : MonoBehaviour
{
    // Start is called before the first frame update
    private float textAlpha, targetAlpha, _tA;
    private bool updateText;
    private TMP_Text genericText;
    private PlayerValues _playerValues;

    //face
    private Stopwatch _blinkTimer;
    private float _blinkCooldown;
    private bool _blinking;
    private bool _faceUpdated;
    private float faceAlpha = 1;

    private string normalFace = "O.O";
    private const string NormalFace1 = "O.O";
    private const string NormalFace2 = "0.0";
    private const string BlinkFace = "-.-";
    private const string ScaredFace = ">.<";

    private void Awake()
    {
        _blinkTimer = new Stopwatch();
        genericText = GameObject.Find("generic text").gameObject.GetComponent<TMP_Text>();
        _playerValues = GetComponent<PlayerValues>();
    }

    void Start()
    {
        genericText.alpha = 0;
        _blinkTimer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateText)
        {
            UpdateTextAlpha();
        }

        if (!_playerValues.GetIsStucked() && _playerValues.GetInputsEnabled() &&
            _playerValues.GetCurrentInput() is CurrentInput.Movement
                or CurrentInput.StealthMovement
                or CurrentInput.RotatingWall or CurrentInput.ShootMovement or CurrentInput.Conversation)
        {
            Face();
        }
    }

    #region screen text

    private void Face()
    {
        if (_blinkTimer.Elapsed.TotalSeconds > _blinkCooldown && Time.timeScale > 0)
        {
            _blinkTimer.Stop();
            _blinkTimer.Reset();
            _blinking = true;
            _faceUpdated = false;
            _blinkCooldown = Random.Range(0, 11);
            StartCoroutine(BlinkCoroutine());
            normalFace = _blinkCooldown <= 4 ? NormalFace2 : NormalFace1;
        }

        if (!_faceUpdated)
        {
            SetTextSize(95f);
            if (genericText.alpha < 1)
                FadeInText();

            _faceUpdated = true;

            if (_blinking)
                SetText(BlinkFace);
            else if (_playerValues.GetIsStucked() || !_playerValues.GetIsGrounded())
            {
                _playerValues.NotifyAction(PlayerActions.ScaredFace);
                SetText(ScaredFace);
                _faceUpdated = false;
            }
            else
            {
                _playerValues.NotifyAction(PlayerActions.NormalFace);
                SetText(normalFace);
            }
        }
    }

    IEnumerator BlinkCoroutine()
    {
        _playerValues.NotifyAction(PlayerActions.BlinkFace);
        float wait = Random.Range(0.01f, 1);
        yield return new WaitForSeconds(wait);
        _blinkTimer.Start();
        _blinking = false;
        _faceUpdated = false;
    }

    public void SetTextSize(float size)
    {
        genericText.fontSize = size;
    }

    public void SetTextAlpha(float value)
    {
        genericText.alpha = value;
    }

    public void SetText(string cad, float size)
    {
        genericText.text = cad;
        SetTextSize(size);
    }

    private void SetText(string cad)
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
        _tA += 0.1f * Time.unscaledDeltaTime;
        if (_tA > 1.0f)
        {
            _tA = 1.0f;
            updateText = false;
        }
    }

    #endregion
}