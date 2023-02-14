using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInputs : MonoBehaviour
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
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _menuManager.SelectPrevButton();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                _menuManager.SelectNextButton();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                _menuManager.PressEnter();
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