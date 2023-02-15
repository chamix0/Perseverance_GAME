using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private MyMenuInputManager _myInputManager;
    private NewGameManager _newGameManager;
    private MenuCamerasController _camerasController;

    void Start()
    {
        _myInputManager = FindObjectOfType<MyMenuInputManager>();
        _newGameManager = FindObjectOfType<NewGameManager>();
        _camerasController = FindObjectOfType<MenuCamerasController>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.NewGame && _myInputManager.GetInputsEnabled())
        {
            //siguiente modelo
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _newGameManager.ShowNext();
            }
            //modelo anterior
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _newGameManager.ShowPrev();
            }
            //confirmar modelo
            else if (Input.GetKeyDown(KeyCode.Return))
            {
            }
            //volver al menú
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                _camerasController.SetCamera(0);
                _myInputManager.SetCurrentInput(CurrentMenuInput.Menu);
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