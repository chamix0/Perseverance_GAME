using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[DefaultExecutionOrder(3)]
public class DialogLoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] [NotNull] private GameObject dialogsTutorial,
        dialogsNexus;

    [SerializeField] [NotNull] private List<GameObject> zonesDialogs;
    private PlayerValues playerValues;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();

        if (!playerValues.gameData.GetIsNewGame())
        {
            dialogsTutorial.SetActive(false);
            dialogsNexus.SetActive(false);
        }

        for (int i = 0; i < 5; i++)
        {
            if ((int)playerValues.gameData.GetZoneTime(i) != -1 || !playerValues.gameData.checkEnabled(i))
            {
                if (!playerValues.gameData.GetIsNewGame() && i != 0)
                    zonesDialogs[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}