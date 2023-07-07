using System.Collections.Generic;
using UnityEngine;

public class ZoneCleaner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<GameObject> zones;
    private PlayerValues playerValues;

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        ZonesStartDisabler();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DisableZonesNotUsed(int index)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (i != index)
                zones[i].SetActive(false);
        }
    }

    private void ZonesStartDisabler()
    {
        for (int i = 1 ;i < zones.Count; i++)
        {
            if (!playerValues.gameData.checkEnabled(i))
                zones[i].SetActive(false);
        }
    }
}