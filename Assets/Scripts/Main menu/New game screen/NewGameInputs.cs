using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private MyMenuInputManager _myInputManager;
    private MainMenuManager _menuManager;

    void Start()
    {
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        _menuManager = FindObjectOfType<MainMenuManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Menu && _myInputManager.GetInputsEnabled())
        {
            //siguiente modelo
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
            }
            //modelo anterior
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
            }
            //confirmar modelo
            else if (Input.GetKeyDown(KeyCode.Return))
            {
            }
            //volver al menú
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
            }
        }
    }

    public void PerformAction(Move move)
    {
        //accelerate deccelerate
        if (move.face == FACES.R)
        {
        }
    }
}