using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(8)]
public class Objetives : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private GuiManager guiManager;
    [SerializeField] private AudioSource audioSource;


    void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        playerValues = FindObjectOfType<PlayerValues>();
        if (!playerValues.gameData.GetIsNewGame())
        {
            int lastZone = 0;
            bool found = false;
            for (int i = 0; i < 5; i++)
            {
                lastZone = i;
                if (!playerValues.gameData.checkEnabled(i))
                {
                    found = true;
                    lastZone--;
                    break;
                }
            }

            if (lastZone == 4 && !found)
                lastZone = 5;


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
            else if (lastZone == 5)
                SetNewObjetive("Beat your times");
            
        }
        else
        {
            SetNewObjetive("Explore");
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