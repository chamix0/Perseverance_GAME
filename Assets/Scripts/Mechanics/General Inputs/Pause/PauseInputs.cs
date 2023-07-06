using Mechanics.General_Inputs;
using UnityEngine;

public class PauseInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PauseManager pauseManager;
    private GuiManager guiManager;
    private PlayerValues playerValues;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        pauseManager = FindObjectOfType<PauseManager>();
        guiManager = FindObjectOfType<GuiManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                guiManager.SetTutorial(
                    "Esc - Exit pause ");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                pauseManager.SelectPrev();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                pauseManager.SelectNext();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                pauseManager.IncreaseValue();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                pauseManager.DecreaseValue();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                pauseManager.Confirm();
            }
        }
    }

    public void PerformAction(Move move)
    {
        guiManager.SetTutorial(
            "R - Select Next/Previous    F - Increase/Decrease value     U - Confirm     B - Exit");

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