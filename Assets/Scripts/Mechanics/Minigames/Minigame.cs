using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
    public abstract void StartMinigame();

    public abstract void ShowUI();

    public abstract void HideUI();

}