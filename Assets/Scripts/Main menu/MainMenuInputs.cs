using UnityEngine;

public class MainMenuInputs : MonoBehaviour
{
    //components
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
        if (_myInputManager.GetCurrentInput() == CurrentMenuInput.Menu && _myInputManager.GetInputsEnabled())
        {
            if (Input.anyKey)
            {
                _menuManager.SetTutortialText("W - up   S - down  Enter/D - Select");
            }

            //select next button
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                _menuManager.SelectPrevButton();

            //select previous button 
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                _menuManager.SelectNextButton();

            //select button
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.D))
                _menuManager.PressEnter();
        }
    }

    public void PerformAction(Move move)
    {
        _menuManager.SetTutortialText("R - up/down  F - Select");

        //move between buttons
        if (move.face == FACES.R)
        {
            if (move.direction == 1)
                _menuManager.SelectPrevButton();
            else
                _menuManager.SelectNextButton();
        }

        //select button
        if (move.face == FACES.F)
        {
            if (move.direction == 1)
                _menuManager.PressEnter();
        }
    }
}