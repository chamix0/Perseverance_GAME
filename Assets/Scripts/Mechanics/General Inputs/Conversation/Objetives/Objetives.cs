using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(8)]
public class Objetives : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private GuiManager guiManager;
    [SerializeField]private AudioSource audioSource;


    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        playerValues = FindObjectOfType<PlayerValues>();
        if (playerValues.gameData.GetGameStarted())
        {
            int lastZone = 0;
            for (int i = 0; i < 5; i++)
            {
                lastZone = i;
                if (!playerValues.gameData.checkEnabled(i)) break;
            }

            lastZone--;

            if (lastZone == 0)
                SetNewObjetive("Explore the foundry");
            else if (lastZone == 1)
                SetNewObjetive("Explore the freezer");
            else if (lastZone == 2)
                SetNewObjetive("Explore the warehouse");
            else if (lastZone == 3)
                SetNewObjetive("Explore the garden");
            else if (lastZone == 4)
                SetNewObjetive("Explore the residential zone");
        }
    }

    public void SetNewObjetive(string text)
    {
        StartCoroutine(ShowNewObjetiveCoroutine(text));
    }

    public void RemoveObjetive()
    {
        guiManager.SetObjetiveText("");
        guiManager.HideObjetives();
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ShowNewObjetiveCoroutine(string text)
    {
        guiManager.ShowObjetives();
        guiManager.SetObjetiveText(text);
        audioSource.Play();
        yield return new WaitForSeconds(3);
        guiManager.HideObjetives();

    }
}