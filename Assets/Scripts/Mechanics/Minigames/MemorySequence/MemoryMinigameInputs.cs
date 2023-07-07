using Mechanics.General_Inputs;
using UnityEngine;

public class MemoryMinigameInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update  
    private PlayerValues _playerValues;
    private MemoryMingameManager memoryMingame;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        memoryMingame = FindObjectOfType<MemoryMingameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.MemoryMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            if (Input.anyKey)
                CursorManager.ShowCursor();
        }
    }
    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            memoryMingame.Select(move.color);
        }
    }
}