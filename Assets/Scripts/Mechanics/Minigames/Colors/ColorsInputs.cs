using Mechanics.General_Inputs;
using UnityEngine;

public class ColorsInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private ColorsManager colorsManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        colorsManager = FindObjectOfType<ColorsManager>();
    }

    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.ColorsMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
                CursorManager.ShowCursor();
        }
    }

    // Update is called once per frame
    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            colorsManager.MoveFace(move);
        }
    }
}