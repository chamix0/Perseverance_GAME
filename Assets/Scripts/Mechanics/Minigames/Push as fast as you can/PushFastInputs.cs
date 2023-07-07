using Mechanics.General_Inputs;
using UnityEngine;

public class PushFastInputs : MonoBehaviour
{
    private PushFastManager _pushFastManager;
    private PlayerValues _playerValues;


    // Start is called before the first frame update
    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _pushFastManager = FindObjectOfType<PushFastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.ClickFastMinigame && _playerValues.GetInputsEnabled()&&!_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                CursorManager.ShowCursor();
                _pushFastManager.ShowKeyTutorial();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _pushFastManager.Click();
            }
        }
    }

    public void PerformAction(Move move)
    {
        _pushFastManager.ShowCubeTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            _pushFastManager.Click();
        }
    }
}