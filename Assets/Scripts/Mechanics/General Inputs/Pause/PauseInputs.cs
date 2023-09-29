using Mechanics.General_Inputs;
using UnityEngine;

public class PauseInputs : MonoBehaviour,InputInterface
{
    // Start is called before the first frame update
    private PauseManager pauseManager;
    private PlayerValues playerValues;
    private PlayerNewInputs _newInputs;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        pauseManager = FindObjectOfType<PauseManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerValues.GetPaused())
        {
            if (_newInputs.UpTap())
                pauseManager.SelectPrev();
            
            else if (_newInputs.DownTap())
                pauseManager.SelectNext();
            
            else if (_newInputs.RightTap())
                pauseManager.IncreaseValue();
            
            else if (_newInputs.LeftTap())
                pauseManager.DecreaseValue();
            
            else if (_newInputs.SelectBasic())
                pauseManager.Confirm();
            
        }
    }

    public void PerformAction(Move move)
    {
        if (move.face is FACES.R)
        {
            if (move.direction == 1)
                pauseManager.SelectPrev();
            else
                pauseManager.SelectNext();
        }
        else if (move.face is FACES.F)
        {
            if (move.direction == 1)
                pauseManager.IncreaseValue();
            else
                pauseManager.DecreaseValue();
        }
        else if (move.Equals(new Move(FACES.U, 1)))
        {
            pauseManager.Confirm();
        }
        else if (move.Equals(new Move(FACES.B, 1)))
        {
            pauseManager.Pause();
        }
    }
}