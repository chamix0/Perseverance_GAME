using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuiManagerMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CanvasGroup tutorialCanvas;
    [SerializeField] private TMP_Text tutorialText;



    public void ShowTutorial()
    {
        tutorialCanvas.alpha = 1;
    }

    public void HideTutorial()
    {
        tutorialCanvas.alpha = 0;
    }

    public void SetTutorial(string text)
    {
        tutorialText.text = text;
    }
}